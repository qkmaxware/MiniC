namespace Qkmaxware.Languages.C;

public abstract class EqOperator : BinaryOperator {
    public EqOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class EqIntInt : EqOperator {
    public EqIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class EqUIntUInt : EqOperator {
    public EqUIntUInt(TypeSpecifier uintType) : base(Integer.Instance, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class EqFloatFloat : EqOperator {
    public EqFloatFloat(TypeSpecifier floatType) : base(Integer.Instance, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class EqCharChar : EqOperator {
    public EqCharChar(TypeSpecifier floatType) : base(Integer.Instance, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}
