using Qkmaxware.Vm;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Statement for raw expressions
/// </summary>
public class ExprStatement : Statement {
    /// <summary>
    /// Expression to run
    /// </summary>
    public Expression Expression {get; private set;}

    public ExprStatement(Expression expr) {
        this.Expression = expr;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}