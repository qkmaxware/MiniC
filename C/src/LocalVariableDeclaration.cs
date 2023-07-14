namespace Qkmaxware.Languages.C;

public class LocalVariableDeclaration : InternalDeclaration, IVariableDeclaration {
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
    /// <summary>
    /// Scope local is declared in
    /// </summary>
    public FunctionDeclaration Scope {get; private set;}
    /// <summary>
    /// Index of the local in the function/namespace 
    /// </summary>
    public int LocalIndex => Scope.Locals.IndexOf(this);

    internal LocalVariableDeclaration(FunctionDeclaration scope, Name name, TypeSpecifier type) {
        this.Scope = scope;
        this.Name = name;
        this.Type = type;
    }

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}
