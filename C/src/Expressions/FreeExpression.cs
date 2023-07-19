namespace Qkmaxware.Languages.C;

/// <summary>
/// Memory freeing expression
/// </summary>
public class FreeExpression : Expression {
    public Expression Loader {get; private set;}
    public FreeExpression(Expression var) : base() {
        this.Loader = var;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}