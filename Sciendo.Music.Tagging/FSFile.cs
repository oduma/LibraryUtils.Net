using System.IO;

namespace Sciendo.Music.Tagging
{
    public class FSFile : IFile
    {
        public byte[] Read(string path)
        {
            return Exists(path) ? File.ReadAllBytes(path) : new byte[0];
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void WriteAlltext(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}