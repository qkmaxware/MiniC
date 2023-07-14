namespace Qkmaxware.Languages.C;

/// <summary>
/// Subtract expression
/// </summary>
public class SubExpression : BinaryExpression {
    public SubExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}