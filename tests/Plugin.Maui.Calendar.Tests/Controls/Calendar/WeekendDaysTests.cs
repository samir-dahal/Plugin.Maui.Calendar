using System.Reflection;
using FluentAssertions;
using Microsoft.Maui.Controls;
using Plugin.Maui.Calendar.Tests.TestHelpers;
using Xunit;

namespace Plugin.Maui.Calendar.Tests.Controls.Calendar;

[Collection(MauiControlsCollection.Name)]
public class WeekendDaysTests
{
	static readonly Style weekendStyle = new(typeof(Label));

	// Titles are in grid order, starting at FirstDayOfWeek (Sunday here).
	static Label[] DayTitles(TestCalendar calendar) =>
		(Label[])typeof(Plugin.Maui.Calendar.Controls.Calendar)
			.GetField("dayTitleLabels", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(calendar)!;

	[Fact]
	public void IsWeekendDay_ShouldBeSaturdayAndSunday_ByDefault()
	{
		var calendar = new TestCalendar();

		calendar.IsWeekendDay(DayOfWeek.Saturday).Should().BeTrue();
		calendar.IsWeekendDay(DayOfWeek.Sunday).Should().BeTrue();
		calendar.IsWeekendDay(DayOfWeek.Friday).Should().BeFalse();
	}

	[Fact]
	public void IsWeekendDay_ShouldFollowWeekendDays()
	{
		var calendar = new TestCalendar { WeekendDays = [DayOfWeek.Saturday] };

		calendar.IsWeekendDay(DayOfWeek.Saturday).Should().BeTrue();
		calendar.IsWeekendDay(DayOfWeek.Sunday).Should().BeFalse();
	}

	[Fact]
	public void WeekendTitleStyle_ShouldOnlyApplyToWeekendDays_WhenWeekendDaysChanges()
	{
		var calendar = new TestCalendar { FirstDayOfWeek = DayOfWeek.Sunday, WeekendTitleStyle = weekendStyle };
		DayTitles(calendar)[0].Style.Should().BeSameAs(weekendStyle, "Sunday is a weekend day by default");

		calendar.WeekendDays = [DayOfWeek.Saturday];

		var titles = DayTitles(calendar);
		titles[0].Style.Should().NotBeSameAs(weekendStyle, "Sunday is no longer a weekend day");
		titles[6].Style.Should().BeSameAs(weekendStyle, "Saturday is still a weekend day");
	}
}
