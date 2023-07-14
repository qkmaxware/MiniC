namespace Qkmaxware.Languages.C;

public abstract class Operator {
    public TypeSpecifier ResultType {get; private set;}

    public Operator(TypeSpecifier resultType) {
        this.ResultType = resultType;
    }

    public abstract void Visit(IOperatorVisitor visitor);

    public override string ToString() => $"{this.GetType().Name}::() => {ResultType?.GetType()?.Name ?? "null"}";
}

public abstract class BinaryOperator : Operator {
    public TypeSpecifier LhsType {get; private set;}
    public TypeSpecifier RhsType {get; private set;}
    public BinaryOperator(TypeSpecifier resultType, TypeSpecifier lhsType, TypeSpecifier rhsType) : base(resultType) {
        this.LhsType = lhsType;
        this.RhsType = rhsType;
    }

    public override string ToString() => $"{this.GetType().Name}::({LhsType?.GetType()?.Name ?? "null"}, {RhsType?.GetType()?.Name ?? "null"}) => {ResultType?.GetType()?.Name ?? "null"}";
}
