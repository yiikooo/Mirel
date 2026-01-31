using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using Mirel.ViewModels;
using Mirel.Views.Main;

namespace Mirel.Module.App.Services;

public class BindingKeys
{
    public static void Main(Window window)
    {
        var c = new MoreButtonMenuCommands();
        window.KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Ctrl+T"),
            Command = new RelayCommand(c.NewTab)
        });
        window.KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Ctrl+W"),
            Command = new RelayCommand(c.CloseCurrentTab)
        });
        window.KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Ctrl+Shift+Q"),
            Command = new RelayCommand(() => c.ToggleTheme())
        });
        window.KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Shift+F12"),
            Command = new RelayCommand(c.DebugTab)
        });
        window.KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Alt+W"),
            Command = new RelayCommand(c.MoveToNewWindow)
        });
        if (window is not MainWindow) return;
        window.KeyBindings.Add(new KeyBinding
        {
            Gesture = KeyGesture.Parse("Ctrl+I"),
            Command = new RelayCommand(() => c.OpenInstancePage("setting"))
        });
    }
}