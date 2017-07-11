# Table Page

## <a name="myTable"> Table

Table

|Col0|Col1|Col2
|
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row
|Row

### <a name="myCode"> Some Code

```csharp
using System;
using System.Diagnostics;
using System.IO;

namespace MDDC
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: source_dir [target_dir]");
                return;
            }

            var src = args[0].TrimEnd(Path.DirectorySeparatorChar);
            var dst = args.Length > 1 ? args[1] : src + "_converted";

            if (!Directory.Exists(src))
            {
                Console.WriteLine("Source dir does not exist. Make sure path is correct.");
                return;
            }

            Debug.Listeners.Add(new ConsoleTraceListener());

            DocGen.ConvertFolderToHtml(src, dst);
        }
    }
}

```