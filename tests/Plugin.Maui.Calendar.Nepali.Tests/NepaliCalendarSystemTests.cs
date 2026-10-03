using FluentAssertions;
using Xunit;

namespace Plugin.Maui.Calendar.Nepali.Tests;

public class NepaliCalendarSystemTests
{
	// BS 2083 Baishakh 1 (Nepali New Year) fell on 14 April 2026.
	static readonly DateTime newYear2083 = new(2026, 4, 14);

	readonly NepaliCalendarSystem calendarSystem = new();

	[Fact]
	public void GetMonthStart_ShouldReturnBaishakhFirst_ForAnyDayOfBaishakh2083()
	{
		calendarSystem.GetMonthStart(newYear2083.AddDays(20)).Should().Be(newYear2083);
	}

	[Fact]
	public void Labels_ShouldBeBikramSambatValues()
	{
		calendarSystem.GetYear(newYear2083).Should().Be(2083);
		calendarSystem.GetDayOfMonth(newYear2083).Should().Be(1);
		calendarSystem.GetMonthName(newYear2083).Should().Be("Baishakh");
		calendarSystem.GetYear(newYear2083.AddDays(-1)).Should().Be(2082);
		calendarSystem.GetMonthName(newYear2083.AddDays(-1)).Should().Be("Chaitra");
	}

	[Fact]
	public void Range_ShouldSpanBaishakhFirstOfMinYearToLastDayOfChaitraOfMaxYear()
	{
		var system = new NepaliCalendarSystem(2080, 2100);

		system.GetYear(system.MinDate).Should().Be(2080);
		system.GetMonthName(system.MinDate).Should().Be("Baishakh");
		system.GetDayOfMonth(system.MinDate).Should().Be(1);

		system.GetYear(system.MaxDate).Should().Be(2100);
		system.GetMonthName(system.MaxDate).Should().Be("Chaitra");
		system.GetYear(system.MaxDate.AddDays(1)).Should().Be(2101);
		system.GetDayOfMonth(system.MaxDate.AddDays(1)).Should().Be(1);
	}

	[Fact]
	public void AddMonths_ShouldRollOverIntoNextYear()
	{
		var chaitra2082 = newYear2083.AddDays(-1);

		var result = calendarSystem.AddMonths(chaitra2082, 1);

		calendarSystem.GetYear(result).Should().Be(2083);
		calendarSystem.GetMonthName(result).Should().Be("Baishakh");
	}

	[Fact]
	public void AddMonths_ShouldClampDayToEndOfShorterMonth()
	{
		var baishakh31 = newYear2083.AddDays(30);
		calendarSystem.GetDayOfMonth(baishakh31).Should().Be(31);

		var result = calendarSystem.AddMonths(baishakh31, 11);

		calendarSystem.GetMonthName(result).Should().Be("Chaitra");
		calendarSystem.GetYear(result).Should().Be(2083);
		calendarSystem.GetMonthName(result.AddDays(1)).Should().Be("Baishakh", "the day is clamped to the last day of Chaitra");
	}

	[Fact]
	public void AddMonths_ShouldStopAtLastSupportedMonth()
	{
		var result = calendarSystem.AddMonths(calendarSystem.MaxDate, 1);

		calendarSystem.GetMonthStart(result).Should().Be(calendarSystem.GetMonthStart(calendarSystem.MaxDate));
	}

	[Fact]
	public void AddMonths_ShouldStopAtFirstSupportedMonth()
	{
		var result = calendarSystem.AddMonths(calendarSystem.MinDate, -1);

		calendarSystem.GetMonthStart(result).Should().Be(calendarSystem.MinDate);
	}

	[Fact]
	public void Members_ShouldAcceptDatesInTheSixWeekGridMarginAroundTheRange()
	{
		var beforeRange = calendarSystem.MinDate.AddDays(-42);
		var afterRange = calendarSystem.MaxDate.AddDays(42);

		var act = () =>
		{
			calendarSystem.GetDayOfMonth(beforeRange);
			calendarSystem.GetMonthStart(beforeRange);
			calendarSystem.GetDayOfMonth(afterRange);
			calendarSystem.GetMonthStart(afterRange);
		};

		act.Should().NotThrow();
	}

	[Theory]
	[InlineData(1901, 2100)]
	[InlineData(2000, 2199)]
	[InlineData(2100, 2000)]
	public void Constructor_ShouldRejectRangesOutsideNepDateSupport(int minYear, int maxYear)
	{
		var act = () => new NepaliCalendarSystem(minYear, maxYear);

		act.Should().Throw<ArgumentOutOfRangeException>();
	}
}
