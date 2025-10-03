namespace LijsDev.CrystalReportsRunner.Core;

using System.Data;

/// <summary>
/// Used to communicate back to the main Process, which callback to execute.
/// </summary>
public interface ICallbackDispatcher
{
    /// <summary>
    /// Executes the callback associated with the provided GUID.
    /// Returns <langword>true</langword> if the callback exists in the Registry and it was executed.
    /// </summary>
    /// <param name="dataTable"></param>
    /// <param name="guid"></param>
    bool TryInvokeCallbackFromGuid(DataTable dataTable, Guid guid);
}
