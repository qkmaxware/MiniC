namespace Qkmaxware.Languages.C;

/// <summary>
/// Add expression
/// </summary>
public class AddExpression : BinaryExpression {
    public AddExpression(Expression lhs, Expression rhs) : base(lhs, rhs) { }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}