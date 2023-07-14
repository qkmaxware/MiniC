using System.Text;

namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// An exception thrown by the parser
/// </summary>
public class ParserException : LanguageException {
    public ParserException (File source, Position position, string text) : base(source, position, text) {}

    public ParserException (File source, Position position, string text, Exception inner) : base(source, position, text, inner) {}

    public override string GetUid() => "Syntactical";
}

public class ParserExceptionWithReference : ParserException {
    public File ReferenceCodeSource {get; private set;}
    public Position ReferencePositionInSource {get; private set;}
    public ParserExceptionWithReference (File source, Position position, File referenceSource, Position referencePosition, string text)
    : base(source, position, text) {
        this.ReferenceCodeSource = referenceSource;
        this.ReferencePositionInSource = referencePosition;
    }
}