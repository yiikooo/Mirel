using System;
using Mirel.Views;

namespace Mirel.Module.Service;

public class ExceptionService
{
    public static void HandleException(Exception e)
    {
        var win = new CrashWindow(e);
        win.Show();
    }
}