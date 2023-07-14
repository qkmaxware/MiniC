namespace Qkmaxware.Languages.C;

/// <summary>
/// Remainder expression
/// </summary>
public class RemExpression : BinaryExpression {
    public RemExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}