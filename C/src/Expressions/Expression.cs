namespace Qkmaxware.Languages.C;

/*
expr	
    → expr OR expr | expr AND expr | expr XOR expr
    → expr EQ expr | expr NE expr 
    → expr LE expr | expr < expr | expr GE expr  | expr > expr
    → expr + expr | expr - expr 
    → expr * expr | expr / expr  | expr % expr
    → ! expr | - expr | + expr
    → ( expr )
    → IDENT | IDENT [ expr ] | IDENT ( args )
    → BOOL_LIT | INT_LIT  | FLOAT_LIT | NEW type_spec [ expr ]
*/

/// <summary>
/// Base class for all expressions
/// </summary>
public abstract class Expression : AstNode {
    public abstract void Visit(IExpressionVisitor visitor);
}

/// <summary>
/// Base class for unary expressions
/// </summary>
public abstract class UnaryExpression : Expression {
    public Expression Operand {get; private set;}
    public UnaryExpression(Expression operand) {
        this.Operand = operand;
    }
}

/// <summary>
/// Base class for binary expressions
/// </summary>
public abstract class BinaryExpression : Expression {
    public Expression LhsOperand {get; private set;}
    public Expression RhsOperand {get; private set;}
    public BinaryExpression(Expression lhs, Expression rhs) {
        this.LhsOperand = lhs;
        this.RhsOperand = rhs;
    }
}