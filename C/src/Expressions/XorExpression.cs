namespace Qkmaxware.Languages.C;

/// <summary>
/// Xor expression
/// </summary>
public class XorExpression : BinaryExpression {
    public XorExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}