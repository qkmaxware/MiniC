namespace Qkmaxware.Languages.C.Text;
using Scanner = Qkmaxware.Languages.C.Text.TextPreprocessor;

public class SingleCharLexeme : Lexeme{

    public char Character {get; private set;}
    private Func<File, Position, Position, Token> producer;

    public SingleCharLexeme(char c, Func<File, Position, Position, Token> producer) {
        this.Character = c;
        this.producer = producer;
    }

    public override Token? Read(Scanner s) {
        var next = s.Peek();
        if (!next.HasValue)
            return null;
        if (next.Value.Character != Character)
            return null;
        
        var source = next.Value.Source;
        var before = next.Value.Position;
        s.Read();
        var after = before;
        return producer(source, before, after);
    }
}

public class TwoCharLexeme : Lexeme{

    public char FirstCharacter {get; private set;}
    public char SecondCharacter {get; private set;}
    private Func<File, Position, Position, Token> producer;

    public TwoCharLexeme(char first, char second, Func<File, Position, Position, Token> producer) {
        this.FirstCharacter = first;
        this.SecondCharacter = second;
        this.producer = producer;
    }

    public override Token? Read(Scanner s) {
        var next = s.Peek(0);
        if (!next.HasValue)
            return null;
        if (next.Value.Character != FirstCharacter)
            return null;

        var source = next.Value.Source;
        var before = next.Value.Position;

        next = s.Peek(1);
        if (!next.HasValue)
            return null;
        if (next.Value.Character != SecondCharacter)
            return null;
        var after = next.Value.Position;
        s.Read();
        s.Read();
        return producer(source, before, after);
    }
}

public class AssignmentOperator : Token {
    public AssignmentOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}


public class Semicolon : Token {
    public Semicolon(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class Comma : Token {
    public Comma(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}
}

public class EqualityOperator : Token, IOperator {
    public EqualityOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 3;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new EqExpression(operands[0], operands[1]);
}

public class InequalityOperator : Token, IOperator {
    public InequalityOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 3;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new NeqExpression(operands[0], operands[1]);
}

public class LessThanOperator : Token, IOperator {
    public LessThanOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 4;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new LtExpression(operands[0], operands[1]);
}

public class GreaterThanOperator : Token, IOperator {
    public GreaterThanOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 4;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new GtExpression(operands[0], operands[1]);
}

public class AddOperator : Token, IOperator {
    public AddOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 5;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new AddExpression(operands[0], operands[1]);
}

public class SubtractOperator : Token, IOperator {
    public SubtractOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 5;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new SubExpression(operands[0], operands[1]);
}

public class TimesOperator : Token, IOperator {
    public TimesOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 6;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new MulExpression(operands[0], operands[1]);
}

public class DivideOperator : Token, IOperator {
    public DivideOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 6;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new DivExpression(operands[0], operands[1]);
}

public class RemainderOperator : Token, IOperator {
    public RemainderOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 6;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new RemExpression(operands[0], operands[1]);
}

public class NotOperator : Token, IOperator {
    public NotOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 7;

    public Associativity Associativity => Associativity.Right;

    public int Arity => 1;

    public Expression Combine(Expression[] operands) => new RemExpression(operands[0], operands[1]);
}

public class OrOperator : Token, IOperator {
    public OrOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 2;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new OrExpression(operands[0], operands[1]);
}

public class AndOperator : Token, IOperator {
    public AndOperator(File source, Position startsAt, Position endsAt) : base(source, startsAt, endsAt) {}

    public int Precedence => 2;

    public Associativity Associativity => Associativity.Left;

    public int Arity => 2;

    public Expression Combine(Expression[] operands) => new AndExpression(operands[0], operands[1]);
}
