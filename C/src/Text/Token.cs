namespace Qkmaxware.Languages.C.Text;

public abstract class Token {
    public File Source;
    public Position StartPosition;
    public Position EndPosition;
    public long Length => EndPosition.Index - StartPosition.Index;

    public Token(File source, Position startsAt, Position endsAt) {
        this.Source = source;
        this.StartPosition = startsAt;
        this.EndPosition = endsAt;
    }

    public override string ToString() => this.GetType().Name;
}

public class TextToken : Token {
    public string Text {get; private set;}
    public TextToken(File source, Position startsAt, Position endsAt, string text) : base(source, startsAt, endsAt) {
        this.Source = source;
        this.Text = text;
    }

    public override string ToString() => this.GetType().Name + "(" + Text + ")";
}

public class ValueToken<T> : TextToken {
    public T Value {get; private set;}

    public ValueToken(File source, Position startsAt, Position endsAt, T value) : base(source, startsAt, endsAt, value?.ToString() ?? string.Empty) {
        this.Value = value;
    }
    public ValueToken(File source, Position startsAt, Position endsAt, T value, string textual) : base(source, startsAt, endsAt, textual) {
        this.Value = value;
    }

    public override string ToString() => this.GetType().Name + "(" + Value + ")";
}
