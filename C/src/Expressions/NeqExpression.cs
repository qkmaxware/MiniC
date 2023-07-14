namespace Qkmaxware.Languages.C;

/// <summary>
/// Inequality expression
/// </summary>
public class NeqExpression : BinaryExpression {
    public NeqExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}