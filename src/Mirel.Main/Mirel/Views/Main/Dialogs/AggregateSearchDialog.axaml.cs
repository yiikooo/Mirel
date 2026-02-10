using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Mirel.Classes.Entries;
using Mirel.Module.Service;
using Ursa.Controls;

namespace Mirel.Views.Main.Dialogs;

public sealed partial class AggregateSearchDialog : UserControl, INotifyPropertyChanged
{
    public AggregateSearchDialog()
    {
        InitializeComponent();
        Width = 870;
        Height = 550;
        DataContext = this;
        Init();
    }

    public ObservableCollection<AggregateSearchEntry> FilteredItems { get; } = new();

    public string SearchFilter
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged();
            Filter();
        }
    } = "";

    public new event PropertyChangedEventHandler? PropertyChanged;

    private void Init()
    {
        // 鼠标移动时自动聚焦搜索框
        PointerMoved += (_, _) => { AggregateSearchBox.Focus(); };

        // 加载完成后聚焦搜索框
        Loaded += (_, _) => { AggregateSearchBox.Focus(); };

        // ESC键关闭对话框
        KeyDown += (_, e) =>
        {
            if (e.Key is not Key.Escape) return;
            CloseDialog();
        };

        // 关闭按钮点击事件
        CloseButton.Click += (_, _) => { CloseDialog(); };

        // 选择项变化处理
        AggregateSearchListBox.SelectionChanged += OnSelectionChanged;

        // 添加拖动功能到顶部区域
        TopDockPanel.PointerPressed += OnTopDockPanelPointerPressed;

        // 初始过滤
        Filter();
    }

    private void OnTopDockPanelPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var host = TopLevel.GetTopLevel(this);
        if (host is DialogWindow window)
        {
            window.BeginMoveDrag(e);
        }
    }

    private void CloseDialog()
    {
        var host = TopLevel.GetTopLevel(this);
        if (host is DialogWindow window) window.Close();
    }

    private void Filter()
    {
        try
        {
            FilteredItems.Clear();

            // 实时构建搜索项并过滤
            var searchResults = AggregateSearchService.Search(SearchFilter);

            foreach (var item in searchResults) FilteredItems.Add(item);
        }
        catch
        {
            // 忽略错误
        }
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (AggregateSearchListBox.SelectedItem is not AggregateSearchEntry entry) return;

        // 执行搜索项的操作
        var host = TopLevel.GetTopLevel(this);
        if (host is not DialogWindow window) return;

        AggregateSearchService.HandleSelection(entry, this);
        window.Close();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}