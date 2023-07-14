namespace Qkmaxware.Languages.C;

public interface IOperatorVisitor {
    public void Accept(AddIntInt op);
    public void Accept(AddUIntUInt op);
    public void Accept(AddFloatFloat op);

    public void Accept(SubIntInt op);
    public void Accept(SubUIntUInt op);
    public void Accept(SubFloatFloat op);

    public void Accept(MulIntInt op);
    public void Accept(MulUIntUInt op);
    public void Accept(MulFloatFloat op);

    public void Accept(DivIntInt op);
    public void Accept(DivUIntUInt op);
    public void Accept(DivFloatFloat op);

    public void Accept(RemIntInt op);
    public void Accept(RemUIntUInt op);
    public void Accept(RemFloatFloat op);

    public void Accept(GtIntInt op);
    public void Accept(GtUIntUInt op);
    public void Accept(GtFloatFloat op);

    public void Accept(LtIntInt op);
    public void Accept(LtUIntUInt op);
    public void Accept(LtFloatFloat op);

    public void Accept(EqIntInt op);
    public void Accept(EqUIntUInt op);
    public void Accept(EqFloatFloat op);
    public void Accept(EqCharChar op);
    
    public void Accept(NeqIntInt op);
    public void Accept(NeqUIntUInt op);
    public void Accept(NeqFloatFloat op);
    public void Accept(NeqCharChar op);

    public void Accept(AndIntInt op);
    public void Accept(AndUIntUInt op);

    public void Accept(OrIntInt op);
    public void Accept(OrUIntUInt op); 

    public void Accept(XorIntInt op);
    public void Accept(XorUIntUInt op);
}