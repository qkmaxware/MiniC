namespace Qkmaxware.Languages.C.Text;

public class OpenParenthesis : Token {
    public OpenParenthesis(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class CloseParenthesis : Token {
    public CloseParenthesis(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class OpenBracket : Token {
    public OpenBracket(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class CloseBracket : Token {
    public CloseBracket(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class OpenBrace : Token {
    public OpenBrace(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class CloseBrace : Token {
    public CloseBrace(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}