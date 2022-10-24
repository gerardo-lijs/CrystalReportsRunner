namespace LijsDev.CrystalReportsRunner;

using System.IO;
using System.IO.MemoryMappedFiles;

/// <summary>
/// Helper methods for memory-mapped files.
/// </summary>
internal static class MemoryMappedFileUtils
{
    public static MemoryMappedFile CreateFromStream(string mapName, Stream stream)
    {
        // Create memory mapped file
        var mmf = MemoryMappedFile.CreateNew(mapName, stream.Length);

        // Write to mmf
        try
        {
            using var mmfStream = mmf.CreateViewStream();
            stream.CopyTo(mmfStream);
        }
        catch
        {
            // In case of exceptions let's dispose the created mmf
            mmf.Dispose();
            throw;
        }

        return mmf;
    }
}
