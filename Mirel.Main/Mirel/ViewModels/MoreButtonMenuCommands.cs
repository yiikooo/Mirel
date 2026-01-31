using System.Linq;
using Avalonia;
using Avalonia.Styling;
using Mirel.Classes.Entries;
using Mirel.Classes.Enums;
using Mirel.Const;
using Mirel.Module.Service;
using Mirel.Views.Main;
using Mirel.Views.Main.Pages;
using Ursa.Controls;

namespace Mirel.ViewModels;

public class MoreButtonMenuCommands
{
    public void NewTab()
    {
        if (UiProperty.ActiveWindow is TabWindow tabWindow)
        {
            tabWindow.CreateTab(new TabEntry(new NewTabPage()));
            return;
        }

        App.UiRoot.CreateTab(new TabEntry(new NewTabPage()));
    }

    public void CloseCurrentTab()
    {
        if (UiProperty.ActiveWindow is TabWindow tabWindow)
        {
            tabWindow.ViewModel.SelectedTab?.Close();
            return;
        }

        App.UiRoot.ViewModel.SelectedTab?.Close();
    }

    public void ToggleTheme(string theme = "auto")
    {
        Data.SettingEntry.Theme = theme switch
        {
            "mirage" => Setting.Theme.Mirage,
            "light" => Setting.Theme.Light,
            "dark" => Setting.Theme.Dark,
            _ => Application.Current.ActualThemeVariant == ThemeVariant.Dark
                ? Setting.Theme.Light
                : Setting.Theme.Dark
        };
    }

    public void DebugTab()
    {
        // App.UiRoot.TogglePage("debug", App.UiRoot.ViewModel.DebugTabPage); //TODO
    }

    public void MoveToNewWindow()
    {
        if (UiProperty.ActiveWindow is TabWindow tabWindow)
        {
            tabWindow.ViewModel.SelectedTab?.MoveTabToNewWindow();
            return;
        }

        App.UiRoot.ViewModel.SelectedTab?.MoveTabToNewWindow();
    }

    public void OpenInstancePage(string page)
    {
        switch (page)
        {
            case "setting":
                App.UiRoot.TogglePage("setting", new SettingTabPage());
                break;
        }
    }
}