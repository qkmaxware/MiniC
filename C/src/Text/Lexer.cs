using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// convert a stream of characters to lexical tokens using a scanner
/// </summary>
public class Lexer {

    private List<Lexeme> lexemes = new List<Lexeme> {
        new LineCommentLexer(),
        new BlockCommentLexer(),

        new TwoCharLexeme('=', '=', (source, startsAt, endsAt) => new EqualityOperator(source, startsAt, endsAt)),
        new TwoCharLexeme('!', '=', (source, startsAt, endsAt) => new InequalityOperator(source, startsAt, endsAt)),
        new TwoCharLexeme('-', '>', (source, startsAt, endsAt) => new ArrowOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('=', (source, startsAt, endsAt) => new AssignmentOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('!', (source, startsAt, endsAt) => new NotOperator(source, startsAt, endsAt)),

        new SingleCharLexeme(';', (source, startsAt, endsAt) => new Semicolon(source, startsAt, endsAt)),
        new SingleCharLexeme(',', (source, startsAt, endsAt) => new Comma(source, startsAt, endsAt)),
        new SingleCharLexeme('<', (source, startsAt, endsAt) => new LessThanOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('>', (source, startsAt, endsAt) => new GreaterThanOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('+', (source, startsAt, endsAt) => new AddOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('-', (source, startsAt, endsAt) => new SubtractOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('*', (source, startsAt, endsAt) => new TimesOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('/', (source, startsAt, endsAt) => new DivideOperator(source, startsAt, endsAt)),
        new SingleCharLexeme('%', (source, startsAt, endsAt) => new RemainderOperator(source, startsAt, endsAt)),

        new SingleCharLexeme('(', (source, startsAt, endsAt) => new OpenParenthesis(source, startsAt, endsAt)),
        new SingleCharLexeme(')', (source, startsAt, endsAt) => new CloseParenthesis(source, startsAt, endsAt)),
        new SingleCharLexeme('[', (source, startsAt, endsAt) => new OpenBracket(source, startsAt, endsAt)),
        new SingleCharLexeme(']', (source, startsAt, endsAt) => new CloseBracket(source, startsAt, endsAt)),
        new SingleCharLexeme('{', (source, startsAt, endsAt) => new OpenBrace(source, startsAt, endsAt)),
        new SingleCharLexeme('}', (source, startsAt, endsAt) => new CloseBrace(source, startsAt, endsAt)),

        new AssemblyLexeme(),
        new KeywordOrIdentifierLexeme(),
        new NumericLexeme(),
        new StringLexer(),
    };

    /// <summary>
    /// Tokenize a stream of text into a sequence of tokens
    /// </summary>
    /// <param name="scanner">scanner to read text from</param>
    /// <returns>list of tokens</returns>
    public IEnumerable<Token> EnumerateTokens(TextPreprocessor scanner) {
        scanner.SkipWhitespace();

        while (scanner.HasNext()) {
            bool foundToken = false;
            foreach (var lexeme in lexemes) {
                var token = lexeme.Read(scanner);
                if (!object.ReferenceEquals(token, null)) {
                    yield return token;
                    foundToken = true;
                    break;
                }
            }
            if (foundToken == false) {
                var next = scanner.Peek();
                if (next.HasValue) {    
                    throw new LexicalException(next.Value.Source, next.Value.Position, $"Unrecognized character '{next.Value.Character}'.");
                } else {
                    throw new EndOfStreamException();
                }
            } else {
                scanner.SkipWhitespace();
                continue;
            }
        }
    }

    /// <summary>
    /// Tokenize a stream of text into a sequence of tokens
    /// </summary>
    /// <param name="scanner">scanner to read text from</param>
    /// <returns>a peek-able stream of tokens</returns>
    public TokenStream Tokenize(Scanner scanner) {
        return new TokenStream(EnumerateTokens(scanner).GetEnumerator());
    }
}