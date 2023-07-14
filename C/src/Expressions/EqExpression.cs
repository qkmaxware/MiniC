namespace Qkmaxware.Languages.C;

/// <summary>
/// Equality expression
/// </summary>
public class EqExpression : BinaryExpression {
    public EqExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}