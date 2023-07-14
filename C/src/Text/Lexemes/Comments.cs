using System.Text;
using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

namespace Qkmaxware.Languages.C.Text;

public class LineCommentLexer : Lexeme {
    public override Token? Read(Scanner s) {
        var first = s.Peek(0);
        if (!first.HasValue || !(first.Value.Character == '/')) {
            return null;
        } 
        var second = s.Peek(1);
        if (!second.HasValue || !(second.Value.Character == '/')) {
            return null;
        } 
        var file = first.Value.Source;
        var before = first.Value.Position;
        s.Read();
        s.Read();

        var text = s.ReadUntil(x => x == '\n');

        return new CommentToken(file, before, before, text);
    }
}

public class BlockCommentLexer : Lexeme {
    public override Token? Read(Scanner s) {
        var first = s.Peek(0);
        if (!first.HasValue || !(first.Value.Character == '/')) {
            return null;
        } 
        var second = s.Peek(1);
        if (!second.HasValue || !(second.Value.Character == '*')) {
            return null;
        } 
        var file = first.Value.Source;
        var before = first.Value.Position;
        var after = before;
        s.Read();
        s.Read();

        var content = new StringBuilder();
        while ((first = s.Peek(0)).HasValue && (second = s.Peek(1)).HasValue && (first.Value.Character != '*' && second.Value.Character != '/')) {
            content.Append(first.Value.Character);
            s.Read();
            after = first.Value.Position;
        }
        s.Read();
        s.Read();

        return new CommentToken(file, before, before, content.ToString());
    }
}


public class CommentToken : TextToken {
    public CommentToken(File source, Position startsAt, Position endsAt, string text) : base(source, startsAt, endsAt, text) {}
}
