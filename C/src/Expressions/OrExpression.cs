namespace Qkmaxware.Languages.C;

/// <summary>
/// Or expression
/// </summary>
public class OrExpression : BinaryExpression {
    public OrExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}