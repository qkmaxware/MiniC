namespace Qkmaxware.Languages.C;

/// <summary>
/// Memory freeing expression
/// </summary>
public class FreeExpression : Expression {
    public LoadVarExpression Loader {get; private set;}
    public FreeExpression(IVariableDeclaration var) : base() {
        this.Loader = new LoadVarExpression(var);
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}