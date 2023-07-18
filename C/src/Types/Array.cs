namespace Qkmaxware.Languages.C;

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