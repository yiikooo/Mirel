using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Module.Service;
using Ursa.Controls;

namespace Mirel.Views.Main.Dialogs;

public sealed partial class AggregateSearchDialog : UserControl, INotifyPropertyChanged
{
    private AggregateSearchType _currentSearchType = AggregateSearchType.All;
    private bool _isSearchTypeVisible;
    private string _searchFilter = "";

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
        get => _searchFilter;
        set
        {
            if (_searchFilter == value) return;
            _searchFilter = value;
            OnPropertyChanged();
            ParseSearchQuery();
            Filter();
        }
    }

    public AggregateSearchType CurrentSearchType
    {
        get => _currentSearchType;
        set
        {
            if (_currentSearchType == value) return;
            _currentSearchType = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SearchTypeLabel));
        }
    }

    public string SearchTypeLabel => AggregateSearchTypeInfo.GetDisplayName(CurrentSearchType);

    public bool IsSearchTypeVisible
    {
        get => _isSearchTypeVisible;
        set
        {
            if (_isSearchTypeVisible == value) return;
            _isSearchTypeVisible = value;
            OnPropertyChanged();
        }
    }

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

        // 处理删除键清除搜索类型
        AggregateSearchBox.KeyDown += OnSearchBoxKeyDown;

        // 关闭按钮点击事件
        CloseButton.Click += (_, _) => { CloseDialog(); };

        // 选择项变化处理
        AggregateSearchListBox.SelectionChanged += OnSelectionChanged;

        // 添加拖动功能到顶部区域
        TopDockPanel.PointerPressed += OnTopDockPanelPointerPressed;

        // 初始过滤
        Filter();
    }

    private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
    {
        // 当搜索框为空且按下删除键或退格键时，清除搜索类型
        if ((e.Key == Key.Back || e.Key == Key.Delete) &&
            string.IsNullOrEmpty(GetActualSearchQuery()) &&
            IsSearchTypeVisible)
        {
            CurrentSearchType = AggregateSearchType.All;
            IsSearchTypeVisible = false;
            SearchFilter = "";
            e.Handled = true;
        }
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

    private void ParseSearchQuery()
    {
        var query = SearchFilter.Trim();

        if (query.StartsWith('#'))
        {
            var parts = query[1..].Split([' '], 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                var keyword = parts[0];
                var searchType = AggregateSearchTypeInfo.FindTypeByKeyword(keyword);

                if (searchType.HasValue)
                {
                    CurrentSearchType = searchType.Value;
                    IsSearchTypeVisible = true;
                    return;
                }
            }
        }

        CurrentSearchType = AggregateSearchType.All;
        IsSearchTypeVisible = false;
    }

    private string GetActualSearchQuery()
    {
        var query = SearchFilter.Trim();

        if (!query.StartsWith('#')) return query;
        var parts = query[1..].Split([' '], 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : "";
    }

    private void Filter()
    {
        try
        {
            FilteredItems.Clear();

            var actualQuery = GetActualSearchQuery();

            // 实时构建搜索项并过滤
            var searchResults = AggregateSearchService.Search(actualQuery, CurrentSearchType);

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