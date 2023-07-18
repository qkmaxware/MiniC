namespace Qkmaxware.Languages.C;

/// <summary>
/// Base class for types
/// </summary>
public abstract class TypeSpecifier : IEquatable<TypeSpecifier> {
    public OperatorSet Operators {get; private set;} = new OperatorSet();

    public abstract bool Equals(TypeSpecifier? other);

    public override string ToString() => this.GetType().Name;
}

/// <summary>
/// Base class for types that have associated values
/// </summary>
public abstract class ValueTypeSpecifier : TypeSpecifier {}

/// <summary>
/// Base class for types that are defined by the user
/// </summary>
public abstract class UserDefinedType : ValueTypeSpecifier {
    /// <summary>
    /// Name of the type
    /// </summary>
    /// <value></value>
    public Name Name {get; private set;}
    
    public UserDefinedType(Name name) {
                    this.Name = name;
    }
}

/// <summary>
/// Interface indicating that a type is a pointer to a value on the heap
/// </summary>
public interface IPointerType {}


