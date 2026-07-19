using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace WinUI3.Senior.Controls;

public enum ImmersiveReaderFocusMode
{
    None,
    Paragraph,
    Sentence,
}

public sealed record ReaderBlock(
    string Id,
    string Text,
    bool IsHeading = false,
    int HeadingLevel = 0,
    object? Tag = null)
{
    public int SafeHeadingLevel => IsHeading ? Math.Clamp(HeadingLevel <= 0 ? 1 : HeadingLevel, 1, 6) : 0;
}

public sealed class ReaderBlockInvokedEventArgs(ReaderBlock block, int index, bool isUserInitiated) : EventArgs
{
    public ReaderBlock Block { get; } = block ?? throw new ArgumentNullException(nameof(block));
    public int Index { get; } = index;
    public bool IsUserInitiated { get; } = isUserInitiated;
}

public sealed class ReaderFocusChangedEventArgs(ReaderBlock? previous, ReaderBlock? current, int index) : EventArgs
{
    public ReaderBlock? Previous { get; } = previous;
    public ReaderBlock? Current { get; } = current;
    public int Index { get; } = index;
}

public sealed class ReaderSpeechRequestedEventArgs(ReaderBlock block, bool isStart) : EventArgs
{
    public ReaderBlock Block { get; } = block ?? throw new ArgumentNullException(nameof(block));
    public bool IsStart { get; } = isStart;
    public bool Handled { get; set; }
}

/// <summary>
/// A virtualized, provider-neutral reading surface. It owns focus and reading requests;
/// speech synthesis, document persistence and file parsing remain host-owned.
/// </summary>
[TemplatePart(Name = "PART_ItemsRepeater", Type = typeof(ItemsRepeater))]
[TemplatePart(Name = "PART_LiveRegion", Type = typeof(TextBlock))]
public sealed class ImmersiveReader : Control
{
    private readonly ObservableCollection<ReaderBlock> _blocks = new();
    private ItemsRepeater? _itemsRepeater;
    private TextBlock? _liveRegion;
    private bool _isApplyingBlocks;

    public static readonly DependencyProperty FocusedBlockIdProperty = DependencyProperty.Register(
        nameof(FocusedBlockId), typeof(string), typeof(ImmersiveReader), new PropertyMetadata(null, OnFocusPropertyChanged));
    public static readonly DependencyProperty FocusModeProperty = DependencyProperty.Register(
        nameof(FocusMode), typeof(ImmersiveReaderFocusMode), typeof(ImmersiveReader), new PropertyMetadata(ImmersiveReaderFocusMode.Paragraph, OnVisualPropertyChanged));
    public static readonly DependencyProperty FontScaleProperty = DependencyProperty.Register(
        nameof(FontScale), typeof(double), typeof(ImmersiveReader), new PropertyMetadata(1d, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsLineFocusEnabledProperty = DependencyProperty.Register(
        nameof(IsLineFocusEnabled), typeof(bool), typeof(ImmersiveReader), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsReadingAloudProperty = DependencyProperty.Register(
        nameof(IsReadingAloud), typeof(bool), typeof(ImmersiveReader), new PropertyMetadata(false, OnReadingChanged));

    public ImmersiveReader()
    {
        DefaultStyleKey = typeof(ImmersiveReader);
        KeyDown += OnKeyDown;
    }

    public ObservableCollection<ReaderBlock> Blocks => _blocks;
    public string? FocusedBlockId { get => (string?)GetValue(FocusedBlockIdProperty); private set => SetValue(FocusedBlockIdProperty, value); }
    public ImmersiveReaderFocusMode FocusMode { get => (ImmersiveReaderFocusMode)GetValue(FocusModeProperty); set => SetValue(FocusModeProperty, value); }
    public double FontScale { get => (double)GetValue(FontScaleProperty); set => SetValue(FontScaleProperty, Math.Clamp(value, .8, 2.5)); }
    public bool IsLineFocusEnabled { get => (bool)GetValue(IsLineFocusEnabledProperty); set => SetValue(IsLineFocusEnabledProperty, value); }
    public bool IsReadingAloud { get => (bool)GetValue(IsReadingAloudProperty); private set => SetValue(IsReadingAloudProperty, value); }
    public int FocusedIndex => FocusedBlockId is null ? -1 : _blocks.ToList().FindIndex(block => string.Equals(block.Id, FocusedBlockId, StringComparison.Ordinal));
    public ReaderBlock? FocusedBlock
    {
        get
        {
            var index = FocusedIndex;
            return index >= 0 && index < _blocks.Count ? _blocks[index] : null;
        }
    }

    public event EventHandler<ReaderBlockInvokedEventArgs>? BlockInvoked;
    public event EventHandler<ReaderFocusChangedEventArgs>? FocusChanged;
    public event EventHandler<ReaderSpeechRequestedEventArgs>? SpeechRequested;

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    public void SetBlocks(IEnumerable<ReaderBlock> blocks)
    {
        ArgumentNullException.ThrowIfNull(blocks);
        var normalized = blocks
            .Where(block => block is not null && !string.IsNullOrWhiteSpace(block.Id) && !string.IsNullOrWhiteSpace(block.Text))
            .GroupBy(block => block.Id, StringComparer.Ordinal)
            .Select(group => group.First())
            .ToArray();
        _isApplyingBlocks = true;
        try
        {
            _blocks.Clear();
            foreach (var block in normalized) _blocks.Add(block);
            if (FocusedBlock is null) FocusedBlockId = _blocks.FirstOrDefault()?.Id;
            else if (!_blocks.Any(block => string.Equals(block.Id, FocusedBlockId, StringComparison.Ordinal))) FocusedBlockId = _blocks.FirstOrDefault()?.Id;
        }
        finally { _isApplyingBlocks = false; }
        UpdateTemplateSource();
        UpdateVisualState();
    }

    public bool FocusBlock(string id, bool isUserInitiated = false)
    {
        var block = _blocks.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
        if (block is null) return false;
        var previous = FocusedBlock;
        FocusedBlockId = block.Id;
        if (isUserInitiated)
            BlockInvoked?.Invoke(this, new ReaderBlockInvokedEventArgs(block, FocusedIndex, true));
        return !ReferenceEquals(previous, block);
    }

    public bool MoveFocus(int delta)
    {
        if (_blocks.Count == 0 || delta == 0) return false;
        var current = FocusedIndex < 0 ? (delta > 0 ? -1 : _blocks.Count) : FocusedIndex;
        var next = Math.Clamp(current + Math.Sign(delta), 0, _blocks.Count - 1);
        var changed = FocusBlock(_blocks[next].Id);
        if (changed && _itemsRepeater?.TryGetElement(next) is FrameworkElement element) element.StartBringIntoView();
        return changed;
    }

    public bool ToggleReading()
    {
        if (FocusedBlock is not { } block) return false;
        IsReadingAloud = !IsReadingAloud;
        var args = new ReaderSpeechRequestedEventArgs(block, IsReadingAloud);
        SpeechRequested?.Invoke(this, args);
        // A handled request means the host accepted responsibility for speech.
        // If nobody handles a start request, do not leave the surface claiming it
        // is reading aloud when no speech provider exists.
        if (!args.Handled && IsReadingAloud) IsReadingAloud = false;
        return true;
    }

    protected override void OnApplyTemplate()
    {
        if (_itemsRepeater is not null)
        {
            _itemsRepeater.PointerPressed -= OnRepeaterPointerPressed;
            _itemsRepeater.ElementPrepared -= OnElementPrepared;
        }
        base.OnApplyTemplate();
        _itemsRepeater = GetTemplateChild("PART_ItemsRepeater") as ItemsRepeater;
        _liveRegion = GetTemplateChild("PART_LiveRegion") as TextBlock;
        if (_itemsRepeater is not null)
        {
            _itemsRepeater.PointerPressed += OnRepeaterPointerPressed;
            _itemsRepeater.ElementPrepared += OnElementPrepared;
        }
        UpdateTemplateSource();
        UpdateVisualState();
    }

    private static void OnFocusPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var reader = (ImmersiveReader)sender;
        if (reader._isApplyingBlocks) return;
        var previous = args.OldValue as string;
        var current = reader.FocusedBlock;
        if (reader._itemsRepeater?.TryGetElement(reader.FocusedIndex) is FrameworkElement focusedElement)
            focusedElement.StartBringIntoView();
        if (current is not null)
        {
            if (reader._liveRegion is not null) reader._liveRegion.Text = current.Text;
            reader.FocusChanged?.Invoke(reader, new ReaderFocusChangedEventArgs(
                reader._blocks.FirstOrDefault(block => string.Equals(block.Id, previous, StringComparison.Ordinal)), current, reader.FocusedIndex));
        }
        reader.UpdateVisualState();
    }

    private static void OnVisualPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => ((ImmersiveReader)sender).UpdateVisualState();
    private static void OnReadingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) => ((ImmersiveReader)sender).UpdateVisualState();

    private void UpdateTemplateSource()
    {
        if (_itemsRepeater is not null) _itemsRepeater.ItemsSource = _blocks;
    }

    private void OnElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args) => ApplyBlockStyle(args.Element as TextBlock, (args.Element as FrameworkElement)?.DataContext as ReaderBlock);

