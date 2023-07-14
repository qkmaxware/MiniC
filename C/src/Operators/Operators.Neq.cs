namespace Qkmaxware.Languages.C;

public abstract class NeqOperator : BinaryOperator {
    public NeqOperator(TypeSpecifier result, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(result, lhsType, rhsType) {}
}

public class NeqIntInt : NeqOperator {
    public NeqIntInt(TypeSpecifier intType) : base(intType, intType, intType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class NeqUIntUInt : NeqOperator {
    public NeqUIntUInt(TypeSpecifier uintType) : base(Integer.Instance, uintType, uintType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class NeqFloatFloat : NeqOperator {
    public NeqFloatFloat(TypeSpecifier floatType) : base(Integer.Instance, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

public class NeqCharChar : NeqOperator {
    public NeqCharChar(TypeSpecifier floatType) : base(Integer.Instance, floatType, floatType) {}

    public override void Visit(IOperatorVisitor visitor) => visitor.Accept(this);
}

