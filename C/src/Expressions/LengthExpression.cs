namespace Qkmaxware.Languages.C;

/// <summary>
/// Array length expression
/// </summary>
public class LengthExpression : Expression {
    public Expression Loader {get; private set;}
    public LengthExpression(Expression var) : base() {
        this.Loader = var;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}

/// <summary>
/// Heap size expression
/// </summary>
public class SizeOfExpression : Expression {
    public Expression Loader {get; private set;}
    public SizeOfExpression(Expression var) : base() {
        this.Loader = var;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}
