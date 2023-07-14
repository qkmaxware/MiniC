using Qkmaxware.Vm;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Statement for conditionals
/// </summary>
public class WhileStatement : Statement {

    public Expression Condition {get; private set;}
    public CompoundStatement Body {get; private set;}

    public WhileStatement(Expression cond, CompoundStatement branch) {
        this.Condition = cond;
        this.Body = branch;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}