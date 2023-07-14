namespace Qkmaxware.Languages.C;

/// <summary>
/// Divide expression
/// </summary>
public class DivExpression : BinaryExpression {
    public DivExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}