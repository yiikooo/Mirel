using Avalonia;
using Avalonia.Controls;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Const;
using ReactiveUI;

namespace Mirel.Controls;

public partial class TabDragAdornerWindow : Window
{
    private TabDragState _dragState = TabDragState.NoOperation;

    public TabDragAdornerWindow()
    {
        InitializeComponent();
    }

    public TabDragAdornerWindow(TabEntry tabEntry) : this()
    {
        DataContext = new TabDragAdornerViewModel(tabEntry);
    }

    public void UpdatePosition(PixelPoint screenPosition)
    {
        // Offset the window slightly from the cursor
        Position = new PixelPoint(screenPosition.X + 10, screenPosition.Y - 15);
    }

    public void UpdateDragState(TabDragState state)
    {
        _dragState = state;
        if (DataContext is TabDragAdornerViewModel viewModel)
        {
            viewModel.DragState = state;
        }
    }
}

/// <summary>
/// 标签页拖动装饰窗口的 ViewModel
/// </summary>
public class TabDragAdornerViewModel : ReactiveObject
{
    private TabDragState _dragState = TabDragState.NoOperation;

    public TabDragAdornerViewModel(TabEntry tabEntry)
    {
        TabEntry = tabEntry;
    }

    public TabEntry TabEntry { get; }

    public TabDragState DragState
    {
        get => _dragState;
        set
        {
            this.RaiseAndSetIfChanged(ref _dragState, value);
            // 当状态改变时，通知图标和可见性属性也改变
            this.RaisePropertyChanged(nameof(StateIcon));
            this.RaisePropertyChanged(nameof(ShowStateIcon));
        }
    }

    /// <summary>
    /// 获取当前拖动状态对应的图标路径
    /// </summary>
    public string StateIcon => DragState switch
    {
        TabDragState.ReorderInCurrentWindow => TabDragIcons.ReorderInCurrentWindow,
        TabDragState.TransferToAnotherWindow => TabDragIcons.TransferToAnotherWindow,
        TabDragState.DetachToNewWindow => TabDragIcons.DetachToNewWindow,
        _ => string.Empty
    };

    /// <summary>
    /// 是否显示状态图标（NoOperation 状态不显示）
    /// </summary>
    public bool ShowStateIcon => DragState != TabDragState.NoOperation && !string.IsNullOrEmpty(StateIcon);
}