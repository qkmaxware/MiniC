namespace Qkmaxware.Languages.C;

/// <summary>
/// A break statement
/// </summary>
public class BreakStatement : Statement { 

    public BreakStatement() {}

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}

