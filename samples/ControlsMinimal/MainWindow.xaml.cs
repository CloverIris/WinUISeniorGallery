using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3.Senior.Samples.ControlsMinimal;

/// <summary>Unpackaged, local-only development surface for CarouselView.</summary>
public sealed partial class MainWindow : Window
{
    private readonly ObservableCollection<SampleItem> _items = [];
    private bool _isReady;

    public MainWindow()
    {
        InitializeComponent();
        Activated += OnWindowActivated;
        ItemCountBox.SelectedIndex = 0;
        NavigationModeBox.SelectedIndex = 0;
        TransitionBox.SelectedIndex = 0;
        _isReady = true;
        ApplySettings();
    }

    private void OnSettingsChanged(object sender, RoutedEventArgs e)
    {
        if (_isReady)
        {
            ApplySettings();
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        Carousel.IsHostWindowActive = args.WindowActivationState != WindowActivationState.Deactivated;
        UpdateStatus();
    }

    private void OnPreviousClick(object sender, RoutedEventArgs e) => Carousel.MovePrevious();

    private void OnNextClick(object sender, RoutedEventArgs e) => Carousel.MoveNext();

    private void OnCarouselSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateStatus();

    private void OnCarouselItemInvoked(object? sender, CarouselItemInvokedEventArgs e)
    {
        StatusText.Text = $"Last invoked: item {e.Index + 1} via {e.InputDeviceKind}.";
    }

    private void ApplySettings()
    {
        var count = int.Parse(SelectedText(ItemCountBox, "5"), System.Globalization.CultureInfo.InvariantCulture);
        if (_items.Count != count)
        {
            _items.Clear();
            for (var index = 0; index < count; index++)
            {
                _items.Add(new SampleItem(index, $"Local item {index + 1}", $"Deterministic local content #{index + 1}"));
            }
        }

        Carousel.ItemsSource = _items;
        Carousel.NavigationMode = SelectedText(NavigationModeBox, "Bounded") == "Loop"
            ? CarouselNavigationMode.Loop
            : CarouselNavigationMode.Bounded;
        Carousel.Transition = ReducedMotionToggle.IsOn
            ? CarouselTransition.Fade
            : SelectedText(TransitionBox, "Slide") switch
            {
                "Fade" => CarouselTransition.Fade,
                "CoverFlow" => CarouselTransition.CoverFlow,
                _ => CarouselTransition.Slide,
            };
        Carousel.IsAutoplayEnabled = AutoplayToggle.IsOn;
        Carousel.IsAdjacentPreviewEnabled = PreviewToggle.IsOn;
        Carousel.FlowDirection = RtlToggle.IsOn ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (!_isReady)
        {
            return;
        }

        StatusText.Text = $"Items: {_items.Count}; selected: {Carousel.SelectedIndex}; {Carousel.NavigationMode}; {Carousel.Transition}; pause: {Carousel.AutoplayPauseReason}; realized: {Carousel.RealizedElementCount}; RTL: {Carousel.FlowDirection == FlowDirection.RightToLeft}; reduced-motion preview: {ReducedMotionToggle.IsOn}.";
    }

    private static string SelectedText(ComboBox comboBox, string fallback) =>
        (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? fallback;

    private sealed record SampleItem(int Index, string Title, string Description);
}
