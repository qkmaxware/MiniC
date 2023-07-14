using Qkmaxware.Vm;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Statement for conditionals
/// </summary>
public class IfStatement : Statement {

    public List<ConditionalBranch> Branches {get; private set;} = new List<ConditionalBranch>();

    public IfStatement(ConditionalBranch branch) {
        this.Branches.Add(branch);
    }

    public IfStatement(Expression cond, CompoundStatement branch) {
        this.Branches.Add(new ConditionalBranch(cond, branch));
    }

    public IfStatement ElseIf(ConditionalBranch branch) {
        this.Branches.Add(branch);
        return this;
    }
    public IfStatement ElseIf(Expression cond, CompoundStatement branch) {
        this.Branches.Add(new ConditionalBranch(cond, branch));
        return this;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}

public class ConditionalBranch {
    public Expression Condition {get; private set;}
    public CompoundStatement Branch {get; private set;}

    public ConditionalBranch(Expression condition, CompoundStatement branch) {
        this.Condition = condition;
        this.Branch = branch;
    }
}