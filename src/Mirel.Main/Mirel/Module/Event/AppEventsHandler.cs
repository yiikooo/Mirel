using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Mirel.Classes.Entries;

namespace Mirel.Module.Event;

public class AppEventsHandler
{
    public delegate void AppDragDropHandler(object? sender, DragEventArgs e);

    public delegate Task<bool> AppExitingHandler();

    public delegate void OnlySenderHandler(object? sender);

    public delegate void TabSelectionChangedHandler(TabSEntry e);
}

public class AppExitingEventArgs : EventArgs
{
    public bool Cancel { get; set; }
}