using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace C.Web;

public class Group {
    public string? GroupName {get; private set;}
    public Group(string? name) => this.GroupName = name;

    public bool IsUngroup => String.IsNullOrEmpty(GroupName);

    public override bool Equals(object? obj) {
        if (obj != null && obj is Group group) {
            return this.GroupName == group.GroupName;
        } else {
            return false;
        }
    }
    public override int GetHashCode() => GroupName?.GetHashCode() ?? -1;
}

public class Document {
    public Dictionary<string,Object> Metadata {get; private set;}
    public string RawContent {get; private set;}
    public string MarkdownContent => md.Transform(RawContent);

    private string _name;
    public string Name => Metadata != null && Metadata.ContainsKey("title") ? (Metadata["title"].ToString() ?? _name) : _name;
    public Group Group => Metadata != null && Metadata.ContainsKey("group") ? new Group(Metadata["group"].ToString()) : new Group(null);

    public static readonly Regex FrontMatterPattern = new Regex(@"\A---(.*?)---", RegexOptions.Multiline | RegexOptions.Singleline);  

    private static IDeserializer yaml 
        = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    private static MarkdownSharp.Markdown md = new MarkdownSharp.Markdown(
        new MarkdownSharp.MarkdownOptions {
            
        }
    );

    public Document(string name, Dictionary<string,Object> metadata, string content) {
        this._name = name;
        this.Metadata = metadata;
        this.RawContent = content;
    }

    public static IEnumerable<Document> Load() {
        // Load embedded resources as virtual files
        var assembly = typeof(Document).Assembly;
        var prefix = assembly.GetName().Name + ".docs.";
        //Console.WriteLine(prefix);
        foreach (var name in assembly.GetManifestResourceNames()) {
            //Console.WriteLine(name);
            if (name.StartsWith(prefix) && name.EndsWith(".md")) {
                var name_only = Path.GetFileNameWithoutExtension(name.Remove(0, prefix.Length));
                using var builder = new StringWriter();
                Stream? stream = assembly.GetManifestResourceStream(name);
                if (stream != null) {
                    using (var reader = new StreamReader(stream)) {
                        builder.Write(reader.ReadToEnd());
                    }
                }
                var text = builder.ToString();

                // Extract front-matter
                var front_matter = FrontMatterPattern.Match(text).Groups[1].Value ?? string.Empty;

                // Extract content
                var content = FrontMatterPattern.Replace(text, string.Empty);

                yield return new Document(name_only, yaml.Deserialize<Dictionary<string, object>>(front_matter), content);
            }
        }
    }
}

