using System.IO;
using System.Linq;

namespace MDDC
{
    public static class Util
    {
        public static string GetRelativePath(string directory, string fileNameInDirectory)
        {
            var i = directory.Length;

            if (directory.Last() != Path.DirectorySeparatorChar)
            {
                i++;
            }

            return fileNameInDirectory.Substring(i);
        }
    }
}
