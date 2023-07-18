namespace Qkmaxware.Languages.C;

/// <summary>
/// The void or nothing type
/// </summary>
public class Void : TypeSpecifier {
    private static Void? _instance;
    public static Void Instance {
        get {
            if (_instance == null)
                _instance = new Void();
            return _instance;
        }
    }

    private Void() {}

    public override bool Equals(TypeSpecifier? other) => other is Void;
}