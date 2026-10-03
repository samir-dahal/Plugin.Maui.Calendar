namespace Plugin.Maui.Calendar.Nepali;

/// <summary>
/// NepDate lists Nepali festivals and international observances together with no category, so
/// international days are recognised by their names ("World Heart Day", "International Day of …").
/// </summary>
public static class NepaliEventNames
{
	public static bool IsInternationalObservance(string eventName) =>
		eventName.StartsWith("World ", StringComparison.OrdinalIgnoreCase)
		|| eventName.StartsWith("International ", StringComparison.OrdinalIgnoreCase)
		|| eventName.Equals("United Nations Day", StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Returns the Nepali events of <paramref name="eventNames"/>, or all of them when every one is an
	/// international observance (so a holiday never ends up without a name).
	/// </summary>
	public static IReadOnlyList<string> WithoutInternationalObservances(IReadOnlyList<string> eventNames)
	{
		var nepaliEvents = eventNames.Where(name => !IsInternationalObservance(name)).ToList();
		return nepaliEvents.Count > 0 ? nepaliEvents : eventNames;
	}
}
