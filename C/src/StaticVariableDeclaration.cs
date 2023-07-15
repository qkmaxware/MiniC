namespace Qkmaxware.Languages.C;

public class StaticVariableDeclaration : GlobalDeclaration, IVariableDeclaration {
    /// <summary>
    /// Type
    /// </summary>
    /// <value>type specifier</value>
    public TypeSpecifier Type {get; set;}

    /// <summary>
    /// Index in the static pool for this variable
    /// </summary>
    public int StaticIndex {get; internal set;}

    public StaticVariableDeclaration(Name name, TypeSpecifier type) : base(name) {
        this.Type = type;
    }

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}
