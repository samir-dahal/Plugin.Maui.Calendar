using FluentAssertions;
using Xunit;

namespace Plugin.Maui.Calendar.Nepali.Tests;

public class NepaliDayInfoTests
{
	[Fact]
	public void For_ShouldReturnHolidayWithEvents_ForConstitutionDay2083()
	{
		// Ashoj 3, 2083 is 19 September 2026.
		var info = NepaliDayInfo.For(new DateTime(2026, 9, 19));

		info.IsPublicHoliday.Should().BeTrue();
		info.Events.Should().Contain("Constitution Day");
		info.Tithi.Should().NotBeEmpty();
	}

	[Fact]
	public void For_ShouldReturnWorkingDay_WhenDayIsNotAHoliday()
	{
		// Ashoj 2, 2083 is 18 September 2026.
		NepaliDayInfo.For(new DateTime(2026, 9, 18)).IsPublicHoliday.Should().BeFalse();
	}

	[Fact]
	public void For_ShouldReturnEmptyInfo_BeyondNepDateCalendarData()
	{
		// BS 2090 has no tithi or event data in NepDate.
		var info = NepaliDayInfo.For(new NepaliCalendarSystem().MaxDate);

		info.Tithi.Should().BeEmpty();
		info.IsPublicHoliday.Should().BeFalse();
		info.Events.Should().BeEmpty();
	}
}
