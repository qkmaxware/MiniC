namespace Qkmaxware.Languages.C;

/// <summary>
/// Base class for types
/// </summary>
public abstract class TypeSpecifier : IEquatable<TypeSpecifier> {
    public OperatorSet Operators {get; private set;} = new OperatorSet();

    public abstract bool Equals(TypeSpecifier? other);

    public override string ToString() => this.GetType().Name;
}

public abstract class ValueTypeSpecifier : TypeSpecifier {}

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

public interface IPointerType {}

/// <summary>
/// An array of values
/// </summary>
public class Array : ValueTypeSpecifier, IPointerType {
    /// <summary>
    /// Type specification of the array elements
    /// </summary>
    public ValueTypeSpecifier ElementType {get; private set;}

    public static readonly Array StringType = new Array(Char.Instance);

    public Array(ValueTypeSpecifier elem) {
        this.ElementType = elem;
    }

    public static Array Of(ValueTypeSpecifier elementType) {
        return new Array(elementType);
    }

    public override bool Equals(TypeSpecifier? other) {
        return other is Array array && array.ElementType.Equals(this.ElementType);
    }
    public override string ToString() => ElementType.ToString() + "[]";
}

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