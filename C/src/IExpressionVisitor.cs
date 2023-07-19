namespace Qkmaxware.Languages.C;

public interface IExpressionVisitor {

    public void Accept(LoadVarExpression expr);
    public void Accept(LoadArrayElementExpression expr);
    public void Accept(LoadEnumConstant expr);
    public void Accept(LoadStructFieldExpression expr);
    public void Accept(NewArrayExpression expr);
    public void Accept(NewStructExpression expr);
    public void Accept(LiteralIntExpression expr);
    public void Accept(LiteralUIntExpression expr);
    public void Accept(LiteralFloatExpression expr);
    public void Accept(LiteralStringExpression expr);

    public void Accept(OrExpression expr);
    public void Accept(AndExpression expr);
    public void Accept(XorExpression expr);

    public void Accept(AddExpression expr);
    public void Accept(SubExpression expr);
    public void Accept(MulExpression expr);
    public void Accept(DivExpression expr);
    public void Accept(RemExpression expr);

    public void Accept(GtExpression expr);
    public void Accept(LtExpression expr);
    public void Accept(EqExpression expr);
    public void Accept(NeqExpression expr);

    public void Accept(CallExpression expr);

    public void Accept(LengthExpression expr);
    public void Accept(SizeOfExpression expr);
    public void Accept(FreeExpression expr);
}