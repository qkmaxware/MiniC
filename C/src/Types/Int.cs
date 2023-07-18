namespace Qkmaxware.Languages.C;

/// <summary>
/// The integer type
/// </summary>
public class Integer : ValueTypeSpecifier {
    private static Integer? _instance;
    public static Integer Instance {
        get {
            if (_instance == null)
                _instance = new Integer();
            return _instance;
        }
    }

    private Integer() {
        this.Operators.Addition.Add(new AddIntInt(this));
        this.Operators.Subtraction.Add(new SubIntInt(this));
        this.Operators.Multiplication.Add(new MulIntInt(this));
        this.Operators.Division.Add(new DivIntInt(this));
        this.Operators.Remainder.Add(new RemIntInt(this));

        this.Operators.LessThan.Add(new LtIntInt(this));
        this.Operators.GreaterThan.Add(new GtIntInt(this));
        this.Operators.Equality.Add(new EqIntInt(this));
        this.Operators.Inequality.Add(new NeqIntInt(this));

        this.Operators.And.Add(new AndIntInt(this));
        this.Operators.Or.Add(new OrIntInt(this));
        this.Operators.Xor.Add(new XorIntInt(this));
    }

    public override bool Equals(TypeSpecifier? other) => other is Integer;
}