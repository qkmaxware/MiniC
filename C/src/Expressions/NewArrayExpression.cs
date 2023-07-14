
namespace Qkmaxware.Languages.C;

public class NewArrayExpression : Expression {
    public Array Type {get; set;}
    public Expression Size {get; set;}

    public NewArrayExpression(ValueTypeSpecifier elementType, Expression size) {
        this.Type = new Array(elementType);
        this.Size = size;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}