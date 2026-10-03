using FluentAssertions;
using Xunit;

namespace Plugin.Maui.Calendar.Nepali.Tests;

public class NepaliEventNamesTests
{
	[Theory]
	[InlineData("World Heart Day")]
	[InlineData("International Day for the Eradication of Poverty")]
	[InlineData("United Nations Day")]
	public void IsInternationalObservance_ShouldBeTrue_ForInternationalDays(string name)
	{
		NepaliEventNames.IsInternationalObservance(name).Should().BeTrue();
	}

	[Theory]
	[InlineData("Fulpati")]
	[InlineData("Constitution Day")]
	[InlineData("National Film Day")]
	[InlineData("Indra Jatra (Kathmandu Valley Holiday)")]
	public void IsInternationalObservance_ShouldBeFalse_ForNepaliEvents(string name)
	{
		NepaliEventNames.IsInternationalObservance(name).Should().BeFalse();
	}

	[Fact]
	public void WithoutInternationalObservances_ShouldKeepOnlyNepaliEvents()
	{
		string[] events = ["Fulpati", "Dashain Holiday", "International Day for the Eradication of Poverty"];

		NepaliEventNames.WithoutInternationalObservances(events).Should().Equal("Fulpati", "Dashain Holiday");
	}

	[Fact]
	public void WithoutInternationalObservances_ShouldKeepAll_WhenAllAreInternational()
	{
		string[] events = ["World Heart Day"];

		NepaliEventNames.WithoutInternationalObservances(events).Should().Equal("World Heart Day");
	}
}
