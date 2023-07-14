namespace Qkmaxware.Languages.C;

public abstract class DivOperator : BinaryOperator {
    public DivOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class DivIntInt : DivOperator {
    public DivIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class DivUIntUInt : DivOperator {
    public DivUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class DivFloatFloat : DivOperator {
    public DivFloatFloat(TypeSpecifier floatType) : base(floatType, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}