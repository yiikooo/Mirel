using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Mirel.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;


namespace Mirel.Classes.Entries;

public class OperateButtonEntry : ReactiveObject
{
    [Reactive] public object? Content { get; set; }
    [Reactive] public Action<object>? Action { get; set; }
    public bool OnUIThread { get; init; } = true;
    public bool CloseOnClick { get; init; } = false;

    public ICommand Command { get; }

    public OperateButtonEntry(string content, Action<object> action, bool closeOnClick = false)
    {
        Content = content;
        Action = action;
        CloseOnClick = closeOnClick;
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
            // 通过 VisualTree 查找按钮所在的 MirelMessageCard 实例
            var messageCard = button.GetVisualAncestors().OfType<MirelMessageCard>().FirstOrDefault();
            messageCard?.Close();
        }
    }

    private class SimpleCommand : ICommand
    {
        private readonly Action<object> _execute;

        public SimpleCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter ?? new object());
        }
    }
}