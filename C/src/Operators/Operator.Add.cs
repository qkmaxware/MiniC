namespace Qkmaxware.Languages.C;

public abstract class AddOperator : BinaryOperator {
    public AddOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class AddIntInt : AddOperator {
    public AddIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class AddUIntUInt : AddOperator {
    public AddUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class AddFloatFloat : AddOperator {
    public AddFloatFloat(TypeSpecifier floatType) : base(floatType, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}