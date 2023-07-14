namespace Qkmaxware.Languages.C;

/// <summary>
/// Array length expression
/// </summary>
public class LengthExpression : Expression {
    public LoadVarExpression Loader {get; private set;}
    public LengthExpression(IVariableDeclaration var) : base() {
        this.Loader = new LoadVarExpression(var);
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}

/// <summary>
/// Heap size expression
/// </summary>
public class SizeOfExpression : Expression {
    public LoadVarExpression Loader {get; private set;}
    public SizeOfExpression(IVariableDeclaration var) : base() {
        this.Loader = new LoadVarExpression(var);
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}
