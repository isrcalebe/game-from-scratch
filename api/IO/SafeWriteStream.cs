/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System.IO;
using miniblocks.API.Platform;

namespace miniblocks.API.IO;

internal class SafeWriteStream : FileStream
{
    private readonly string temporaryPath, finalPath;
    private readonly Storage storage;

    private bool isDisposed;

    public SafeWriteStream(string temporaryPath, string finalPath, Storage storage)
        : base(storage.GetFullPath(temporaryPath, true), FileMode.Create, FileAccess.Write)
    {
        this.temporaryPath = temporaryPath;
        this.finalPath = finalPath;
        this.storage = storage;
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            Dispose();
            return;
        }

        if (!isDisposed)
        {
            if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            {
                try
                {
                    Flush(true);
                }
                catch
                {
                }
            }
        }

        Dispose(true);

        if (!isDisposed)
        {
            storage.Delete(finalPath);
            storage.Move(temporaryPath, finalPath);

            isDisposed = true;
        }
    }
}
