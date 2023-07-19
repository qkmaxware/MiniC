namespace Qkmaxware.Languages.C;

public class Field {
    public string Name {get; private set;}
    public TypeSpecifier Type {get; private set;}
    public StructuredType MemberOf {get; private set;}

    public Field(StructuredType memberOf, string name, TypeSpecifier type) {
        this.Name = name;
        this.Type = type;
        this.MemberOf = memberOf;
    }
}

public class StructuredType : ValueTypeSpecifier, IPointerType {
    public Name Name {get; private set;}
    private List<Field> _fields = new List<Field>();
    public IEnumerable<Field> Fields => _fields.AsReadOnly();
    
    public StructuredType(Name name) {
        this.Name = name;
    }
    
    public Field MakeField(string name, TypeSpecifier type) {
        var field = new Field(this, name, type);
        _fields.Add(field);
        return field;
    } 

    public int IndexOf(Field field) => _fields.IndexOf(field);
    
    public int FieldCount => _fields.Count;
    
    public override bool Equals(TypeSpecifier? other) {
        return other is StructuredType e && e.Name.Equals(this.Name);
    }

    public override string ToString() => this.GetType().Name + " " + this.Name.Value;
}
