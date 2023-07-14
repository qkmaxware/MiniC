namespace Qkmaxware.Languages.C;

/// <summary>
/// Greater than expression
/// </summary>
public class GtExpression : BinaryExpression {
    public GtExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}