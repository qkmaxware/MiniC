namespace Qkmaxware.Languages.C;

/// <summary>
/// A constant definition as a part of an enum
/// </summary>
public class EnumerationConstant {
    /// <summary>
    /// Enumeration type that owns this constant
    /// </summary>
    public Enumeration Enum {get; private set;}

    /// <summary>
    /// Name of the constant
    /// </summary>
    public Name Name {get; private set;}

    /// <summary>
    /// Numeric value of the constant
    /// </summary>
    public int Value {get; private set;}

    public EnumerationConstant(Enumeration @enum, Name name, int value) {
        this.Enum = @enum;
        this.Name = name;
        this.Value = value;
    }
}

/// <summary>
/// A user defined enumeration type
/// </summary>
public class Enumeration : UserDefinedType  {
    private List<EnumerationConstant> _constants;
    public IEnumerable<EnumerationConstant> Constants => _constants.AsReadOnly();
    
    private Enumeration(Name typeName, EnumerationConstant[] constants) : base(typeName) {
        this._constants = new List<EnumerationConstant>(constants);
    }
    
    public static Enumeration Generate(Name name, params (string, int?)[] pairs) {
        int nextIndex = 0;
        EnumerationConstant[] constants = new EnumerationConstant[pairs.Length];
        var e = new Enumeration(name, new EnumerationConstant[0]);

        for (var i = 0; i < pairs.Length; i++) {
            var pair = pairs[i];
            var index = pair.Item2.HasValue ? pair.Item2.Value : nextIndex;
            
            constants[i] = new EnumerationConstant(
                e,
                new Name(name.DeclaredNamespace, pair.Item1),
                index
            );
            
            nextIndex = index + 1;
        }
        e._constants.AddRange(constants);
        
        return e;
    }
    
    public int? ValueOf(string constant) {
        return _constants.Where(c => c.Name.Value == constant).FirstOrDefault()?.Value;
    }

    public override bool Equals(TypeSpecifier? other) {
        return other is Enumeration e && e.Name.Equals(this.Name);
    }

    public override string ToString() => this.GetType().Name + " " + this.Name.Value;
}
