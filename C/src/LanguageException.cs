using System.Text;
using Qkmaxware.Languages.C.Text;

namespace Qkmaxware.Languages.C;

/// <summary>
/// An exception thrown at any time in the language's lifecycle
/// </summary>
public abstract class LanguageException : Exception, IPrettyPrint {
    public File? CodeSource {get; private set;}
    public Position? PositionInSource {get; private set;}

    public abstract string GetUid();

    public LanguageException (string text) : base(text) {
        this.CodeSource = null;
        this.PositionInSource = null;
    }

    public LanguageException (Exception inner) : base(inner.Message, inner) {
        this.CodeSource = null;
        this.PositionInSource = null;
    }

    public LanguageException (string text, Exception inner) : base(text, inner) {
        this.CodeSource = null;
        this.PositionInSource = null;
    }

    public LanguageException (File source, Position position, string text) : base(text) {
        this.CodeSource = source;
        this.PositionInSource = position;
    }

    public LanguageException (File source, Position position, string text, Exception inner) : base(text, inner) {
        this.CodeSource = source;
        this.PositionInSource = position;
    }

    public LanguageException (File source, Position position, Exception inner) : base(inner.Message, inner) {
        this.CodeSource = source;
        this.PositionInSource = position;
    }

    public string PrettyPrint() {
        if (object.ReferenceEquals(this.PositionInSource, null) || object.ReferenceEquals(this.CodeSource,null))
            return this.ToString();

        StringBuilder sb = new StringBuilder();

        sb.Append("Error["); sb.Append(this.GetUid()); sb.Append("]: "); sb.AppendLine(this.Message);
        var line = PositionInSource.LineNumber;
        var lineString = PositionInSource.LineNumber.ToString();
        var tab = new string(' ', lineString.Length);
        sb.Append("--> "); sb.Append(CodeSource.Name); sb.Append("@Ln "); sb.Append(PositionInSource.LineNumber); sb.Append(", Col "); sb.AppendLine(PositionInSource.Column.ToString());
        sb.Append(' '); sb.Append(tab); sb.AppendLine(" | ");
        sb.Append(' '); sb.Append(lineString); sb.Append(" | ");
        using (var reader = CodeSource.Open()) {
            for (int i = 1; i < line; i++)
                reader.ReadLine();
            sb.AppendLine(reader.ReadLine());
        }
        sb.Append(' '); sb.Append(tab); sb.Append(" |"); sb.Append(new string(' ', (int)PositionInSource.Column)); sb.AppendLine("^-- Here");
        
        if (!object.ReferenceEquals(this.InnerException, null)) {
            sb.AppendLine();
            sb.AppendLine(this.InnerException.Message);
        }

        sb.AppendLine();
        sb.AppendLine("Stack Trace:");
        sb.AppendLine(this.StackTrace);

        return sb.ToString();
    }
}