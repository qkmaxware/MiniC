namespace Qkmaxware.Languages.C;

public abstract class SubOperator : BinaryOperator {
    public SubOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class SubIntInt : SubOperator {
    public SubIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class SubUIntUInt : SubOperator {
    public SubUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class SubFloatFloat : SubOperator {
    public SubFloatFloat(TypeSpecifier floatType) : base(floatType, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}