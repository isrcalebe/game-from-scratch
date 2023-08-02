/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using miniblocks.API.Extensions.ObjectExtensions;
using miniblocks.API.IO;

namespace miniblocks.API.Platform;

public abstract class Storage
{
    protected string BasePath { get; }

    protected Storage(string path, string subfolder = null)
    {
        static string fileNameStrip(string entry)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                entry = entry.Replace(c.ToString(), string.Empty);

            return entry;
        }

        BasePath = path;

        if (BasePath == null)
            throw new InvalidOperationException($"{nameof(BasePath)} not correctly initialized!");

        if (!string.IsNullOrEmpty(subfolder))
            BasePath = Path.Combine(BasePath, fileNameStrip(subfolder));
    }

    public abstract string GetFullPath(string path, bool createIfNotExists = false);

    public abstract bool Exists(string path);

    public abstract bool ExistsDirectory(string path);

    public abstract void Delete(string path);

    public abstract void DeleteDirectory(string path);

    public abstract IEnumerable<string> GetDirectories(string path);

    public abstract IEnumerable<string> GetFiles(string path, string pattern = "*");

    public abstract void Move(string from, string to);

    public abstract bool OpenFileExternally(string fileName);

    public abstract bool PresentFileExternally(string fileName);

    public bool PresentExternally()
        => OpenFileExternally(string.Empty);

    [Pure]
    public abstract Stream GetStream(string path, FileAccess access = FileAccess.Read, FileMode mode = FileMode.OpenOrCreate);

    [Pure]
    public Stream CreateFileSafely(string path)
    {
        var temporaryPath = Path.Combine(Path.GetDirectoryName(path).AsNonNull(), $"_{Path.GetFileName(path)}_{Guid.NewGuid()}");

        return new SafeWriteStream(temporaryPath, path, this);
    }

    public virtual Storage GetStorageForDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Must be non-null and not empty string.", nameof(path));

        if (!path.EndsWith(Path.DirectorySeparatorChar))
            path += Path.DirectorySeparatorChar;

        var fullPath = GetFullPath(path, true);

        return (Storage)Activator.CreateInstance(GetType(), fullPath);
    }
}
