using System.Collections.Generic;

namespace Sciendo.Junk.Detect.Library
{
    public interface IJunkDetector
    {
        IEnumerable<string> Detect(string path, string[] extensions);
        IEnumerable<string> GetExtensions(string path);
    }
}