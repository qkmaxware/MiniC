namespace Qkmaxware.Languages.C;

/// <summary>
/// A structure type definition;
/// </summary>
public class StructDeclaration : GlobalDeclaration {
    public StructuredType Type {get; private set;}
    
    public StructDeclaration(StructuredType type) : base(type.Name) {
        this.Type = type;
    }

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}
