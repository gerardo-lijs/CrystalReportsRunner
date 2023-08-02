# Crystal Reports Runner

Runner to allow the use of Crystal Reports in .NET Core using external process (in .NET Framework 4.8) and named pipes for communication.

If you are using Crystal Reports in your application you're probably stuck with .NET Framework 4.x. However, all the new features are in the .NET Core framework nowadays and you might want to take advantage of them by upgrading your app to use the latest version of .NET.

Unfortunately, Crystal Reports doesn't support .NET Core so one workaround is to isolate it into its own executable so that your own application doesn't need to have a dependency on Crystal Reports SDK. 

## Quick Start 

1. Create a new Console Application and reference one of these NuGet packages depending the Crystal Reports runtime version you're using:
   - Crystal Reports v13.0.34 x64: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.34.x64.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.34.x64)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.34.x64)
   - Crystal Reports v13.0.34 x86: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.34.x86.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.34.x86)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.34.x86)
   - Crystal Reports v13.0.33 x64: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.33.x64.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.33.x64)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.33.x64)
   - Crystal Reports v13.0.33 x86: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.33.x86.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.33.x86)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.33.x86)
   - Crystal Reports v13.0.32 x64: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.32.x64.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.32.x64)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.32.x64)
   - Crystal Reports v13.0.32 x86: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.32.x86.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.32.x86)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.32.x86)
   - Crystal Reports v13.0.20 x64: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.20.x64.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.20.x64)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.20.x64)
   - Crystal Reports v13.0.20 x86: [![NuGet version](https://img.shields.io/nuget/v/LijsDev.CrystalReportsRunner.13.0.20.x86.svg?style=flat&label=nuget%3A%20LijsDev.CrystalReportsRunner.13.0.20.x86)](https://www.nuget.org/packages/LijsDev.CrystalReportsRunner.13.0.20.x86)
   - You need a version that is not listed here? Please refer to [Creating a custom Runner](./docs/custom-runner.md) or contact us.
   
   - You can download SAP Crystal Reports runtime engine for .NET Framwork from [SAP download website](https://origin.softwaredownloads.sap.com/public/site/index.html)

2. Create an engine:

   ```csharp
   using LijsDev.CrystalReportsRunner.Core;
   
   using var engine = new CrystalReportsEngine();
   ```

3. Optionally customizing viewer settings:

   ```csharp
   engine.ViewerSettings.AllowedExportFormats =
       ReportViewerExportFormats.PdfFormat |
       ReportViewerExportFormats.ExcelFormat;
   
   engine.ViewerSettings.ShowRefreshButton = false;
   engine.ViewerSettings.ShowCopyButton = false;
   engine.ViewerSettings.ShowGroupTreeButton = false;
   
   engine.ViewerSettings.SetUICulture(Thread.CurrentThread.CurrentUICulture);
   ```

4. Show the report and provide a connection string:

   ```csharp
   var report = new Report("SampleReport.rpt", "Sample Report")
   {
       Connection = CrystalReportsConnectionFactory.CreateSqlConnection(
           ".\\SQLEXPRESS", 
           "CrystalReportsSample")
   };
   
   report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
   report.Parameters.Add("UserName", "Gerardo");
   
   await engine.ShowReportDialog(report);
   ```

## Samples

Samples are available in [this repo](https://github.com/gerardo-lijs/CrystalReportsRunner.Samples).

## Guides

- [Creating a custom Runner](./docs/custom-runner.md)
- [How this library works](./docs/how-this-library-works.md)

## Thanks

- This library heavily depends on [PipeMethodCalls](https://github.com/RandomEngy/PipeMethodCalls) for Named Pipe communication.
- The signing certificate for the runners is provided by [Microptic S.L.](https://www.micropticsl.com/)
