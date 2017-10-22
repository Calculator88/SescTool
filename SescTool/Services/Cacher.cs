using System.IO;

namespace SescTool.Services
{
    public class Cacher
    {
        public void Cache(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public string GetContent(string path)
        {
            return File.ReadAllText(path);
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }
    }
}