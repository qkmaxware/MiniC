namespace Qkmaxware.Languages.C;

/// <summary>
/// An enum declaration
/// </summary>
public class EnumDeclaration : GlobalDeclaration {

    public Enumeration Type {get; private set;}
    
    public EnumDeclaration(Enumeration type) : base(type.Name) {
        this.Type = type;
    }

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}