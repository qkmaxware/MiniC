using Qkmaxware.Languages.C.Validation;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Base class for all statements
/// </summary>
public abstract class Statement : AstNode {
    public abstract void Visit(IStatementVisitor visitor);
}

