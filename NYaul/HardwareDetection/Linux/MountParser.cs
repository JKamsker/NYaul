﻿namespace NYaul.HardwareDetection.Linux;

using NYaul.IO.FileProvider;
using System;
using System.Collections.Generic;
using System.IO;

public class MountEntry
{
    public string DeviceName => Device == null ? string.Empty : Path.GetFileName(Device);
    public string? Device { get; set; }
    public string? MountPoint { get; set; }
    public string? FileSystem { get; set; }
    public string? Options { get; set; }
}

public class MountParser
{
    private readonly IFileProvider _fileProvider;

    public MountParser(IFileProvider? fileProvider = null)
    {
        _fileProvider = fileProvider ?? DefaultFileProvider.Instance;
    }

    public IEnumerable<MountEntry> ParseMounts()
    {
        if (_fileProvider.FileExists("/proc/mounts"))
        {
            return ReadMount("/proc/mounts");
        }
        else if (_fileProvider.FileExists("/etc/mtab"))
        {
            return ReadMount("/etc/mtab");
        }
        else
        {
            throw new Exception("Could not find mount file");
        }
    }

    internal IEnumerable<MountEntry> ParseMtab()
    {
        string mountsFile = "/etc/mtab";
        return ReadMount(mountsFile);
    }

    internal IEnumerable<MountEntry> ParseProcMounts()
    {
        string mountsFile = "/proc/mounts";
        return ReadMount(mountsFile);
    }

    private IEnumerable<MountEntry> ReadMount(string mountsFile)
    {
        if (!_fileProvider.FileExists(mountsFile))
        {
            yield break;
        }

        using Stream stream = _fileProvider.OpenRead(mountsFile);
        using StreamReader reader = new StreamReader(stream);
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var normalizedLine = line.TrimStart(' ');
            if (string.IsNullOrWhiteSpace(normalizedLine))
            {
                continue;
            }

            if (normalizedLine.StartsWith("#"))
            {
                continue;
            }

            string[] parts = normalizedLine.Split(' ');
            MountEntry entry = new MountEntry();
            entry.Device = ElementAtOrDefault(parts, 0);
            entry.MountPoint = ElementAtOrDefault(parts, 1);
            entry.FileSystem = ElementAtOrDefault(parts, 2);
            entry.Options = ElementAtOrDefault(parts, 3);

            yield return entry;
        }
    }

    private string ElementAtOrDefault(string[] array, int pos)
    {
        if (array.Length > pos)
        {
            return array[pos];
        }
        else
        {
            return string.Empty;
        }
    }
}