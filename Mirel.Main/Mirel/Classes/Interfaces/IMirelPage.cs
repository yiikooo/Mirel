using Avalonia.Controls;
using Mirel.Module.Ui.Helper;

namespace Mirel.Classes.Interfaces;

public interface IMirelPage
{
    public Control RootElement { get; init; }
}