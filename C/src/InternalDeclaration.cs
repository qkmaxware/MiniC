namespace Qkmaxware.Languages.C;

public abstract class InternalDeclaration : AstNode {
    public abstract void Visit(IDeclarationVisitor visitor);
}
