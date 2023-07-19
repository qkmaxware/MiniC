using Qkmaxware.Languages.C.Text;

namespace Qkmaxware.Languages.C.Validation;

public class AnalyzeExpressionTypes : IDeclarationVisitor, IValidationPass, IStatementVisitor, IExpressionVisitor {

    /// <summary>
    /// Validate this module against this validation pass
    /// </summary>
    /// <param name="unit">translation unit to validate</param>
    public void Validate(TranslationUnit unit) {
        foreach (var decl in unit) {
            decl.Visit(this);
        }
    }

    protected void ThrowSemanticError(AstNode on, string Message) {
        File? src; Position? pos;
        if (on.TryGetTag<File>(out src) && on.TryGetTag<Position>(out pos)) {
            throw new SematicException(src, pos, Message);
        } else {
            throw new SematicException(Message);
        }
    }

    public void Accept(LocalVariableDeclaration decl) { }

    public void Accept(StaticVariableDeclaration decl) { }

    public void Accept(EnumDeclaration decl) { }
    public void Accept(StructDeclaration decl) {}

    public void Accept(FunctionDeclaration decl) {
        decl.Body.Visit(this);
    }

    private TypeSpecifier type = Void.Instance;

    public TypeSpecifier EvaluateType(Expression expr) {
        expr.Visit(this);
        return type;
    }

    public void Accept(AsmStatement stmt) {}

    public void Accept(AssignmentStatement stmt) {
        stmt.Value.Visit(this);
    }

    public void Accept(ArrayAssignmentStatement stmt) {
        stmt.Value.Visit(this);
    }
    public void Accept(StructFieldAssignmentStatement stmt) {
        var structType = stmt.Variable.Type;
        if (structType is not StructuredType) {
            ThrowSemanticError(stmt, $"Cannot dereference a field from something of type '{structType}'.");
        }
        var field = ((StructuredType)structType).Fields.Where(field => field.Name == stmt.FieldName).FirstOrDefault();
        if (field == null) {
            ThrowSemanticError(stmt, $"Structured type '{structType}' doesn't have a field named '{stmt.FieldName}'.");
        }
        
        stmt.Value.Visit(this);
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

    public void Accept(LoadVarExpression expr) {
        type = expr.Variable.Type;
    }
    public void Accept(LoadArrayElementExpression expr) {
        if (expr.Variable.Type is not Array arrayType)
            ThrowSemanticError(expr, "Cannot index a non-array variable.");
        expr.Index.Visit(this);
        if (!type.Equals(Integer.Instance) && !type.Equals(UnsignedInteger.Instance)) {
            ThrowSemanticError(expr.Index, $"Type of array index must evaluate to an integer and cannot be of type '{type}'.");
        }
        type = ((Array)expr.Variable.Type).ElementType;
    }
    public void Accept(LoadEnumConstant expr) {
        type = expr.Type;
    }
    public void Accept(LoadStructFieldExpression expr) {
        var structType = expr.Variable.Type;
        if (structType is not StructuredType) {
            ThrowSemanticError(expr, $"Cannot dereference a field from something of type '{structType}'.");
        }
        var field = ((StructuredType)structType).Fields.Where(field => field.Name == expr.FieldName).FirstOrDefault();
        if (field == null) {
            ThrowSemanticError(expr, $"Structured type '{structType}' doesn't have a field named '{expr.FieldName}'.");
        } else {
            type = field.Type;
        }
    }
    public void Accept(NewArrayExpression expr) {
        expr.Size.Visit(this);
        if (!type.Equals(Integer.Instance) && !type.Equals(UnsignedInteger.Instance)) {
            ThrowSemanticError(expr.Size, $"Type of array size must evaluate to an integer and cannot be of type '{type}'.");
        }
        type = expr.Type;
    }
    public void Accept(NewStructExpression expr) {
        type = expr.Type;
    }
    public void Accept(LiteralIntExpression expr) => type = Integer.Instance;
    public void Accept(LiteralUIntExpression expr) => type = UnsignedInteger.Instance;
    public void Accept(LiteralFloatExpression expr) => type = Float.Instance;
    public void Accept(LiteralStringExpression expr) => type = Array.StringType;


    private void validateBinaryExpression(BinaryExpression expr, string name, Func<TypeSpecifier, IEnumerable<BinaryOperator>> opset) {
        TypeSpecifier? resultType;
        if (expr.TryGetTag<TypeSpecifier>(out resultType)) {
            type = resultType;
            return;
        }

        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = type;
        expr.RhsOperand.Visit(this);
        var rhsType = type;
        
        // Get the operator
        var op = opset(lhsType)?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            ThrowSemanticError(expr, $"The {name} operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        } else {
            // Store the result type
            resultType = op.ResultType;
            type = resultType;
            return;
        }
    }

    public void Accept(OrExpression expr) => validateBinaryExpression(expr, "or", (type) => type.Operators.Or);
    public void Accept(AndExpression expr) => validateBinaryExpression(expr, "and", (type) => type.Operators.And);
    public void Accept(XorExpression expr) => validateBinaryExpression(expr, "xor", (type) => type.Operators.And);
    public void Accept(AddExpression expr) => validateBinaryExpression(expr, "+", (type) => type.Operators.Addition);
    public void Accept(SubExpression expr) => validateBinaryExpression(expr, "-", (type) => type.Operators.Subtraction);
    public void Accept(MulExpression expr) => validateBinaryExpression(expr, "*", (type) => type.Operators.Multiplication);
    public void Accept(DivExpression expr) => validateBinaryExpression(expr, "/", (type) => type.Operators.Division);
    public void Accept(RemExpression expr) => validateBinaryExpression(expr, "%", (type) => type.Operators.Remainder);
    public void Accept(GtExpression expr) => validateBinaryExpression(expr, ">", (type) => type.Operators.GreaterThan);
    public void Accept(LtExpression expr) => validateBinaryExpression(expr, "<", (type) => type.Operators.LessThan);
    public void Accept(EqExpression expr) => validateBinaryExpression(expr, "==", (type) => type.Operators.Equality);
    public void Accept(NeqExpression expr) => validateBinaryExpression(expr, "!=", (type) => type.Operators.Inequality);

    public void Accept(CallExpression expr) => type = expr.Function.ReturnType;

    public void Accept(LengthExpression expr) => type = Integer.Instance;

    public void Accept(SizeOfExpression expr) => type = Integer.Instance;

    public void Accept(FreeExpression expr) {
        expr.Loader.Visit(this);  
    }

}