namespace Qkmaxware.Languages.C.Validation;

public class ValidateLoopControlFlow : BaseValidationPass, IDeclarationVisitor, IStatementVisitor {
    public void Accept(AsmStatement stmt) {}

    public void Accept(AssignmentStatement stmt) { }

    public void Accept(ArrayAssignmentStatement stmt) {}

    public void Accept(CompoundStatement stmt) {
        foreach (var s in stmt)
            s.Visit(this);
    }

    public void Accept(IfStatement stmt) {
        foreach (var b in stmt.Branches) {
            b.Branch.Visit(this);
        }
    }

    public void Accept(ReturnStatement stmt) {}

    int nested_loop_depth;
    public void Accept(WhileStatement stmt) {
        nested_loop_depth++;
        stmt.Body.Visit(this);
        nested_loop_depth = Math.Max(0, nested_loop_depth - 1);
    }

    public void Accept(BreakStatement stmt) {
        if (nested_loop_depth == 0) {
            ThrowSemanticError(stmt, "Break statements can only be used within a loop body.");
        }
    }

    public void Accept(ContinueStatement stmt) {
        if (nested_loop_depth == 0) {
            ThrowSemanticError(stmt, "Break statements can only be used within a loop body.");
        }
    }

    public void Accept(ExprStatement stmt) { }

    public void Accept(LocalVariableDeclaration decl) { }

    public void Accept(StaticVariableDeclaration decl) { }

    private FunctionDeclaration? currentFunction;
    public void Accept(FunctionDeclaration decl) {
        currentFunction = decl;
        decl.Body.Visit(this);
        currentFunction = null;
    }

    public override void Validate(TranslationUnit unit) {
        foreach (var decl in unit) {
            decl.Visit(this);
        }
    }
}