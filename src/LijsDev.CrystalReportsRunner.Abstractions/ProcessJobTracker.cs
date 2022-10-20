// https://gist.github.com/AArnott/2609636d2f2369495abe76e8a01446a4
// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the MIT license.

namespace LijsDev.CrystalReportsRunner.Abstractions;

/* This is a derivative from multiple answers on https://stackoverflow.com/questions/3342941/kill-child-process-when-parent-process-is-killed */

using System;
using System.ComponentModel;
using System.Diagnostics;

using Microsoft.Win32.SafeHandles;

using Windows.Win32.System.JobObjects;

using static Windows.Win32.PInvoke;

/// <summary>
/// Allows processes to be automatically killed if this parent process unexpectedly quits
/// (or when an instance of this class is disposed).
/// </summary>
/// <remarks>
/// This "just works" on Windows 8.
/// To support Windows Vista or Windows 7 requires an app.manifest with specific content
/// <see href="https://stackoverflow.com/a/9507862/46926">as described here</see>.
/// </remarks>
public class ProcessJobTracker : IDisposable
{
    private readonly object _disposeLock = new();
    private bool _disposed;

    /// <summary>
    /// The job handle.
    /// </summary>
    /// <remarks>
    /// Closing this handle would close all tracked processes. So we don't do it in this process
    /// so that it happens automatically when our process exits.
    /// </remarks>
    private readonly SafeFileHandle? _jobHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessJobTracker"/> class.
    /// </summary>
    public unsafe ProcessJobTracker()
    {
#if NET5_0_OR_GREATER
        if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
#else
        if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version > new Version(5, 1, 2600))
#endif
        {
            // The job name is optional (and can be null) but it helps with diagnostics.
            //  If it's not null, it has to be unique. Use SysInternals' Handle command-line
            //  utility: handle -a ChildProcessTracker
            var jobName = nameof(ProcessJobTracker) + Process.GetCurrentProcess().Id;
            _jobHandle = CreateJobObject(null, jobName);

            var extendedInfo = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = new JOBOBJECT_BASIC_LIMIT_INFORMATION
                {
                    LimitFlags = JOB_OBJECT_LIMIT.JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE,
                },
            };

            if (!SetInformationJobObject(_jobHandle, JOBOBJECTINFOCLASS.JobObjectExtendedLimitInformation, &extendedInfo, (uint)sizeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION)))
            {
                throw new Win32Exception();
            }
        }
    }

    /// <summary>
    /// Ensures a given process is killed when the current process exits.
    /// </summary>
    /// <param name="process">The process whose lifetime should never exceed the lifetime of the current process.</param>
    public void AddProcess(Process process)
    {
        if (process is null)
        {
            throw new ArgumentNullException(nameof(process));
        }

#if NET5_0_OR_GREATER
        if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
#else
        if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version > new Version(5, 1, 2600))
#endif
        {
            bool success = AssignProcessToJobObject(_jobHandle, new SafeFileHandle(process.Handle, ownsHandle: false));
            if (!success && !process.HasExited)
            {
                throw new Win32Exception();
            }
        }
    }

    /// <summary>
    /// Kills all processes previously tracked with <see cref="AddProcess(Process)"/> by closing the Windows Job.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    _jobHandle?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
