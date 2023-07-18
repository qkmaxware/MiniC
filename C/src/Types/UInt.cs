namespace Qkmaxware.Languages.C;

/// <summary>
/// The unsigned integer type
/// </summary>
public class UnsignedInteger : ValueTypeSpecifier {
    private static UnsignedInteger? _instance;
    public static UnsignedInteger Instance {
        get {
            if (_instance == null)
                _instance = new UnsignedInteger();
            return _instance;
        }
    }

    private UnsignedInteger() {
        this.Operators.Addition.Add(new AddUIntUInt(this));
        this.Operators.Subtraction.Add(new SubUIntUInt(this));
        this.Operators.Multiplication.Add(new MulUIntUInt(this));
        this.Operators.Division.Add(new DivUIntUInt(this));
        this.Operators.Remainder.Add(new RemUIntUInt(this));

        this.Operators.LessThan.Add(new LtUIntUInt(this));
        this.Operators.GreaterThan.Add(new GtUIntUInt(this));
        this.Operators.Equality.Add(new EqUIntUInt(this));
        this.Operators.Inequality.Add(new NeqUIntUInt(this));

        this.Operators.And.Add(new AndUIntUInt(this));
        this.Operators.Or.Add(new OrUIntUInt(this));
        this.Operators.Xor.Add(new XorUIntUInt(this));
    }

    public override bool Equals(TypeSpecifier? other) => other is UnsignedInteger;
}