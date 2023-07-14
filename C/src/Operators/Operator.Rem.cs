namespace Qkmaxware.Languages.C;

public abstract class RemOperator : BinaryOperator {
    public RemOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class RemIntInt : RemOperator {
    public RemIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class RemUIntUInt : RemOperator {
    public RemUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class RemFloatFloat : RemOperator {
    public RemFloatFloat(TypeSpecifier floatType) : base(floatType, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}