namespace Qkmaxware.Languages.C;

public class CallExpression : Expression {
    public FunctionDeclaration Function {get; private set;}
    public List<Expression> Arguments {get; private set;} = new List<Expression>();
    
    public CallExpression(FunctionDeclaration funcdef, params Expression[] args) {
        this.Function = funcdef;
        this.Arguments.AddRange(args);
    }

    public override void Visit(IExpressionVisitor visitor) => visitor.Accept(this);
}