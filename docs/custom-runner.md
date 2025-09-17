# Creating your own Runner

Maybe you need a version of the Crystal Reports runtime that we don't support or maybe you want to have full control over the runner. In any case, it's easy to create your own runner that uses whatever version of the Crystal Reports runtime you need and also customize the viewer form however you wish.

1. Create a new console application that uses .NET 4.8 with these properties:

    ```xml
    <PropertyGroup>
        <OutputType>WinExe</OutputType>
        <TargetFramework>net48</TargetFramework>
        <Platforms>x86</Platforms>
        <UseWindowsForms>true</UseWindowsForms>
        <LangVersion>latest</LangVersion>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="LijsDev.CrystalReportsRunner.Shell" Version="1.0.0" />
    </ItemGroup>

    <ItemGroup>
        <!-- Add a reference to the Crystal Reports SDK. -->
        <!-- Note: The paths depend on your codebase and runtime versions. -->
        <Reference Include="CrystalDecisions.CrystalReports.Engine">
          <HintPath>..\..\dependencies\13.0.20\CrystalDecisions.CrystalReports.Engine.dll</HintPath>
        </Reference>
        <Reference Include="CrystalDecisions.ReportSource">
          <HintPath>..\..\dependencies\13.0.20\CrystalDecisions.ReportSource.dll</HintPath>
        </Reference>
        <Reference Include="CrystalDecisions.Shared">
          <HintPath>..\..\dependencies\13.0.20\CrystalDecisions.Shared.dll</HintPath>
        </Reference>
        <Reference Include="CrystalDecisions.Windows.Forms">
          <HintPath>..\..\dependencies\13.0.20\CrystalDecisions.Windows.Forms.dll</HintPath>
        </Reference>
    </ItemGroup>
    ```

2. Add the following code to `Program.cs`:

    ```csharp
    namespace SampleRunner;

    using System;
    using LijsDev.CrystalReportsRunner.Core;

    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var shell = new Shell.Shell(new ReportViewer(), new ReportExporter());
            shell.StartListening(args);
        }
    }
    ```

3. Create an implementation of `IReportViewer`:

    ```csharp
    namespace SampleRunner;
    
    using System.Windows.Forms;
    
    using LijsDev.CrystalReportsRunner.Core;
    using LijsDev.CrystalReportsRunner.Shell;
    
    internal class ReportViewer : IReportViewer
    {
        public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings)
        {
            var document = ReportUtils.CreateReportDocument(report);
            // Note: You nee to create a Form that shows the report using a CrystalReportViewer
            return new ViewerForm(document, viewerSettings)
            {
                Text = report.Title
            };
        }
    }
    ```

    Reference Implementations:

    - [`ViewerForm`](../src/LijsDev.CrystalReportsRunner/shared/ViewerForm.cs)

4. And an implementation of `IReportExporter`:

    ```csharp
    namespace SampleRunner;
    
    using CrystalDecisions.Shared;
    
    using LijsDev.CrystalReportsRunner.Core;
    using LijsDev.CrystalReportsRunner.Shell;
    
    internal class ReportExporter : IReportExporter
    {
        public void Export(
            Report report, 
            ReportExportFormats exportFormat, 
            string destinationFilename, 
            bool overwrite = true)
        {
            var document = ReportUtils.CreateReportDocument(report);
    
            // Overwrite
            if (overwrite && File.Exists(destinationFilename)) File.Delete(destinationFilename);
    
            // Export
            document.ExportToDisk((ExportFormatType)exportFormat, destinationFilename);
        }
    
        public string ExportToMemoryMappedFile(
            Report report, 
            ReportExportFormats exportFormat)
        {
            var document = ReportUtils.CreateReportDocument(report);
    
            // Export
            var reportStream = document.ExportToStream((ExportFormatType)exportFormat);
    
            // Create MemoryMappedFile from Stream
            var mmfName = $"CrystalReportsRunner_Export_{Guid.NewGuid()}";
            MemoryMappedFileUtils.CreateFromStream(mmfName, reportStream);
    
            return mmfName;
        }
    }
    ```
    
    Reference Implementations:
    
    - [`ReportUtils`](../src/LijsDev.CrystalReportsRunner/shared/ReportUtils.cs)
    - [`MemoryMappedFileUtils`](../src/LijsDev.CrystalReportsRunner/shared/MemoryMappedFileUtils.cs)

4. Make sure you copy the sample runner executable to `crystal-reports-runner\runner.exe` along with its dependencies in the output folder of the Caller assembly.
