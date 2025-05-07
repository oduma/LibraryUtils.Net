namespace Sciendo.Music.Tagging
{
    public interface IFile
    {
        byte[] Read(string path);
        bool Exists(string path);

        void WriteAlltext(string path, string content);
    }
}