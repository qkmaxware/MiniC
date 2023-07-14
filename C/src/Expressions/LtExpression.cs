namespace Qkmaxware.Languages.C;

/// <summary>
/// Less than expression
/// </summary>
public class LtExpression : BinaryExpression {
    public LtExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}