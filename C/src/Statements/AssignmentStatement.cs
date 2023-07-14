using Qkmaxware.Vm;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Statement for local assignment
/// </summary>
public class AssignmentStatement : Statement {
    /// <summary>
    /// Variable to assign to
    /// </summary>
    public IVariableDeclaration Variable {get; private set;}
    /// <summary>
    /// Value to assign to the variable
    /// </summary>
    public Expression Value {get; private set;}

    public AssignmentStatement(IVariableDeclaration decl, Expression value) {
        this.Variable = decl;
        this.Value = value;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}

/// <summary>
/// Statement for local assignment of array elements
/// </summary>
public class ArrayAssignmentStatement : Statement {
    /// <summary>
    /// Variable to assign to
    /// </summary>
    public IVariableDeclaration Variable {get; private set;}

    /// <summary>
    /// The value to use as an array index
    /// </summary>
    public Expression Index {get; private set;}

    /// <summary>
    /// Value to assign to the variable
    /// </summary>
    public Expression Value {get; private set;}

    public ArrayAssignmentStatement(IVariableDeclaration decl, Expression index, Expression value) {
        this.Variable = decl;
        this.Index = index;
        this.Value = value;
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);
}