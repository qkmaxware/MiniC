namespace Qkmaxware.Languages.C;

public class LoadVarExpression : Expression {
    public IVariableDeclaration Variable {get; private set;}

    public LoadVarExpression(IVariableDeclaration @var) {
        this.Variable = @var;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}

public class LoadEnumConstant : Expression {
    public Enumeration Type {get; private set;}
    public EnumerationConstant Constant {get; private set;}

    public LoadEnumConstant(Enumeration type, EnumerationConstant constant) {
        this.Type = type;
        this.Constant = constant;
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

public class LoadStructFieldExpression : Expression {
    public IVariableDeclaration Variable {get; private set;}
    public string FieldName {get; private set;}

    public LoadStructFieldExpression(IVariableDeclaration @var, string fieldname) {
        this.Variable = @var;
        this.FieldName = fieldname;
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}