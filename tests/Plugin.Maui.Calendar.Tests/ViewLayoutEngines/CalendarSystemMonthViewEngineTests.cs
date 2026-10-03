using FluentAssertions;
using Plugin.Maui.Calendar.Controls.ViewLayoutEngines;
using Plugin.Maui.Calendar.Interfaces;
using Xunit;

namespace Plugin.Maui.Calendar.Tests.ViewLayoutEngines;

public class CalendarSystemMonthViewEngineTests
{
	// Months start on the 10th of each Gregorian month, so the engine cannot pass by
	// accidentally using Gregorian month boundaries.
	sealed class OffsetMonthCalendarSystem(DateTime minDate, DateTime maxDate) : ICalendarSystem
	{
		public DateTime MinDate { get; } = minDate;

		public DateTime MaxDate { get; } = maxDate;

		public DateTime GetMonthStart(DateTime date)
		{
			var start = new DateTime(date.Year, date.Month, 10);
			return date.Day >= 10 ? start : start.AddMonths(-1);
		}

		public DateTime AddMonths(DateTime date, int months)
		{
			var target = GetMonthStart(date).AddMonths(months);
			if (target < GetMonthStart(MinDate))
			{
				return GetMonthStart(MinDate);
			}
			return target > GetMonthStart(MaxDate) ? GetMonthStart(MaxDate) : target;
		}

		public int GetYear(DateTime date) => GetMonthStart(date).Year;

		public int GetDayOfMonth(DateTime date) => (date - GetMonthStart(date)).Days + 1;

		public string GetMonthName(DateTime date) => GetMonthStart(date).ToString("MMM");
	}

	static readonly OffsetMonthCalendarSystem calendarSystem = new(new DateTime(2024, 1, 10), new DateTime(2024, 12, 9));

	readonly CalendarSystemMonthViewEngine engine = new(DayOfWeek.Sunday, calendarSystem);

	[Fact]
	public void GetFirstDate_ShouldBeStartOfWeekContainingCalendarSystemMonthStart()
	{
		// Month containing 2024-05-05 starts on Wednesday 2024-04-10; its week starts Sunday 2024-04-07.
		engine.GetFirstDate(new DateTime(2024, 5, 5)).Should().Be(new DateTime(2024, 4, 7));
	}

	[Fact]
	public void GetLastDate_ShouldBeFortyOneDaysAfterFirstDate()
	{
		var shownDate = new DateTime(2024, 5, 15);

		engine.GetLastDate(shownDate).Should().Be(engine.GetFirstDate(shownDate).AddDays(41));
	}

	[Fact]
	public void GetNextUnit_ShouldMoveByCalendarSystemMonths()
	{
		engine.GetNextUnit(new DateTime(2024, 5, 15), 2).Should().Be(new DateTime(2024, 7, 10));
	}

	[Fact]
	public void GetPreviousUnit_ShouldMoveBackByCalendarSystemMonths()
	{
		engine.GetPreviousUnit(new DateTime(2024, 5, 15)).Should().Be(new DateTime(2024, 4, 10));
	}

	[Fact]
	public void GetNextUnit_ShouldReturnSameDate_WhenNumberOfUnitsIsZero()
	{
		var date = new DateTime(2024, 5, 15, 13, 45, 0);

		engine.GetNextUnit(date, 0).Should().Be(date);
	}

	[Fact]
	public void Navigation_ShouldStopAtSupportedRange()
	{
		engine.GetNextUnit(calendarSystem.MaxDate, 5).Should().Be(calendarSystem.GetMonthStart(calendarSystem.MaxDate));
		engine.GetPreviousUnit(calendarSystem.MinDate, 5).Should().Be(calendarSystem.MinDate);
	}

	[Fact]
	public void GetFirstDate_ShouldClampDatesOutsideSupportedRange()
	{
		engine.GetFirstDate(new DateTime(2030, 1, 1)).Should().Be(engine.GetFirstDate(calendarSystem.MaxDate));
		engine.GetFirstDate(new DateTime(2000, 1, 1)).Should().Be(engine.GetFirstDate(calendarSystem.MinDate));
	}
}
