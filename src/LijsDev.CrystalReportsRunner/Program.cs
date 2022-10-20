namespace LijsDev.CrystalReportsRunner;

using System;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var shell = new Shell.Shell(new ReportViewer());
        shell.StartListening(args);
    }
}