    private void ApplyRealizedStyles()
    {
        if (_itemsRepeater is null) return;
        for (var index = 0; index < _blocks.Count; index++)
        {
            if (_itemsRepeater.TryGetElement(index) is TextBlock text)
                ApplyBlockStyle(text, _blocks[index]);
        }
    }

    private void ApplyBlockStyle(TextBlock? text, ReaderBlock? block)
    {
        if (text is null || block is null) return;
        text.FontSize = 18 * Math.Clamp(FontScale, .8, 2.5) * (block.IsHeading ? 1.2 : 1);
        text.FontWeight = block.IsHeading ? Windows.UI.Text.FontWeights.SemiBold : Windows.UI.Text.FontWeights.Normal;
        AutomationProperties.SetName(text, block.Text);
        text.Opacity = IsLineFocusEnabled && FocusedBlockId is not null && !string.Equals(FocusedBlockId, block.Id, StringComparison.Ordinal) ? .4 : 1;
    }

    private void OnRepeaterPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs args)
    {
        if (args.OriginalSource is FrameworkElement element && element.DataContext is ReaderBlock block)
        {
            FocusBlock(block.Id, true);
            args.Handled = true;
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        switch (args.Key)
        {
            case VirtualKey.Up:
                args.Handled = MoveFocus(-1);
                break;
            case VirtualKey.Down:
                args.Handled = MoveFocus(1);
                break;
            case VirtualKey.Enter:
            case VirtualKey.Space:
                if (FocusedBlock is { } block)
                {
                    BlockInvoked?.Invoke(this, new ReaderBlockInvokedEventArgs(block, FocusedIndex, true));
                    args.Handled = true;
                }
                break;
            case VirtualKey.R:
                args.Handled = ToggleReading();
                break;
        }
    }

    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, _blocks.Count == 0 ? "Empty" : "Ready", true);
        VisualStateManager.GoToState(this, IsReadingAloud ? "Reading" : "Idle", true);
        VisualStateManager.GoToState(this, FocusMode.ToString(), true);
        ApplyRealizedStyles();
    }
}
