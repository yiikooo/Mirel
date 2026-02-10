using Avalonia.Input;
using Mirel.Classes.Interfaces;
using Mirel.Module.Event;

namespace Mirel.Module.Service;

public class DataDragDropService
{
    public static void HandleData(IMirelWindow sender, DragEventArgs e)
    {
        AppEvents.OnAppDragDrop(sender, e);
    }
}