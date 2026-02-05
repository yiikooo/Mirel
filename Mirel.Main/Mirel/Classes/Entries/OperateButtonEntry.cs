using System;
using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Mirel.Classes.Entries;

public class OperateButtonEntry : ReactiveObject
{
    [Reactive] public object? Content { get; set; }
    [Reactive] public Action<object>? Action { get; set; }
    public bool OnUIThread { get; init; } = true;

    public ICommand Command { get; }


    public OperateButtonEntry(string content, Action<object> action)
    {
        Content = content;
        Action = action;
        Command = ReactiveCommand.Create<object>(ExecuteCommand);
    }

    public void ExecuteCommand(object parameter)
    {
        if (OnUIThread)
            Dispatcher.UIThread.Post(() => { Action?.Invoke(parameter); });
        else
            Action?.Invoke(parameter);
    }
}