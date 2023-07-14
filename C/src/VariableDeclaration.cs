namespace Qkmaxware.Languages.C;

public interface IVariableDeclaration {
    /// <summary>
    /// Type
    /// </summary>
    /// <value>type specifier</value>
    public TypeSpecifier Type {get;}
    /// <summary>
    /// The name of the variable
    /// </summary>
    /// <value>name</value>
    public Name Name {get;}
}
