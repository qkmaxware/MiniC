namespace Qkmaxware.Languages.C;

public class LoadVarExpression : Expression {
    public IVariableDeclaration Variable {get; private set;}

    public LoadVarExpression(IVariableDeclaration @var) {
        this.Variable = @var;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}

public class LoadArrayElementExpression : Expression {
    public IVariableDeclaration Variable {get; private set;}
    public Expression Index {get; private set;}

    public LoadArrayElementExpression(IVariableDeclaration @var, Expression index) {
        this.Variable = @var;
        this.Index = index;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}