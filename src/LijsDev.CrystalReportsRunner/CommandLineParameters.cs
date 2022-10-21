#if DEBUG

namespace LijsDev.CrystalReportsRunner;

internal sealed class CommandLineParameters
{
    /// <summary>
    /// Allows to run Crystal Reports Runner in Debug Test mode.
    /// </summary>
    public bool DebugTest { get; private set; }

    public static CommandLineParameters Parse(string[] args)
    {
        var parseParameters = new CommandLineParameters();

        for (var i = 0; i < args.Length; i++)
        {
            var param = args[i];

            if (param.Equals("--DebugTest", StringComparison.OrdinalIgnoreCase))
            {
                parseParameters.DebugTest = true;
            }
        }

        return parseParameters;
    }
}

#endif
