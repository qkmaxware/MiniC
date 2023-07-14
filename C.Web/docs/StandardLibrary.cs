using System.Text;
using System.Text.RegularExpressions;

namespace C.Web.Docs;

public class StandardLibraryDoc : Document {
    public StandardLibraryDoc() : base(
        "Standard Library",
        new Dictionary<string, object>{
            { "group", "Language" }
        },
        getContent()
    ) {} 

    private static Regex newline = new Regex("\r?\n");

    private static string getContent() {
        StringBuilder sb = new StringBuilder();
        var files = Qkmaxware.Languages.C.StandardLib.LibraryLoader.Load().Files();

        sb.AppendLine("# Standard Library");
        sb.AppendLine("The Mini-C language runtime provided in the web and downloadable versions of the compiler includes a small standard library allowing coders to write some programs easier thanks to pre-made functionality. These can be included in your code using the include directive which you can read about in the directives document. The full text of each of the standard library files is included below.");
        sb.AppendLine();

        foreach (var file in files) {
            sb.AppendLine($"## {file.Name}");
            sb.Append("<pre><code>");
            sb.Append(newline.Replace(((Qkmaxware.Languages.C.VirtualFile)file).GetContent(), @"<br>"));
            sb.AppendLine("</code></pre>");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}