namespace Qkmaxware.Languages.C;

public abstract class MulOperator : BinaryOperator {
    public MulOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class MulIntInt : MulOperator {
    public MulIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class MulUIntUInt : MulOperator {
    public MulUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class MulFloatFloat : MulOperator {
    public MulFloatFloat(TypeSpecifier floatType) : base(floatType, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}