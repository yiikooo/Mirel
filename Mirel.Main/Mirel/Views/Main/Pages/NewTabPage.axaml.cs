using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Mirel.Classes.Entries;
using Mirel.Classes.Interfaces;
using Mirel.Module.IO.Local;
using Mirel.Module.Service;
using Mirel.Module.Ui.Helper;
using Mirel.ViewModels;
using Ursa.Controls;

namespace Mirel.Views.Main.Pages;

public partial class NewTabPage : PageMixModelBase, IMirelTabPage, IMirelNavPage
{
    // public DateTime _currentTime = DateTime.Now;

    // public string _currentLunarMonthDay = string.Empty;
    // public string _currentLunarYear = string.Empty;
    // public string _currentWeekDay = string.Empty;
    // public string _poem = "loading";
    private string _shortInfo = string.Empty;

    public string ShortInfo
    {
        get => _shortInfo;
        set => SetField(ref _shortInfo, value);
    }

    public Control BottomElement { get; set; }
    // public DateTime CurrentTime
    // {
    //     get => _currentTime;
    //     set
    //     {
    //         SetField(ref _currentTime, value);
    //         // UpdateLunarDate(CurrentTime);
    //         // UpdateWeekDay(CurrentTime);
    //     }
    // }

    //
    // public string CurrentLunarMonthDay
    // {
    //     get => _currentLunarMonthDay;
    //     set => SetField(ref _currentLunarMonthDay, value);
    // }
    //
    // public string Poem
    // {
    //     get => _poem;
    //     set => SetField(ref _poem, value);
    // }
    //
    // public string CurrentLunarYear
    // {
    //     get => _currentLunarYear;
    //     set => SetField(ref _currentLunarYear, value);
    // }
    //
    // public string CurrentWeekDay
    // {
    //     get => _currentWeekDay;
    //     set => SetField(ref _currentWeekDay, value);
    // }
    //
    private bool _fl = true;
    // private bool _suppressNextFilter = false;
    //
    // // File system search properties
    // [Reactive]
    // public StreamGeometry SearchIconData { get; set; } = StreamGeometry.Parse(
    //     "M416 208c0 45.9-14.9 88.3-40 122.7L502.6 457.4c12.5 12.5 12.5 32.8 0 45.3s-32.8 12.5-45.3 0L330.7 376c-34.4 25.2-76.8 40-122.7 40C93.1 416 0 322.9 0 208S93.1 0 208 0S416 93.1 416 208zM208 352a144 144 0 1 0 0-288 144 144 0 1 0 0 288z");
    //
    // [Reactive] public bool IsFileSystemMode { get; set; }
    // [Reactive] public string CurrentFileSystemPath { get; set; } = string.Empty;

    public NewTabPage()
    {
        InitializeComponent();
        RootElement = Root;
        InAnimator = new PageLoadingAnimator(Root, new Thickness(0, 60, 0, 0), (0, 1));
        PageInfo = new PageInfoEntry
        {
            Title = "新标签页",
            Icon = StreamGeometry.Parse(
                "F1 M 12.670898 5.825195 L 15 6.796875 L 15.97168 9.125977 C 16.025391 9.228516 16.132812 9.296875 16.25 9.296875 C 16.367188 9.296875 16.474609 9.228516 16.52832 9.125977 L 17.5 6.796875 L 19.829102 5.825195 C 19.931641 5.771484 20 5.664062 20 5.546875 C 20 5.429688 19.931641 5.322266 19.829102 5.268555 L 17.5 4.296875 L 16.52832 1.967773 C 16.474609 1.865234 16.367188 1.796875 16.25 1.796875 C 16.132812 1.796875 16.025391 1.865234 15.97168 1.967773 L 15 4.296875 L 12.670898 5.268555 C 12.568359 5.322266 12.5 5.429688 12.5 5.546875 C 12.5 5.664062 12.568359 5.771484 12.670898 5.825195 Z M 19.829102 17.768555 L 17.5 16.796875 L 16.52832 14.467773 C 16.474609 14.365234 16.367188 14.296875 16.25 14.296875 C 16.132812 14.296875 16.025391 14.365234 15.97168 14.467773 L 15 16.796875 L 12.670898 17.768555 C 12.568359 17.822266 12.5 17.929688 12.5 18.046875 C 12.5 18.164062 12.568359 18.271484 12.670898 18.325195 L 15 19.296875 L 15.97168 21.625977 C 16.025391 21.728516 16.132812 21.796875 16.25 21.796875 C 16.367188 21.796875 16.474609 21.728516 16.52832 21.625977 L 17.5 19.296875 L 19.829102 18.325195 C 19.931641 18.271484 20 18.164062 20 18.046875 C 20 17.929688 19.931641 17.822266 19.829102 17.768555 Z M 15 11.782227 C 15 11.547852 14.868164 11.328125 14.65332 11.220703 L 10.258789 9.018555 L 8.056641 4.614258 C 7.84668 4.189453 7.15332 4.189453 6.943359 4.614258 L 4.741211 9.018555 L 0.34668 11.220703 C 0.131836 11.328125 0 11.547852 0 11.782227 C 0 12.021484 0.131836 12.236328 0.34668 12.34375 L 4.741211 14.545898 L 6.943359 18.950195 C 7.045898 19.160156 7.265587 19.296875 7.5 19.296875 C 7.734337 19.296875 7.954102 19.160156 8.056641 18.950195 L 10.258789 14.545898 L 14.65332 12.34375 C 14.868164 12.236328 15 12.021484 15 11.782227 Z ")
        };
        ShortInfo = "新标签页";
        DataContext = this;
        Loaded += (_, _) =>
        {
            if (!_fl) return;
            _fl = false;
        };
    }

