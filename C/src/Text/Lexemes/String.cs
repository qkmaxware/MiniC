using System.Text;
using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

namespace Qkmaxware.Languages.C.Text;

public class StringLexer : Lexeme {

    public char StartChar = '"';

    public StringLexer(char start = '"') {
        this.StartChar = start;
    }

    public override Token? Read(Scanner s) {
        var first = s.Peek(0);
        if (!first.HasValue || !(first.Value.Character == StartChar)) {
            return null;
        } 
        var file = first.Value.Source;
        var before = first.Value.Position;
        s.Read();

        var content = new StringBuilder();

        ReadCharacter? peek;
        while ((peek = s.Peek(0)).HasValue && peek.Value.Character != StartChar) {
            if (peek.HasValue && peek.Value.Character ==('\\')) {
                s.Read();

                peek = s.Peek();
                if (peek.HasValue && peek.Value.Character ==('\'')) {
                    content.Append(peek.Value.Character); s.Read();
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('\"')) {
                    content.Append(peek.Value.Character); s.Read();
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('\\')) {
                    content.Append(peek.Value.Character); s.Read();
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('/')) {
                    content.Append(peek.Value.Character); s.Read();
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('b')) {
                    s.Read();
                    content.Append('\b');
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('f')) {
                    s.Read();
                    content.Append('\f');
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('n')) {
                    s.Read();
                    content.Append('\n');
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('r')) {
                    s.Read();
                    content.Append('\r');
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('t')) {
                    s.Read();
                    content.Append('\t');
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('v')) {
                    s.Read();
                    content.Append('\v');
                    continue;
                }
                else if (peek.HasValue && peek.Value.Character ==('u')) {
                    s.Read();
                    var start = peek;
                    char[] chars = new char[4];
                    for (var i = 0; i < 4; i++) {
                        peek = s.Peek(0);
                        if (peek.HasValue && char.IsLetterOrDigit(peek.Value.Character)) {
                            char next = peek.Value.Character; s.Read();
                            chars[i] = next;
                        } else {
                            throw new LexicalException(start.Value.Source, start.Value.Position, "Missing digit " + ((i + 1)) + " in unicode code-point.");
                        }
                    }
                    var value = int.Parse(chars, System.Globalization.NumberStyles.HexNumber);
                    content.Append(char.ConvertFromUtf32(value));
                }
                else {
                    throw new Exception($"Invalid escape character '{s.Read()}'");
                }
            }

            // Not an escaped character, just add it
            else {
                content.Append(peek.Value.Character); s.Read();
            }
        }

        var last = s.Peek(0);
        if (!last.HasValue || last.Value.Character != StartChar)
            throw new LexicalException(file, first.Value.Position, "Missing closing '\"' on string literal.");
        s.Read();

        return new StringToken(file, before, before, content.ToString());
    }
}

public class StringToken : TextToken {
    public StringToken(File source, Position startsAt, Position endsAt, string text) : base(source, startsAt, endsAt, text) {}
}
