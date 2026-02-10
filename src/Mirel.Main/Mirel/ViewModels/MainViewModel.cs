using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Mirel.Classes.Entries;
using Mirel.Const;
using Mirel.Module.Event;
using Mirel.Views.Main.Pages;

namespace Mirel.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        InitializationEvents.AfterUiLoaded += () =>
        {
            Tabs.Add(new TabEntry(new HomePage(), headerContent: CeateDockPanel(), minWidth: 65));
            Tabs.Add(new TabEntry(new NewTabPage()));
            SelectedTab = Tabs[1];
        };
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(SelectedTab) || SelectedTab == null) return;
            if (SelectedTab.Content.RootElement == null) return;
            SelectedTab.Content.RootElement.IsVisible = false;
            SelectedTab.Content.InAnimator.Animate();
        };
        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
        timer.Tick += (_, _) => Time = DateTime.Now;
        timer.Start();
    }

    public ObservableCollection<TabEntry> Tabs { get; set; } = [];
    public static ObservableCollection<NotificationEntry> Notifications => UiProperty.Notifications;
    public Aside Aside { get; } = new();

    public Vector TabScrollOffset
    {
        get;
        set => SetField(ref field, value);
    }

    public bool IsTabMaskVisible
    {
        get;
        set => SetField(ref field, value);
    }

    public TabEntry? SelectedTab
    {
        get;
        set => SetField(ref field, value);
    }

    public DateTime Time
    {
        get;
        set => SetField(ref field, value);
    } = DateTime.Now;

    public void CreateTab(TabEntry tab)
    {
        Tabs.Add(tab);
        SelectedTab = tab;
    }


    private DockPanel CeateDockPanel()
    {
        var rootDockPanel = new DockPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new Image
                {
                    Height = 24,
                    Width = 24,
                    Margin = new Thickness(0),
                    VerticalAlignment = VerticalAlignment.Center,
                    Source = new DrawingImage
                    {
                        Drawing = new DrawingGroup
                        {
                            ClipGeometry = Geometry.Parse("M0,0 V280 H280 V0 H0 Z"),
                            Children =
                            {
                                new DrawingGroup
                                {
                                    Opacity = 1,
                                    Children =
                                    {
                                        new GeometryDrawing
                                        {
                                            Brush = new SolidColorBrush(Color.Parse("#FF5EB5EA")),
                                            Geometry = Geometry.Parse(
                                                "F1 M280,280z M0,0z M77.63,91.65C79.43,88.07 81.59,83.62 86.04,83.01 92.99,82.2 100.01,82.9 107,82.62 111.24,82.76 116.37,81.99 119.63,85.38 122.69,88.72 124.58,92.93 126.33,97.06 123.54,104.05 119.23,110.26 115.88,116.96 109.81,129.28 103.19,141.31 97.12,153.63 90.79,164.97 85.34,176.77 79.19,188.21 77.31,191.47 75.92,195.54 72.4,197.41 68.11,198.9 63.46,198.23 59,198.34 53.75,198.14 48.38,198.92 43.21,197.73 38.91,196.07 35.62,192.25 33.85,188.07 32.19,184.32 34.59,180.56 36.15,177.25 50.16,148.8 63.77,120.16 77.63,91.65z")
                                        },
                                        new GeometryDrawing
                                        {
                                            Brush = new SolidColorBrush(Color.Parse("#FF5EB5EA")),
                                            Geometry = Geometry.Parse(
                                                "F1 M280,280z M0,0z M159.91,90.93C161.8,87.68 163.91,83.63 168.03,83.02 174.98,82.16 182.02,82.92 189,82.6 193.03,82.72 197.8,81.97 200.98,85.04 205.04,89.05 206.82,94.65 209.43,99.6 219.96,122.34 231,144.84 241.48,167.6 243.8,172.87 246.98,177.8 248.57,183.37 249.96,188.6 245.85,193.12 242.03,196.06 238.37,199.01 233.37,198.25 229,198.37 223.05,198.12 217.04,198.84 211.13,197.9 207.81,197.41 205.62,194.57 204.25,191.74 199.64,182.1 194.74,172.59 190.13,162.95 187.33,157.59 185.02,151.97 181.76,146.87 179.35,149.32 178.13,152.6 176.52,155.58 171.14,165.89 165.81,176.23 160.62,186.64 158.29,190.69 156.44,195.97 151.67,197.69 145.55,199.03 139.22,198.04 133,198.37 128.54,198.23 122.87,199.36 119.8,195.21 116.54,190.72 113.86,185.7 112.16,180.41 114.7,176.49 117.13,172.5 119.27,168.35 126.78,154.01 134.45,139.76 141.73,125.3 148.18,114.05 153.77,102.35 159.91,90.93z")
                                        }
                                    }
                                },
                                new DrawingGroup
                                {
                                    Opacity = 1,
                                    Children =
                                    {
                                        new GeometryDrawing
                                        {
                                            Brush = new SolidColorBrush(Color.Parse("#FF9DD7F6")),
                                            Geometry = Geometry.Parse(
                                                "F1 M280,280z M0,0z M126.33,97.06C131.69,106.34 135.74,116.37 141.73,125.3 134.45,139.76 126.78,154.01 119.27,168.35 117.13,172.5 114.7,176.49 112.16,180.41 106.5,171.85 102.97,162.07 97.12,153.63 103.19,141.31 109.81,129.28 115.88,116.96 119.23,110.26 123.54,104.05 126.33,97.06z")
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new TextBlock
                {
                    Name = "TitleText",
                    Foreground = new SolidColorBrush(Color.Parse("#9dd7f6")),
                    Text = "irel",
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(-1, 0, 0, 0)
                }
            }
        };
        return rootDockPanel;
    }
}