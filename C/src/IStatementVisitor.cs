namespace Qkmaxware.Languages.C;

public interface IStatementVisitor {
    public void Accept(AsmStatement stmt);
    public void Accept(AssignmentStatement stmt);
    public void Accept(ArrayAssignmentStatement stmt);
    public void Accept(StructFieldAssignmentStatement stmt);
    public void Accept(CompoundStatement stmt);
    public void Accept(IfStatement stmt);
    public void Accept(ReturnStatement stmt);
    public void Accept(WhileStatement stmt);
    public void Accept(BreakStatement stmt);
    public void Accept(ContinueStatement stmt);
    public void Accept(ExprStatement stmt);
}