﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NYaul.IO;

/// <summary>
/// Provides extended directory-related utility methods and a fluent copy API.
/// </summary>
public static class DirectoryEx
{
    public static IEnumerable<DirectoryInfo> TraverseCurrentParents()
        => new DirectoryInfo(Environment.CurrentDirectory).TraverseParents();

    // DeleteIfExists
    public static void DeleteIfExists(string path, bool recursive = false)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive);
        }
    }

    public static void DeleteIfExists(this DirectoryInfo directory, bool recursive = false)
    {
        directory.Refresh();
        if (directory.Exists)
        {
            directory.Delete(recursive);
        }
    }

    // GetTotalSize
    public static long GetTotalSize(this DirectoryInfo directory)
    {
        if (!directory.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + directory.FullName);
        }

        return directory.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
            .Sum(x => x is FileInfo fi ? fi.Length : 0);
    }

    internal static void CopyImpl(this DirectoryInfo sourceDir, string destDirName, Func<FileSystemInfo, bool>? filter = null)
    {
        filter ??= _ => true;
        if (!sourceDir.Exists)
        {
            sourceDir.Refresh();
        }

        if (!sourceDir.Exists)
        {
            throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + sourceDir.Name);
        }

        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Use EnumerateFiles method to get both file names and FileInfo objects.
        var files = sourceDir.EnumerateFileSystemInfos().Where(filter);
        foreach (FileSystemInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            if (file is FileInfo fileInfo)
            {
                fileInfo.CopyTo(tempPath, true);
            }
            else if (file is DirectoryInfo subDir)
            {
                subDir.CopyImpl(tempPath, filter);
            }
        }
    }

    public static IDirectoryFromBuilder Copy => DirectoryCopyBuilder.Copy;
}

/// <summary>
/// Defines methods for specifying the source directory in a fluent copy builder sequence.
/// </summary>
public interface IDirectoryFromBuilder
{
    IDirectoryToBuilder From(string sourcePath);

    IDirectoryToBuilder From(DirectoryInfo sourcePath);

    //{
    //    return From(sourcePath.FullName);
    //}
}

/// <summary>
/// Defines methods for specifying the destination directory in a fluent copy builder sequence.
/// </summary>
public interface IDirectoryToBuilder
{
    IDirectoryCopyBuilder To(string destinationPath);

    IDirectoryCopyBuilder To(DirectoryInfo destinationPath);

    //{
    //    return To(destinationPath.FullName);
    //}
}

/// <summary>
/// Provides methods for configuring directory copy operations and executing them.
/// </summary>
public interface IDirectoryCopyBuilder
{
    IDirectoryCopyBuilder WithFileFilter(Func<FileInfo, bool> fileFilter);

    IDirectoryCopyBuilder WithDirectoryFilter(Func<DirectoryInfo, bool> directoryFilter);

    IDirectoryCopyBuilder DeepCopy();

    /// <summary>
    /// Alias of DeepCopy. Copies subdirectories and files.
    /// </summary>
    /// <returns></returns>
    IDirectoryCopyBuilder WithSubdirectories();

    //=> DeepCopy();

    void Invoke();

    /// <summary>
    /// Alias of Invoke. Copies subdirectories and files.
    /// </summary>
    void Copy();// => Invoke();
}

    /// <summary>
    /// Provides a fluent interface for configuring and executing directory copy operations,
    /// including filtering files and directories and optionally performing a deep copy.
    /// </summary>
public class DirectoryCopyBuilder : IDirectoryFromBuilder, IDirectoryToBuilder, IDirectoryCopyBuilder
{
    private string _sourcePath;
    private string _destinationPath;

    private readonly List<Func<FileSystemInfo, bool>> _directoryFilters = new();
    private readonly List<Func<FileSystemInfo, bool>> _fileFilters = new();
    private bool _deepCopy;

    private DirectoryCopyBuilder()
    {
        _sourcePath = string.Empty;
        _destinationPath = string.Empty;
    }

    public static DirectoryCopyBuilder Copy => new DirectoryCopyBuilder();

    public IDirectoryToBuilder From(string sourcePath)
    {
        _sourcePath = sourcePath;
        return this;
    }

    public IDirectoryCopyBuilder To(string destinationPath)
    {
        _destinationPath = destinationPath;
        return this;
    }

    public IDirectoryCopyBuilder WithFileFilter(Func<FileInfo, bool> fileFilter)
    {
        _fileFilters.Add(fsi => fsi is FileInfo fi && fileFilter(fi));
        return this;
    }

    public IDirectoryCopyBuilder WithDirectoryFilter(Func<DirectoryInfo, bool> directoryFilter)
    {
        _directoryFilters.Add(fsi => fsi is DirectoryInfo di && directoryFilter(di));
        return this;
    }

    public IDirectoryCopyBuilder DeepCopy()
    {
        _deepCopy = true;
        return this;
    }

    public void Invoke()
    {
        if (string.IsNullOrEmpty(_sourcePath))
        {
            throw new InvalidOperationException("Source path cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(_destinationPath))
        {
            throw new InvalidOperationException("Destination path cannot be null or empty.");
        }

        var sourceDirInfo = new DirectoryInfo(_sourcePath);

        Func<FileInfo, bool> filefilterEvaluator = _fileFilters.Count > 0
             ? fsi => _fileFilters.Any(filter => filter(fsi))
             : _ => true;

        Func<DirectoryInfo, bool> directoryFilterEvaluator = _directoryFilters.Count > 0
            ? fsi => _directoryFilters.Any(filter => filter(fsi))
            : _ => true;

        Func<FileSystemInfo, bool> filter;
        if (!_deepCopy)
        {
            filter = fsi => fsi is FileInfo fi && filefilterEvaluator(fi);
        }
        else
        {
            filter = fsi =>
            {
                if (fsi is FileInfo fi)
                {
                    return filefilterEvaluator(fi);
                }
                else if (fsi is DirectoryInfo di)
                {
                    return directoryFilterEvaluator(di);
                }
                else
                {
                    return false;
                }
            };
        }

        sourceDirInfo.CopyImpl(_destinationPath, filter);
    }

    public IDirectoryToBuilder From(DirectoryInfo sourcePath)
    {
        return From(sourcePath.FullName);
    }

    public IDirectoryCopyBuilder To(DirectoryInfo destinationPath)
    {
        return To(destinationPath.FullName);
    }

    public IDirectoryCopyBuilder WithSubdirectories()
    {
        return DeepCopy();
    }

    void IDirectoryCopyBuilder.Copy()
    {
        Invoke();
    }
}
