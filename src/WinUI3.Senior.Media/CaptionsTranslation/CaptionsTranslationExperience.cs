using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Media;

public enum CaptionsTranslationState
{
    Idle,
    Listening,
    Translating,
    Degraded,
    Error
}

public sealed record CaptionsTranslationRevision(
    long Revision,
    TimedTextDocument Source,
    TimedTextDocument? Translation = null,
    bool IsFinal = false,
    string? ErrorCode = null)
{
    public CaptionsTranslationRevision Normalize()
    {
        if (Revision < 0) throw new ArgumentOutOfRangeException(nameof(Revision));
        ArgumentNullException.ThrowIfNull(Source);
        return this;
    }
}

public sealed class CaptionsTranslationRevisionAppliedEventArgs(CaptionsTranslationRevision revision, bool accepted) : EventArgs
{
    public CaptionsTranslationRevision Revision { get; } = revision ?? throw new ArgumentNullException(nameof(revision));
    public bool Accepted { get; } = accepted;
}

/// <summary>
/// Composition layer for host-provided ASR/translation revisions. It owns ordering,
/// fallback and TimedTextView projection only; provider/network/recognition implementations
/// remain outside this package.
/// </summary>
[TemplatePart(Name = "PART_TimedTextView", Type = typeof(TimedTextView))]
[TemplatePart(Name = "PART_Status", Type = typeof(TextBlock))]
public sealed class CaptionsTranslationExperience : Control
{
    private TimedTextView? _timedTextView;
    private TextBlock? _status;
    private long _revision = -1;
    private CaptionsTranslationRevision? _current;

    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(CaptionsTranslationState), typeof(CaptionsTranslationExperience), new PropertyMetadata(CaptionsTranslationState.Idle, OnVisualChanged));
    public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(TimedTextDisplayMode), typeof(CaptionsTranslationExperience), new PropertyMetadata(TimedTextDisplayMode.Bilingual, OnVisualChanged));
    public static readonly DependencyProperty IsAutoFollowEnabledProperty = DependencyProperty.Register(nameof(IsAutoFollowEnabled), typeof(bool), typeof(CaptionsTranslationExperience), new PropertyMetadata(true, OnVisualChanged));

    public CaptionsTranslationExperience() => DefaultStyleKey = typeof(CaptionsTranslationExperience);

    public CaptionsTranslationState State { get => (CaptionsTranslationState)GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public TimedTextDisplayMode DisplayMode { get => (TimedTextDisplayMode)GetValue(DisplayModeProperty); set => SetValue(DisplayModeProperty, value); }
    public bool IsAutoFollowEnabled { get => (bool)GetValue(IsAutoFollowEnabledProperty); set => SetValue(IsAutoFollowEnabledProperty, value); }
    public long Revision => _revision;
    public CaptionsTranslationRevision? CurrentRevision => _current;
    public TimeSpan Position { get; private set; }
    public event EventHandler<CaptionsTranslationRevisionAppliedEventArgs>? RevisionApplied;
    public event EventHandler? StateChanged;

    public bool ApplyRevision(CaptionsTranslationRevision revision)
    {
        ArgumentNullException.ThrowIfNull(revision);
        revision = revision.Normalize();
        if (revision.Revision <= _revision)
        {
            RevisionApplied?.Invoke(this, new CaptionsTranslationRevisionAppliedEventArgs(revision, false));
            return false;
        }
        _revision = revision.Revision;
        _current = revision;
        State = revision.ErrorCode is null ? revision.IsFinal ? CaptionsTranslationState.Idle : CaptionsTranslationState.Listening : CaptionsTranslationState.Degraded;
        ApplyProjection();
        RevisionApplied?.Invoke(this, new CaptionsTranslationRevisionAppliedEventArgs(revision, true));
        return true;
    }

    public void SetPosition(TimeSpan position)
    {
        Position = position < TimeSpan.Zero ? TimeSpan.Zero : position;
        if (_timedTextView is not null) _timedTextView.Position = Position;
    }

    public void SetError(string errorCode)
    {
        State = string.IsNullOrWhiteSpace(errorCode) ? CaptionsTranslationState.Error : CaptionsTranslationState.Degraded;
        if (_status is not null) _status.Text = string.IsNullOrWhiteSpace(errorCode) ? "Caption provider error" : $"Degraded: {errorCode}";
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _timedTextView = GetTemplateChild("PART_TimedTextView") as TimedTextView;
        _status = GetTemplateChild("PART_Status") as TextBlock;
        ApplyProjection();
    }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs _)
    {
        var owner = (CaptionsTranslationExperience)d;
        owner.ApplyProjection();
        owner.StateChanged?.Invoke(owner, EventArgs.Empty);
    }

    private void ApplyProjection()
    {
        if (_timedTextView is not null)
        {
            _timedTextView.DisplayMode = DisplayMode;
            _timedTextView.IsAutoScrollEnabled = IsAutoFollowEnabled;
            _timedTextView.Position = Position;
            _timedTextView.Document = BuildProjectionDocument(_current);
        }
        if (_status is not null)
        {
            _status.Text = _current is null ? "Waiting for host-provided captions" : $"{State} · revision {_revision}";
        }
    }

    private static TimedTextDocument? BuildProjectionDocument(CaptionsTranslationRevision? revision)
    {
        if (revision is null) return null;
        if (revision.Translation is null) return revision.Source;
        var tracks = revision.Source.Tracks
            .Concat(revision.Translation.Tracks)
            .GroupBy(track => track.Id, StringComparer.Ordinal)
            .Select(group => group.First())
            .ToArray();
        return revision.Source with { Revision = Math.Max(revision.Source.Revision, revision.Translation.Revision), Tracks = tracks };
    }
}