    public string SearchFilter
    {
        get;
        set
        {
            SetField(ref field, value);
            // Filter();
        }
    } = "";
    

    public Control RootElement { get; set; }
    public PageLoadingAnimator InAnimator { get; set; }
    public TabEntry HostTab { get; set; }
    public PageInfoEntry PageInfo { get; }

    public void OnClose()
    {
    }

    public void OpenNewPageCommand()
    {
        if (this.GetVisualRoot() is TabWindow tabWindow)
        {
            tabWindow.CreateTab(new TabEntry(new NewTabPage()));
            return;
        }

        App.UiRoot.CreateTab(new TabEntry(new NewTabPage()));
        HostTab.Close();
    }

    public void AggSearchCommand()
    {
        var options = new DialogOptions()
        {
            ShowInTaskBar = false,
            IsCloseButtonVisible = true,
            StartupLocation = WindowStartupLocation.Manual,
            CanDragMove = true,
            StyleClass = "aggregate-search"
        };
        // Dialog.ShowCustom<AggregateSearchDialog, AggregateSearchDialog>(new AggregateSearchDialog(),
        //     this.GetVisualRoot() as Window, options: options); //TODO
    }
    
    public void OpenInNewWindowCommand()
    {
        HostTab.MoveTabToNewWindow();
    }


    public static MirelStaticPageInfo StaticPageInfo { get; } = new()
    {
        Icon = StreamGeometry.Parse(
            "F1 M 12.670898 5.825195 L 15 6.796875 L 15.97168 9.125977 C 16.025391 9.228516 16.132812 9.296875 16.25 9.296875 C 16.367188 9.296875 16.474609 9.228516 16.52832 9.125977 L 17.5 6.796875 L 19.829102 5.825195 C 19.931641 5.771484 20 5.664062 20 5.546875 C 20 5.429688 19.931641 5.322266 19.829102 5.268555 L 17.5 4.296875 L 16.52832 1.967773 C 16.474609 1.865234 16.367188 1.796875 16.25 1.796875 C 16.132812 1.796875 16.025391 1.865234 15.97168 1.967773 L 15 4.296875 L 12.670898 5.268555 C 12.568359 5.322266 12.5 5.429688 12.5 5.546875 C 12.5 5.664062 12.568359 5.771484 12.670898 5.825195 Z M 19.829102 17.768555 L 17.5 16.796875 L 16.52832 14.467773 C 16.474609 14.365234 16.367188 14.296875 16.25 14.296875 C 16.132812 14.296875 16.025391 14.365234 15.97168 14.467773 L 15 16.796875 L 12.670898 17.768555 C 12.568359 17.822266 12.5 17.929688 12.5 18.046875 C 12.5 18.164062 12.568359 18.271484 12.670898 18.325195 L 15 19.296875 L 15.97168 21.625977 C 16.025391 21.728516 16.132812 21.796875 16.25 21.796875 C 16.367188 21.796875 16.474609 21.728516 16.52832 21.625977 L 17.5 19.296875 L 19.829102 18.325195 C 19.931641 18.271484 20 18.164062 20 18.046875 C 20 17.929688 19.931641 17.822266 19.829102 17.768555 Z M 15 11.782227 C 15 11.547852 14.868164 11.328125 14.65332 11.220703 L 10.258789 9.018555 L 8.056641 4.614258 C 7.84668 4.189453 7.15332 4.189453 6.943359 4.614258 L 4.741211 9.018555 L 0.34668 11.220703 C 0.131836 11.328125 0 11.547852 0 11.782227 C 0 12.021484 0.131836 12.236328 0.34668 12.34375 L 4.741211 14.545898 L 6.943359 18.950195 C 7.045898 19.160156 7.265587 19.296875 7.5 19.296875 C 7.734337 19.296875 7.954102 19.160156 8.056641 18.950195 L 10.258789 14.545898 L 14.65332 12.34375 C 14.868164 12.236328 15 12.021484 15 11.782227 Z"),
        Title = "新标签页",
        NeedPath = false,
        AutoCreate = true
    };

    public static IMirelNavPage Create((object sender, object? param) t)
    {
        var root = ((Control)t.sender).GetVisualRoot();
        if (root is TabWindow tabWindow)
        {
            tabWindow.CreateTab(new TabEntry(new NewTabPage()));
            return null;
        }

        App.UiRoot.CreateTab(new TabEntry(new NewTabPage()));
        return null;
    }
}