namespace Qkmaxware.Languages.C;

/// <summary>
/// And expression
/// </summary>
public class AndExpression : BinaryExpression {
    public AndExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}