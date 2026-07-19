using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class BigTitlePage : Page
{
    public IReadOnlyList<string> Paragraphs { get; } = Enumerable.Range(1, 16).Select(index => $"Section {index:00} · This synthetic paragraph provides scroll distance for the BigTitle lab.").ToArray();

    public BigTitlePage()
    {
        InitializeComponent();
        DataContext = this;
        Title.StateChanged += (_, _) => { };
    }

    private void OnScrollChanged(object sender, ScrollViewerViewChangedEventArgs e) { }
}
