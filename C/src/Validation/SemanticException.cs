using System.Text;
using Qkmaxware.Languages.C.Text;

namespace Qkmaxware.Languages.C.Validation;

/// <summary>
/// An exception thrown by the lexer
/// </summary>
public class SematicException : LanguageException {
    public SematicException (File source, Position position, string text) : base(source, position, text) {}
    public SematicException (File source, Position position, string text, Exception inner) : base(source, position, text, inner) {}
    public SematicException (string text) : base(text) {}
    public SematicException (string text, Exception inner) : base(text, inner) {}

    public override string GetUid() => "Semantic";
}