using System.Text;
using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

namespace Qkmaxware.Languages.C.Text;

public class NumericLexeme : Lexeme {
    public override Token? Read(Scanner s) {
        var next = s.Peek();
        if (!next.HasValue || !(char.IsDigit(next.Value.Character))) {
            return null;
        } 
        var source = next.Value.Source;
        var begin = next.Value.Position;
        var after = begin;

        // Whole integer digits
        StringBuilder sb = new StringBuilder();
        while ((next = s.Peek()).HasValue && char.IsDigit(next.Value.Character)) {
            sb.Append(next.Value.Character);
            s.Read();
            after = next.Value.Position;
        }

        // unsigned
        next = s.Peek();
        if (next.HasValue && next.Value.Character == 'u') {
            s.Read();
            after = next.Value.Position;
            return new UIntegerLiteralToken(source, begin, after, uint.Parse(sb.ToString()));
        } else if (next.HasValue && next.Value.Character == 'f') {
            s.Read();
            after = next.Value.Position;
            return new FloatLiteralToken(source, begin, after, float.Parse(sb.ToString()));
        } else if (next.HasValue && next.Value.Character == '.') {
            s.Read();
            sb.Append(next.Value.Character);

            while ((next = s.Peek()).HasValue && char.IsDigit(next.Value.Character)) {
                sb.Append(next.Value.Character);
                s.Read();
                after = next.Value.Position;
            }

            return new FloatLiteralToken(source, begin, after, float.Parse(sb.ToString()));
        } else {
            return new IntegerLiteralToken(source, begin, after, int.Parse(sb.ToString()));
        }
    }
}

public class KeywordOrIdentifierLexeme : Lexeme {
    public override Token? Read(Scanner s) {
        var next = s.Peek();
        if (next.HasValue && char.IsLetter(next.Value.Character)) {
            var source = next.Value.Source;
            var before = next.Value.Position;
            var after = before;
            StringBuilder sb = new StringBuilder();
            while ((next = s.Peek()).HasValue && (char.IsLetterOrDigit(next.Value.Character) || next.Value.Character == '_')) {
                sb.Append(next.Value.Character);
                s.Read();
                after = next.Value.Position;
            }

            var text = sb.ToString();
            return text switch {
                "void" => new VoidKeyword(source, before, after),
                "int" => new IntKeyword(source, before, after),
                "uint" => new UIntKeyword(source, before, after),
                "float" => new FloatKeyword(source, before, after),
                "bool" => new BoolKeyword(source, before, after),
                "char" => new CharKeyword(source, before, after),
                
                "new" => new NewKeyword(source, before, after),
                "len" => new LengthKeyword(source, before, after),
                "sizeof" => new LengthKeyword(source, before, after),
                "free" => new FreeKeyword(source, before, after),

                "while" => new WhileKeyword(source, before, after),
                "for" => new ForKeyword(source, before, after),
                "if" => new IfKeyword(source, before, after),
                "else" => new ElseKeyword(source, before, after),
                "return" => new ReturnKeyword(source, before, after),
                "break" => new BreakKeyword(source, before, after),
                "continue" => new ContinueKeyword(source, before, after),

                "true" => new BooleanLiteralToken(source, before, after, true),
                "false" => new BooleanLiteralToken(source, before, after, false),

                "typedef" => new TypedefKeyword(source, before, after),
                "enum" => new EnumKeyword(source, before, after),

                _ => new Identifier(source, before, after, text)
            };
        } else {
            return null;
        }
    }
}

public class Identifier : TextToken {
    public Identifier(File source, Position startsAt, Position endsAt, string text) : base(source, startsAt, endsAt, text) {}
}

public class BooleanLiteralToken : ValueToken<bool> {
    public BooleanLiteralToken(File source, Position startsAt, Position endsAt, bool value) : base(source, startsAt, endsAt, value){}
}

public class IntegerLiteralToken : ValueToken<int> {
    public IntegerLiteralToken(File source, Position startsAt, Position endsAt, int value) : base(source, startsAt, endsAt, value){}
}

public class UIntegerLiteralToken : ValueToken<uint> {
    public UIntegerLiteralToken(File source, Position startsAt, Position endsAt, uint value) : base(source, startsAt, endsAt, value){}
}

public class FloatLiteralToken : ValueToken<float> {
    public FloatLiteralToken(File source, Position startsAt, Position endsAt, float value) : base(source, startsAt, endsAt, value){}
}