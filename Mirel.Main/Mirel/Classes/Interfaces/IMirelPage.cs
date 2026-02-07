using Avalonia.Controls;

namespace Mirel.Classes.Interfaces;

public interface IMirelPage
{
    public Control RootElement { get; init; }
}