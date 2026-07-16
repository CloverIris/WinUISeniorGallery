using System.Collections.ObjectModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Interactive Gallery route for the <see cref="CarouselView"/> P0 acceptance surface.</summary>
public sealed partial class CarouselViewPage : Page
{
    private readonly ObservableCollection<CarouselLabItem> _items = [];
    private readonly DispatcherQueueTimer _diagnosticsTimer;
    private bool _isReady;
    private bool _hostWindowActive = true;

    public CarouselViewPage()
    {
        InitializeComponent();
        _diagnosticsTimer = DispatcherQueue.CreateTimer();
        _diagnosticsTimer.Interval = TimeSpan.FromMilliseconds(250);
        _diagnosticsTimer.Tick += (_, _) => UpdateDiagnostics();

        ItemCountBox.SelectedIndex = 0;
        NavigationModeBox.SelectedIndex = 0;
        TransitionBox.SelectedIndex = 0;
        _isReady = true;
        ApplySettings();
    }

    internal void SetHostWindowActive(bool value)
    {
        _hostWindowActive = value;
        if (!_isReady)
        {
            return;
        }

        HostWindowActiveToggle.IsOn = value;
        Carousel.IsHostWindowActive = value;
        UpdateDiagnostics();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _diagnosticsTimer.Start();
        ApplySettings();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _diagnosticsTimer.Stop();
        base.OnNavigatedFrom(e);
    }

    private void OnSettingsChanged(object sender, RoutedEventArgs e)
    {
        if (_isReady)
        {
            ApplySettings();
        }
    }

    private void OnHostWindowActiveToggled(object sender, RoutedEventArgs e)
    {
        if (_isReady)
        {
            _hostWindowActive = HostWindowActiveToggle.IsOn;
            Carousel.IsHostWindowActive = _hostWindowActive;
            UpdateDiagnostics();
        }
    }

    private void OnPreviousClick(object sender, RoutedEventArgs e) => Carousel.MovePrevious();

    private void OnNextClick(object sender, RoutedEventArgs e) => Carousel.MoveNext();

    private void OnCarouselSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateDiagnostics();

    private void OnCarouselItemInvoked(object? sender, CarouselItemInvokedEventArgs e)
    {
        InvocationStatusText.Text = $"Last invoked: item {e.Index + 1} via {e.InputDeviceKind}.";
    }

    private void ApplySettings()
    {
        var itemCount = int.Parse(SelectedText(ItemCountBox, "5"), System.Globalization.CultureInfo.InvariantCulture);
        if (_items.Count != itemCount)
        {
            _items.Clear();
            for (var index = 0; index < itemCount; index++)
            {
                _items.Add(new CarouselLabItem(index, $"Local item {index + 1}", $"Deterministic Gallery content #{index + 1}."));
            }
        }

        Carousel.ItemsSource = _items;
        Carousel.NavigationMode = SelectedText(NavigationModeBox, "Bounded") == "Loop"
            ? CarouselNavigationMode.Loop
            : CarouselNavigationMode.Bounded;
        Carousel.Transition = ReducedMotionPreviewToggle.IsOn
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
        Carousel.IsHostWindowActive = _hostWindowActive;
        UpdateDiagnostics();
    }

    private void UpdateDiagnostics()
    {
        if (!_isReady)
        {
            return;
        }

        SelectionStatusText.Text = $"Selected index: {Carousel.SelectedIndex}; autoplay pause: {Carousel.AutoplayPauseReason}; realized elements: {Carousel.RealizedElementCount}.";
        EnvironmentStatusText.Text = $"Items: {_items.Count}; {Carousel.NavigationMode}; {Carousel.Transition}; RTL: {Carousel.FlowDirection == FlowDirection.RightToLeft}; host active: {Carousel.IsHostWindowActive}; reduced-motion preview: {ReducedMotionPreviewToggle.IsOn}.";
        if (string.IsNullOrEmpty(InvocationStatusText.Text))
        {
            InvocationStatusText.Text = "Last invoked: none. Focus the carousel and press Enter or Space to invoke the current item.";
        }
    }

    private static string SelectedText(ComboBox comboBox, string fallback) =>
        (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? fallback;
}

public sealed record CarouselLabItem(int Index, string Title, string Description);
