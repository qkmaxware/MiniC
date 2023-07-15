namespace Qkmaxware.Languages.C.Validation;

public class ValidateTypes : BaseValidationPass, IDeclarationVisitor, IStatementVisitor, IExpressionVisitor {
    public void Accept(AsmStatement stmt) {}

    public void Accept(AssignmentStatement stmt) {
        stmt.Value.Visit(this);
        // Get value to save
        var value_type = new AnalyzeExpressionTypes().EvaluateType(stmt.Value); 
        // Validate against variable type
        if (!value_type.Equals(stmt.Variable.Type)) {
            ThrowSemanticError(stmt.Value, $"Type mismatch, cannot assign values of type '{value_type}' to a '{stmt.Variable.Type}' variable.");
        }
    }

    public void Accept(ArrayAssignmentStatement stmt) {
        stmt.Value.Visit(this);
        // Validate against variable type
        if (stmt.Variable.Type is not Array) {
            ThrowSemanticError(stmt.Value, $"Type mismatch, cannot use index assignment on variables of type '{stmt.Variable.Type}'.");
        }
        var array = (Array)(stmt.Variable.Type);
        // Get value to save
        var value_type = new AnalyzeExpressionTypes().EvaluateType(stmt.Value); 
        // Validate against variable type
        if (!value_type.Equals(array.ElementType)) {
            ThrowSemanticError(stmt.Value, $"Type mismatch, cannot assign values of type '{value_type}' to a '{array.ElementType}' array value.");
        }
    }

    public void Accept(CompoundStatement stmt) {
        foreach (var s in stmt)
            s.Visit(this);
    }

    public void Accept(IfStatement stmt) {
        foreach (var b in stmt.Branches) {
            b.Condition.Visit(this);
            b.Branch.Visit(this);
        }
    }

    public void Accept(ReturnStatement stmt) {
        stmt.ReturnedValue?.Visit(this);

        // check return type
        var return_type = object.ReferenceEquals(stmt.ReturnedValue, null) ? Void.Instance : new AnalyzeExpressionTypes().EvaluateType(stmt.ReturnedValue); 

        if (currentFunction != null && !currentFunction.ReturnType.Equals(return_type)) {
            ThrowSemanticError(stmt, $"Type mismatch, cannot return values of type '{return_type}' from a function with return type '{currentFunction.ReturnType}'.");
        }
    }

    public void Accept(WhileStatement stmt) {
        stmt.Condition.Visit(this);
        stmt.Body.Visit(this);
    }

    public void Accept(BreakStatement stmt) {}

    public void Accept(ContinueStatement stmt) {}

    public void Accept(ExprStatement stmt) {
        stmt.Expression.Visit(this);
    }

    public void Accept(LoadVarExpression expr) {}

    public void Accept(LoadArrayElementExpression expr){
        expr.Index.Visit(this);
    }

    public void Accept(NewArrayExpression expr) {
        expr.Size.Visit(this);
    }

    public void Accept(LiteralIntExpression expr) {}

    public void Accept(LiteralUIntExpression expr) {}

    public void Accept(LiteralFloatExpression expr) {}

    public void Accept(LiteralStringExpression expr) {}

    public void Accept(OrExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(AndExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(XorExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(AddExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(SubExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(MulExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(DivExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(RemExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(GtExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(LtExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(EqExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(NeqExpression expr) {
        expr.LhsOperand.Visit(this);
    }

    public void Accept(CallExpression expr) {
        var func = expr.Function;
        if (func.FormalArguments.Count != expr.Arguments.Count) {
            ThrowSemanticError(expr, $"Argument count mismatch, expected '{func.FormalArguments.Count}' but found '{expr.Arguments.Count}'.");
        }
        for (var i = 0; i < func.FormalArguments.Count; i++) {
            // Figure out the type of the argument
            // Evaluate type can throw error
            var argType = new AnalyzeExpressionTypes().EvaluateType(expr.Arguments[i]);
            // Compare
            if (!func.FormalArguments[i].Type.Equals(argType)) {
                ThrowSemanticError(expr.Arguments[i], $"Argument type mismatch, expected '{func.FormalArguments[i].Type}' but found '{argType}'.");
            }
        } 
    }

    public void Accept(LengthExpression expr) {
        expr.Loader.Visit(this);
    }

    public void Accept(SizeOfExpression expr) {
        expr.Loader.Visit(this);
    }

    public void Accept(FreeExpression expr) {
        expr.Loader.Visit(this);
    }

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