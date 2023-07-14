namespace Qkmaxware.Languages.C;

public class StaticVariableDeclaration : InternalDeclaration, IVariableDeclaration {
    /// <summary>
    /// Type
    /// </summary>
    /// <value>type specifier</value>
    public TypeSpecifier Type {get; set;}
    /// <summary>
    /// The name of the variable
    /// </summary>
    /// <value>name</value>
    public Name Name {get; set;}

    public StaticVariableDeclaration(Name name, TypeSpecifier type) {
        this.Name = name;
        this.Type = type;
    }

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}
