namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// A lazy stream of tokens
/// </summary>
public class TokenStream {
    private IEnumerator<Token> stream;
    private List<Token?> buffer;

    internal TokenStream (IEnumerator<Token> stream) {
        this.stream = stream;
        this.buffer = new List<Token?>();
    }

    private Token? read() {
        while (stream.MoveNext()) {
            var current = stream.Current;
            if (IsTokenSkippable(current)) {
                continue;       // Skip token get the one after this one
            } else {
                return current; // Is not skippable, return this one
            }
        }
        return null;            // Return null at the end of the stream
    }

    public virtual bool IsTokenSkippable(Token t) {
        return t is CommentToken;
    }

    public void EnsureLookaheads(int count) {
        while(buffer.Count < count) {
            buffer.Add(read());
        }
    }

    public bool HasNext() {
        return !object.ReferenceEquals(Peek(0), null);
    }
    public Token? Peek(int i) {
        EnsureLookaheads(i + 1);
        return buffer[i];
    }
    public bool IsLookahead<T>(int i) {
        EnsureLookaheads(i + 1);
        return buffer[i] is T;
    }
    public bool IsLookahead(int i, Predicate<Token?> condition) {
        EnsureLookaheads(i + 1);
        return condition(buffer[i]);
    }

    public Token? Advance() {
        EnsureLookaheads(1);
        var next = buffer[0];
        for (var i = 0; i < buffer.Count - 1; i++) {
            buffer[i] = buffer[i + 1];
        }
        buffer[buffer.Count - 1] = read();
        return next;
    }

    public List<Token> ToList() {
        var lst = new List<Token>();
        while (this.HasNext()) {
            var token = Advance();
            if (token == null)
                break;
            lst.Add(token);
        }
        return lst;
    }
}