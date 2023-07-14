using System.Text;

namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// An exception thrown by the lexer
/// </summary>
public class LexicalException : LanguageException {
    public LexicalException (File source, Position position, string text) : base(source, position, text) {}

    public override string GetUid() => "Lexical";
}