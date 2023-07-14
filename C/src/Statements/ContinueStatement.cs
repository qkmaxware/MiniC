namespace Qkmaxware.Languages.C;

/// <summary>
/// A continue statement
/// </summary>
public class ContinueStatement : Statement { 

    public ContinueStatement() {}

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}

