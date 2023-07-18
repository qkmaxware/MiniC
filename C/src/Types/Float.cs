namespace Qkmaxware.Languages.C;

/// <summary>
/// The floating point value type
/// </summary>
public class Float : ValueTypeSpecifier {
    private static Float? _instance;
    public static Float Instance {
        get {
            if (_instance == null)
                _instance = new Float();
            return _instance;
        }
    }

    private Float() {
        this.Operators.Addition.Add(new AddFloatFloat(this));
        this.Operators.Subtraction.Add(new SubFloatFloat(this));
        this.Operators.Multiplication.Add(new MulFloatFloat(this));
        this.Operators.Division.Add(new DivFloatFloat(this));
        this.Operators.Remainder.Add(new RemFloatFloat(this));

        this.Operators.LessThan.Add(new LtFloatFloat(this));
        this.Operators.GreaterThan.Add(new GtFloatFloat(this));
        this.Operators.Equality.Add(new EqFloatFloat(this));
        this.Operators.Inequality.Add(new NeqFloatFloat(this));
    }

    public override bool Equals(TypeSpecifier? other) => other is Float;
}