// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

Console.WriteLine("ProcessName\tProcessHandle");
foreach (var process in Process.GetProcesses())
{
    // MainWindowHandleが0の場合、ウインドウを持たないためスキップ
    // https://learn.microsoft.com/ja-jp/dotnet/api/system.diagnostics.process.mainwindowhandle?view=net-7.0#system-diagnostics-process-mainwindowhandle
    if (process.MainWindowHandle == IntPtr.Zero) continue;
    Console.WriteLine($"{process.ProcessName}\t{process.MainWindowHandle}");
}

