// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

Console.WriteLine("ProcessName\tProcessHandle\tClassName");
foreach (var process in Process.GetProcesses())
{
    // MainWindowHandleが0の場合、ウインドウを持たないためスキップ
    // https://learn.microsoft.com/ja-jp/dotnet/api/system.diagnostics.process.mainwindowhandle?view=net-7.0#system-diagnostics-process-mainwindowhandle
    var handle = process.MainWindowHandle;
    if (handle == IntPtr.Zero) continue;

    // WindowsAPIを利用してクラス名を取得
    // https://stackoverflow.com/questions/12372534/how-to-get-a-process-window-class-name-from-c
    // StringBuilderの値を超えた値をGetClassNameの引数に入れてしまうとエラーになるためちゃんと指定する
    var stringBuilder = new StringBuilder(256);
    var result = GetClassName(handle, stringBuilder, stringBuilder.Capacity);

    // 0を返された場合は失敗のためスキップ
    // https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-getclassname
    if (result == 0) continue;

    Console.WriteLine($"{process.ProcessName}\t{handle}\t{stringBuilder}");
}

[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

