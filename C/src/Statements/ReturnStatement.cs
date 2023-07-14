namespace Qkmaxware.Languages.C;

/// <summary>
/// A return statement
/// </summary>
public class ReturnStatement : Statement { 
    public Expression? ReturnedValue {get; private set;}

    public ReturnStatement() {
        this.ReturnedValue = null;
    }

    public ReturnStatement(Expression returnValue) {
        this.ReturnedValue = returnValue;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}

