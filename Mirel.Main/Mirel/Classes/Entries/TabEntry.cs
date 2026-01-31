using System.Linq;
using Mirel.Views.Main.Pages;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using Mirel.Classes.Interfaces;
using Mirel.ViewModels;
using Mirel.Views.Main;
using NewTabPage = Mirel.Views.Main.Pages.NewTabPage;

namespace Mirel.Classes.Entries;

public partial class TabEntry : ViewModelBase
{
    private IMirelTabPage _content;
    private object _headerContent;
    private StreamGeometry? _icon;

    private int _minWidth = -1;
    private string _tag;
    private string _title;

    public TabEntry(IMirelTabPage content, string? title = null, object? headerContent = null, int minWidth = -1)
    {
        CanClose = content.PageInfo.CanClose;
        Title = title ?? content.PageInfo.Title;
        MinWidth = minWidth > 0 ? minWidth : 140;
        Icon = content.PageInfo.Icon;
        HeaderContent = headerContent;
        Content = content;
        content.HostTab = this;
        content.PageInfo.PropertyChanged += (_, _) =>
        {
            Title = content.PageInfo.Title;
            Icon = content.PageInfo.Icon;
        };
    }

    public bool IconIsVisible => Icon != null;

    public IMirelTabPage Content
    {
        get => _content;
        set => SetField(ref _content, value);
    }

    public StreamGeometry? Icon
    {
        get => _icon;
        set => SetField(ref _icon, value);
    }

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public int MinWidth
    {
        get => _minWidth;
        set => SetField(ref _minWidth, value);
    }
    
    public bool IsUseHeaderContent => HeaderContent != null;

    public string Tag
    {
        get => _tag;
        set => SetField(ref _tag, value);
    }

    public bool CanClose
    {
        get;
        set => SetField(ref field, value);
    }

    public object HeaderContent
    {
        get => _headerContent;
        set => SetField(ref _headerContent, value);
    }


    public void RefreshContent()
    {
        // Force refresh of content to avoid layout manager conflicts
        if (Content?.RootElement != null)
        {
            Content.RootElement.InvalidateVisual();
            Content.RootElement.InvalidateMeasure();
        }
    }

    public async void Close(IRenderRoot? root = null)
    {
        if (!CanClose) return;
        var renderRoot = root ?? (Content as UserControl)!.GetVisualRoot();
        if (renderRoot is TabWindow tabWindow)
        {
            if (Content is IMirelRequestableClosePage requestableClosePage)
            {
                var close = await requestableClosePage.RequestClose(tabWindow);
                if (!close) return;
            }

            var wasSelected = tabWindow.ViewModel.SelectedTab == this;
            tabWindow.ViewModel.Tabs.Remove(this);
            if (wasSelected) tabWindow.ViewModel.SelectedTab = tabWindow.ViewModel.Tabs.LastOrDefault();
        }
        else
        {
            if (Content is IMirelRequestableClosePage requestableClosePage)
            {
                var close = await requestableClosePage.RequestClose(App.UiRoot);
                if (!close) return;
            }

            var wasSelected = App.UiRoot.ViewModel.SelectedTab == this;
            App.UiRoot.ViewModel.Tabs.Remove(this);
            if (wasSelected) App.UiRoot.ViewModel.SelectedTab = App.UiRoot.ViewModel.Tabs.LastOrDefault();
        }

        DisposeContent();
        Removing();

        if (renderRoot is TabWindow tabWindow1)
        {
            if (tabWindow1.ViewModel.Tabs.Count > 0) return;
            tabWindow1.CreateTab(new TabEntry(new NewTabPage()));
        }
        else
        {
            if (App.UiRoot.ViewModel.Tabs.Count > 0) return;
            App.UiRoot.CreateTab(new TabEntry(new NewTabPage()));
        }
    }

    public void ReplacePage(IMirelTabPage tabPage)
    {
        DisposeContent();
        Content = tabPage;
        tabPage.HostTab = this;
        Icon = tabPage.PageInfo.Icon;
        Title = tabPage.PageInfo.Title;
        Content = tabPage;
    }

    public void Removing()
    {
        DisposeContent();
        Content = null;
    }

    public void DisposeContent()
    {
        Content?.OnClose();
    }

    [RelayCommand]
    public void CloseInWindow(Window window)
    {
        if (!CanClose) return;

        if (window is MainWindow mainWindow)
        {
            Close(); // Use existing Close method for MainWindow
        }
        else if (window is TabWindow tabWindow)
        {
            // Remove from TabWindow - RemoveTab method already handles selection logic
            tabWindow.ViewModel.RemoveTab(this);

            // If TabWindow has no more tabs, close it
            if (!tabWindow.ViewModel.HasTabs) tabWindow.Close();

            DisposeContent();
            Removing();
        }
    }
}