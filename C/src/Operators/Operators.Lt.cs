namespace Qkmaxware.Languages.C;

public abstract class LtOperator : BinaryOperator {
    public LtOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class LtIntInt : LtOperator {
    public LtIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class LtUIntUInt : LtOperator {
    public LtUIntUInt(TypeSpecifier uintType) : base(Integer.Instance, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class LtFloatFloat : LtOperator {
    public LtFloatFloat(TypeSpecifier floatType) : base(Integer.Instance, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}
