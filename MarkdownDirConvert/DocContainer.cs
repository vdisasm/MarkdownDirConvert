using Markdig.Syntax;

namespace MDDC
{
    public partial class DocGen
    {
        public class DocContainer
        {
            /// <summary>
            /// File name relative to base directory.
            /// </summary>
            public readonly string FileName;

            public readonly MarkdownDocument Doc;

            public readonly string RootPath;

            public DocContainer(string fileName, MarkdownDocument doc)
            {
                FileName = fileName;
                Doc = doc;

                var paths = fileName.Replace('/', '\\').Split('\\');
                var depth = paths.Length - 1;
                RootPath = "";
                for (int i = 0; i < depth; i++)
                {
                    RootPath += "..\\";
                }
            }
        }
    }
}
