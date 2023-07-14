namespace Qkmaxware.Languages.C;

public abstract class OrOperator : BinaryOperator {
    public OrOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class OrIntInt : OrOperator {
    public OrIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class OrUIntUInt : OrOperator {
    public OrUIntUInt(TypeSpecifier uintType) : base(uintType, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}
