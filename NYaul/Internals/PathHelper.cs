//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NYaul.Internals;

//internal static class PathHelper
//{
//    private static string EnsurePathRooted(string path)
//    {
//        if (Path.IsPathRooted(path))
//        {
//            return path;
//        }

//        // trimstart: ./ and .\
//        if (path.StartsWith("./") || path.StartsWith(".\\"))
//        {
//            path = path[2..];
//        }

//        return Path.Combine(Environment.CurrentDirectory, path);
//    }

//    public static IEnumerable<string> EnumerateParents(string path)
//    {
//        path = EnsurePathRooted(path);
//        while (true)
//        {
//            yield return path;

//            var parent = Path.GetDirectoryName(path);
//            if (string.IsNullOrEmpty(parent))
//                yield break;
//            path = parent;
//        }
//    }

//    public static string ResolveSymLinksTarget(string input)
//    {
//        try
//        {
//            var normalizedInput = input.TrimEnd('\\', '/');

//            var parents = EnumerateParents(normalizedInput).SkipLast(1);
//            foreach (var parent in parents)
//            {
//                FileSystemInfo di = new DirectoryInfo(parent);
//                if (!di.Exists)
//                {
//                    var fi = new FileInfo(parent);
//                    if (!fi.Exists)
//                    {
//                        continue;
//                    }

//                    di = fi;
//                }

//                var target = di.ResolveLinkTarget(returnFinalTarget: true);
//                if (target == null)
//                {
//                    continue;
//                }
//                // we have found the nearest symlink
//                // now we need to reapply the rest of the path

//                if (string.Equals(normalizedInput, parent))
//                {
//                    return target.FullName;
//                }

//                var relativeToParent = normalizedInput.Substring(parent.Length + 1);
//                var result = Path.Combine(target.FullName, relativeToParent);
//                return result;
//            }

//            return normalizedInput;
//        }
//        // Usually linux code under windows
//        catch (DirectoryNotFoundException)
//        {
//            return input;
//        }
//    }
//}