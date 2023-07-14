namespace Qkmaxware.Languages.C.Text;

public enum Associativity {
    Right, Left
}

public interface IOperator {
    public int Precedence  {get;}
    public Associativity Associativity {get;}
    public int Arity {get;}

    public Expression Combine(Expression[] operands);
}