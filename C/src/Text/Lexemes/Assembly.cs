using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

namespace Qkmaxware.Languages.C.Text;

public class AssemblyLexeme : Lexeme {
    public override Token? Read(Scanner scanner) {
        var a = scanner.Peek(0);
        var s = scanner.Peek(1);
        var m = scanner.Peek(2);
        var _ = scanner.Peek(3);

        if (!a.HasValue || a.Value.Character != 'a')
            return null;
        if (!s.HasValue || s.Value.Character != 's')
            return null;
        if (!m.HasValue || m.Value.Character != 'm')
            return null;
        if (!_.HasValue || char.IsLetterOrDigit(_.Value.Character))
            return null; // Is an identifier, not assembly
        
        // Read ASM
        scanner.Read(); scanner.Read(); scanner.Read();
        scanner.SkipWhitespace();

        // Read {
        var open = scanner.Peek();
        if (!open.HasValue || open.Value.Character != '{') {
            var position = open.HasValue ? open.Value : m.Value;
            throw new LexicalException(position.Source, position.Position, "Assembly blocks should immediately be followed by a block of assembly code surrounded in curly braces {}.");
        }
        scanner.Read();

        // Read until }
        var asm = scanner.ReadUntil((c) => c == '}');
        scanner.Read();

        return new AssemblyToken(a.Value.Source, a.Value.Position, a.Value.Position, asm);
    }
}

public class AssemblyToken : TextToken {
    public AssemblyToken(File source, Position startsAt, Position endsAt, string text) : base(source, startsAt, endsAt, text) {}

    public override string ToString() {
        var text = Text.Replace('\n', ' ').Replace('\r', ' ').Trim();
        if (text.Length > 17) {
            text = text.Substring(0, 17)+"...";
        }
        return this.GetType().Name + "(" + text + ")";
    }
}
