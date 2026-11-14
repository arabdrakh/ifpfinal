namespace MoodLog.Core.Services;

/// <summary>Lightweight circuit-scoped notification of changes to wellness data.</summary>
public sealed class WellnessState
{
    public event Action? Changed;
    public void NotifyChanged() => Changed?.Invoke();
}
