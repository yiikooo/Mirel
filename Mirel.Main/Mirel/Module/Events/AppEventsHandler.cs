using System;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Mirel.Module.Events;

public class AppEventsHandler
{
    public delegate void AppDragDropHandler(object? sender, DragEventArgs e);
    public delegate void OnlySenderHandler(object? sender);
    public delegate Task<bool> AppExitingHandler();
}

public class AppExitingEventArgs : EventArgs
{ 
    public bool Cancel { get; set; }
}