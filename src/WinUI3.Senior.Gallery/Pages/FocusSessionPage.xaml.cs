using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Deterministic local lab for the FocusSession phase/task state machine.</summary>
public sealed partial class FocusSessionPage : Page
{
    public FocusSessionPage()
    {
        InitializeComponent();
        Session.WorkDuration = TimeSpan.FromSeconds(30);
        Session.ShortBreakDuration = TimeSpan.FromSeconds(10);
        Session.LongBreakDuration = TimeSpan.FromSeconds(20);
        Session.CyclesBeforeLongBreak = 2;
        Session.ReplaceTasks(new[]
        {
            new FocusSessionTask("spec", "Review the specification"),
            new FocusSessionTask("model", "Shape the view model"),
            new FocusSessionTask("handoff", "Write the implementation handoff")
        });
        Session.StateChanged += OnStateChanged;
        Session.PhaseChanged += OnPhaseChanged;
        UpdateStatus();
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        if (Session.State == FocusSessionState.Paused) Session.Resume();
        else Session.Start();
    }

    private void OnPauseClick(object sender, RoutedEventArgs e) => Session.Pause();
    private void OnSkipClick(object sender, RoutedEventArgs e) => Session.SkipPhase();
    private void OnCompleteTaskClick(object sender, RoutedEventArgs e) => Session.CompleteActiveTask();
    private void OnResetClick(object sender, RoutedEventArgs e) => Session.Reset();

    private void OnStateChanged(object? sender, FocusSessionStateChangedEventArgs e) => UpdateStatus(e.Snapshot);

    private void OnPhaseChanged(object? sender, FocusSessionPhaseChangedEventArgs e) =>
        Status.Text = $"Phase: {e.Current}; cycle {e.Cycle}";

    private void UpdateStatus(FocusSessionSnapshot? snapshot = null)
    {
        var state = snapshot ?? Session.Snapshot;
        Status.Text = $"{state.State} · {state.Phase} · cycle {state.Cycle} · remaining {state.Remaining:c} · {state.Tasks.Count(task => task.IsCompleted)}/{state.Tasks.Count} tasks";
    }
}
