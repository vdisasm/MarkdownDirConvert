using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MDDC
{
    public partial class DocGen
    {
        static string MdToHtmlFileName(string path)
        {
            return Path.ChangeExtension(path, ".html");
        }

        static void CopyDirectoriesExceptMd(string src, string dst, Predicate<string> fileNameFilter)
        {
            // Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(src, dst));
            }

            // Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(src, "*.*", SearchOption.AllDirectories))
            {
                if (fileNameFilter(newPath))
                {
                    File.Copy(newPath, newPath.Replace(src, dst), true);
                }
            }
        }

        public static void ConvertFolderToHtml(string sourceFolder, string targetFolder)
        {
            Debug.WriteLine("Copy directory content");
            Debug.WriteLine("  from: " + sourceFolder);
            Debug.WriteLine("    to: " + targetFolder);
            CopyDirectoriesExceptMd(sourceFolder, targetFolder, (fn) => Path.GetExtension(fn).ToLower() != ".md");


            Debug.WriteLine("Parsing Markdown files");

            // Parse.
            var pipeline = new MarkdownPipelineBuilder().
                UsePipeTables().
                Build();

            var containers = new List<DocContainer>();

            foreach (var fileName in Directory.EnumerateFiles(sourceFolder, "*.md", SearchOption.AllDirectories))
            {
                var relFileName = Util.GetRelativePath(sourceFolder, fileName);

                var text = File.ReadAllText(fileName);
                var doc = Markdown.Parse(text, pipeline);
                containers.Add(new DocContainer(relFileName, doc));
            }

            Debug.WriteLine("Fixing links");

            // Fix links.
            FixLinks(containers);

            Debug.WriteLine("Exporting");

            // Export
            var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var marginSourceDir = Path.Combine(dir, "margin");

            Debug.WriteLine("  Copy .css and .js");

            var marginTargetDir = Path.Combine(targetFolder, "margin");
            Directory.CreateDirectory(marginTargetDir);
            File.Copy(Path.Combine(marginSourceDir, "highlight.css"), Path.Combine(marginTargetDir, "highlight.css"), true);
            File.Copy(Path.Combine(marginSourceDir, "mermaid.css"), Path.Combine(marginTargetDir, "mermaid.css"), true);
            File.Copy(Path.Combine(marginSourceDir, "mermaid.js"), Path.Combine(marginTargetDir, "mermaid.js"), true);
            File.Copy(Path.Combine(marginSourceDir, "prism.js"), Path.Combine(marginTargetDir, "prism.js"), true);

            Debug.WriteLine("  Write htmls");
            foreach (var container in containers)
            {
                var htmlFileName = MdToHtmlFileName(container.FileName);
                var target = Path.Combine(targetFolder, htmlFileName);
                DocToFile(container, target, pipeline);
            }
        }

        static void WriteCssLinks(TextWriter writer, DocContainer container)
        {
            writer.WriteLine($"<link rel=\"stylesheet\" href=\"{Path.Combine(container.RootPath, "margin\\highlight.css")}\" />");
            writer.WriteLine($"<link rel=\"stylesheet\" href=\"{Path.Combine(container.RootPath, "margin\\mermaid.css")}\" />");
        }

        static void WriteScripts(TextWriter writer, DocContainer container)
        {
            writer.WriteLine($"<script src=\"{Path.Combine(container.RootPath, "margin\\prism.js")}\"></script>");
            writer.WriteLine($"<script src=\"{Path.Combine(container.RootPath, "margin\\mermaid.js")}\"></script>");
        }

        static void DocToFile(DocContainer container, string targetPath, MarkdownPipeline pipeline)
        {
            var dir = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(dir);

            using (var writer = new StreamWriter(targetPath, false, Encoding.UTF8))
            {
                writer.WriteLine("<!DOCTYPE html>");
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<meta charset=\"UTF-8\">");

                WriteCssLinks(writer, container);

                writer.WriteLine("</head>");
                writer.WriteLine("<body class=\"markdown-body\">");

                WriteRightDiv(container, writer);

                writer.WriteLine("<div id='___markdown-content___'>");

                var renderer = new HtmlRenderer(writer);

                // We override the renderer with our own writer
                pipeline.Setup(renderer);

                renderer.Render(container.Doc);

                writer.WriteLine("</div>");

                WriteScripts(writer, container);

                writer.WriteLine("</body>");
                writer.Write("</html>");
            }
        }

        static void WriteRightDiv(DocContainer container, StreamWriter writer)
        {
            writer.WriteLine(@"
<style>
.markdown-body {
margin:5px,0,5px,0;
}

#___markdown-content___ {
float:left;
width:75%;
}

.rightdiv {
position:fixed;
margin-left:75%;
width:25%;
height:100%;
padding:10px;
}
</style>");

            writer.WriteLine($"<div class=\"rightdiv\">");

            int headingId = 0;

            foreach (var h in container.Doc.Descendants().OfType<HeadingBlock>())
            {
                var headingIdText = $"heading{headingId}";
                h.SetData(typeof(HtmlAttributes), new HtmlAttributes { Id = headingIdText });

                var literal = h.Inline.Descendants<LiteralInline>().First();
                writer.Write($"<a href=\"#{headingIdText}\" style=\"display: inline-block; text-indent: {h.Level * 20}px\">");
                writer.Write(literal.Content);
                writer.Write("</a><br/>");
                writer.WriteLine();

                headingId++;
            }

            writer.WriteLine("</div>");
        }

        static void FixLinks(IEnumerable<DocContainer> containers)
        {
            foreach (var container in containers)
            {
                foreach (var link in container.Doc.Descendants().OfType<LinkInline>())
                {
                    link.Url = link.Url.Replace(".md", ".html");
                }
            }
        }
    }
}
