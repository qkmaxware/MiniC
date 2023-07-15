namespace Qkmaxware.Languages.C;

public class LocalVariableDeclaration : InternalDeclaration, IVariableDeclaration {
    /// <summary>
    /// Type
    /// </summary>
    /// <value>type specifier</value>
    public TypeSpecifier Type {get; set;}
    /// <summary>
    /// Scope local is declared in
    /// </summary>
    public FunctionDeclaration Scope {get; private set;}
    /// <summary>
    /// Index of the local in the function/namespace 
    /// </summary>
    public int LocalIndex => Scope.Locals.IndexOf(this);

    internal LocalVariableDeclaration(FunctionDeclaration scope, Name name, TypeSpecifier type) : base(name) {
        this.Scope = scope;
        this.Type = type;
    }

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}
