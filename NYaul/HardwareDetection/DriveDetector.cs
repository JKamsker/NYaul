//using NYaul.IO.FileProvider;
//using NYaul.IO.HardwareDetection.Linux;
//using NYaul.IO.HardwareDetection.Windows;

//using NYaul.Internals;

////using NYaul.IO.FileProvider;
////using NYaul.IO.Paths;

//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Text.RegularExpressions;

//[assembly: InternalsVisibleTo("NYaul.Tests")]

//namespace NYaul.IO.HardwareDetection;

//public class DriveDetector
//{
//    static DriveDetector()
//    {
//        if (OperatingSystemPolyfill.IsWindows())
//        {
//            _isOnSSD = WindowsDriveDetector.IsOnSSD;
//        }
//        else if (OperatingSystemPolyfill.IsLinux())
//        {
//            _isOnSSD = (string path) => LinuxDriveDetector.IsOnSSD(path, null);
//        }
//        else
//        {
//            _isOnSSD = _ => null;
//        }
//    }

//    private static readonly Func<string, bool?> _isOnSSD;

//    public static bool? IsOnSSD(string path)
//    {
//        return _isOnSSD(path);
//    }

//    internal class LinuxDriveDetector
//    {
//        public static bool? IsOnSSD(string path, IFileProvider? fileProvider = null)
//        {
//            try
//            {
//                var parser = new MountParser(fileProvider);
//                var hddDetector = new RotationalFileReader(fileProvider);

//                var finalPath = PathHelper.ResolveSymLinksTarget(path);
//                var bestEntry = GetBestMountEntry(parser.ParseMounts(), finalPath);
//                var deviceName = bestEntry.DeviceName;

//                return !hddDetector.IsRotational(deviceName);
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }

//        public static MountEntry GetBestMountEntry(IEnumerable<MountEntry> mountEntries, string filePath)
//        {
//            MountEntry bestMountEntry = null;
//            int bestMatchLength = -1;

//            foreach (var mountEntry in mountEntries)
//            {
//                string mountPoint = mountEntry.MountPoint;

//                if (filePath.StartsWith(mountPoint) && mountPoint.Length > bestMatchLength)
//                {
//                    bestMountEntry = mountEntry;
//                    bestMatchLength = mountPoint.Length;
//                }
//            }

//            return bestMountEntry;
//        }
//    }

//    internal class WindowsDriveDetector
//    {
//        // Finds
//        private static readonly Regex _driveLetterFinder = new Regex(@"([a-zA-Z]){1}:", RegexOptions.Compiled);

//        public static bool? IsOnSSD(string path)
//        {
//            //Returns something like \\?\C:\Users\
//            //var finalPath = FinalPathResolver.GetFinalPathName(path);

//            //Using a more integrated approach that works an any os
//            var finalPath = PathHelper.ResolveSymLinksTarget(path);
//            var driveLetter = _driveLetterFinder.Match(finalPath);
//            if (!driveLetter.Success)
//            {
//                return null;
//            }
//            var result = driveLetter.Groups[1].Value;
//            try
//            {
//                return !WindowsSeekDetection.HasSeekPenalty(result[0]);
//            }
//            catch (Exception)
//            {
//                return null;
//            }
//        }
//    }
//}