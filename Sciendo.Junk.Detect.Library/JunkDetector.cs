using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sciendo.Junk.Detect.Library
{
    public class JunkDetector : IJunkDetector
    {
        public IEnumerable<string> Detect(string path, string[] extensions)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                yield break;

            foreach (var result in DetectRecursive(path, extensions))
            {
                yield return result;
            }
        }

        private IEnumerable<string> DetectRecursive(string currentPath, string[] extensions)
        {
            // Process files in current directory
            var extensionSet = new HashSet<string>(extensions.Select(e => e.StartsWith(".") ? e.ToLowerInvariant() : $".{e.ToLowerInvariant()}"));
            var allFiles = Directory.GetFiles(currentPath);
            foreach (var file in allFiles)
            {
                var extension = Path.GetExtension(file).ToLowerInvariant();
                if (!string.IsNullOrEmpty(extension) && !extensionSet.Contains(extension))
                {
                    yield return file;
                }
            }

            // Process subdirectories
            foreach (var directory in Directory.GetDirectories(currentPath))
            {
                foreach (var result in DetectRecursive(directory, extensions))
                {
                    yield return result;
                }
            }
        }

        public IEnumerable<string> GetExtensions(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                yield break;

            var extensions = new HashSet<string>();
            foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                var extension = Path.GetExtension(file);
                if (!string.IsNullOrEmpty(extension))
                {
                    extensions.Add(extension);
                }
            }

            foreach (var extension in extensions)
            {
                yield return extension;
            }
        }
    }
}