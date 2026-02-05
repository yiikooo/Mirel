using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Mirel.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Mirel.Classes.Entries;

public class OperateButtonEntry : ReactiveObject
{
    [Reactive] public object? Content { get; set; }
    [Reactive] public Action<object>? Action { get; set; }
    public bool OnUIThread { get; init; } = true;
    public bool CloseOnClick { get; init; } = true;

    public ICommand Command { get; }

    public OperateButtonEntry(string content, Action<object> action)
    {
        Content = content;
        Action = action;
        Command = new SimpleCommand(ExecuteCommand);
    }

    public void ExecuteCommand(object parameter)
    {
        if (OnUIThread)
            Dispatcher.UIThread.Invoke(() => 
            {
                Action?.Invoke(parameter);
                if (CloseOnClick)
                {
                    CloseNotification(parameter);
                }
            });
        else
        {
            Action?.Invoke(parameter);
            if (CloseOnClick)
            {
                CloseNotification(parameter);
            }
        }
    }

    private void CloseNotification(object parameter)
    {
        // 尝试从参数中获取按钮实例
        if (parameter is Avalonia.Controls.Button button)
        {
            // 通过 LogicalTree 查找按钮所在的 MirelMessageCard 实例
            var messageCard = button.GetLogicalAncestors().OfType<MirelMessageCard>().FirstOrDefault();
            messageCard?.Close();
        }
    }

    private class SimpleCommand(Action<object> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            execute(parameter ?? new object());
        }
    }
}