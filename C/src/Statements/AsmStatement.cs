using Qkmaxware.Vm;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Statement for inline assembly
/// </summary>
public class AsmStatement : Statement {
    /// <summary>
    /// Inline assembly
    /// </summary>
    public string Assembly {get; private set;}

    public AsmStatement(string assembly) {
        this.Assembly = assembly;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}