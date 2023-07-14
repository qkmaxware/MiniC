namespace Qkmaxware.Languages.C;

public abstract class LiteralExpression : Expression {}

public class LiteralIntExpression : LiteralExpression {
    public int Value {get; set;}
    public LiteralIntExpression(int value) {
        this.Value = value;
    }
    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}

public class LiteralUIntExpression : LiteralExpression {
    public uint Value {get; set;}
    public LiteralUIntExpression(uint value) {
        this.Value = value;
    }
    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}

public class LiteralFloatExpression : LiteralExpression {
    public float Value {get; set;}
    public LiteralFloatExpression(float value) {
        this.Value = value;
    }
    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}


public class LiteralStringExpression : LiteralExpression {
    public string Value {get; set;}
    public LiteralStringExpression(string value) {
        this.Value = value;
    }
    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}