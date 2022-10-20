namespace LijsDev.CrystalReportsRunner;

using System;

using LijsDev.CrystalReportsRunner.Core;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var shell = new Shell(new ReportViewer());
        shell.StartListening(args);
    }
}
