namespace Qkmaxware.Languages.C;

/// <summary>
/// Multiply expression
/// </summary>
public class MulExpression : BinaryExpression {
    public MulExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}