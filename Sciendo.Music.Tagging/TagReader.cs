using System;
using System.IO;
using TagLib;

namespace Sciendo.Music.Tagging
{
    public static class TagReader
    {
        public static TagLib.File ReadTag(MemoryStream fs, string path)
        {
            var fsa = new StreamFileAbstraction(path, fs, fs);
            try
            {
                TagLib.File file = TagLib.File.Create(fsa);
                return file;
            }
            catch (CorruptFileException cex)
            {
                Console.WriteLine(cex);
                return null;
            }

        }

        public static TagLib.File ReadTag(this IFile fsFile, string path)
        {
            using (MemoryStream fs = new MemoryStream(fsFile.Read(path)))
            {
                return TagReader.ReadTag(fs, path);
            }
        }

    }
}