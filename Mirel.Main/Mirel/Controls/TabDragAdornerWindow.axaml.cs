using Avalonia;
using Avalonia.Controls;
using Mirel.Classes.Entries;

namespace Mirel.Controls;

public partial class TabDragAdornerWindow : Window
{
    public TabDragAdornerWindow()
    {
        InitializeComponent();
    }

    public TabDragAdornerWindow(TabEntry tabEntry) : this()
    {
        DataContext = tabEntry;
    }

    public void UpdatePosition(PixelPoint screenPosition)
    {
        // Offset the window slightly from the cursor
        Position = new PixelPoint(screenPosition.X + 10, screenPosition.Y - 15);
    }
}