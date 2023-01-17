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

    // 以下の通りEnumChildWindowsを利用することで子は取得できる
    // ただし、再帰的に処理できないため、階層構造を確認するためには利用できなさそう
    // http://studio-jpn.com/win32-api/
    var childWindows = new List<IntPtr>();
    EnumChildWindows(handle, new EnumWindowsDelegate(EnumWindowCallBack), IntPtr.Zero);
    foreach (var childWindow in childWindows)
    {
        Console.WriteLine(childWindow);
    }

    bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
    {
        childWindows.Add(hWnd);
        return true;
    }
}

[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

[DllImport("user32.dll")]
[return: MarshalAs(UnmanagedType.Bool)]
static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsDelegate enumProc, IntPtr lParam);

public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

