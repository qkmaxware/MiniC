namespace Qkmaxware.Languages.C;

/// <summary>
/// A character type
/// </summary>
public class Char : ValueTypeSpecifier {
    private static Char? _instance;
    public static Char Instance {
        get {
            if (_instance == null)
                _instance = new Char();
            return _instance;
        }
    }

    private Char() {
        this.Operators.Equality.Add(new EqCharChar(this));
        this.Operators.Inequality.Add(new NeqCharChar(this));
    }

    public override bool Equals(TypeSpecifier? other) => other is Char;
}