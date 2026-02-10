using System.Threading.Tasks;

namespace Mirel.Classes.Interfaces;

public interface IMirelRequestableClosePage
{
    Task<bool> RequestClose(object? sender);
}