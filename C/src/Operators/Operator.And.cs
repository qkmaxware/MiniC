namespace Qkmaxware.Languages.C;

public abstract class AndOperator : BinaryOperator {
    public AndOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class AndIntInt : AndOperator {
    public AndIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class AndUIntUInt : AndOperator {
    public AndUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}
