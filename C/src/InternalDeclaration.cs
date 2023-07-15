namespace Qkmaxware.Languages.C;

public abstract class InternalDeclaration : AstNode {
    /// <summary>
    /// The name of the function
    /// </summary>
    /// <value>name</value>
    public Name Name {get; set;}

    public InternalDeclaration(Name name) {
        this.Name = name;
    }

    public abstract void Visit(IDeclarationVisitor visitor);
}

public abstract class GlobalDeclaration : InternalDeclaration {
    public GlobalDeclaration(Name name) : base(name) {}
}