using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// Base class for a token lexeme
/// </summary>
public abstract class Lexeme {
    /// <summary>
    /// Attempt to read the next token. Shouldn't consume input if a token cannot be read.
    /// </summary>
    /// <returns>token if the token can be read, null otherwise</returns>
    public abstract Token? Read(Scanner scanner);
}