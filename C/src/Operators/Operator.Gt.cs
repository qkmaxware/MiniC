namespace Qkmaxware.Languages.C;

public abstract class GtOperator : BinaryOperator {
    public GtOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class GtIntInt : GtOperator {
    public GtIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class GtUIntUInt : GtOperator {
    public GtUIntUInt(TypeSpecifier uintType) : base(Integer.Instance, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class GtFloatFloat : GtOperator {
    public GtFloatFloat(TypeSpecifier floatType) : base(Integer.Instance, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}
