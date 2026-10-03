namespace NamaskarApp.Models;

/// <summary>A public holiday of the shown BS month: its BS day number and the day's event names.</summary>
public sealed record MonthHoliday(int Day, string Names);
