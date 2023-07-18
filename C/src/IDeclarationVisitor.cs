namespace Qkmaxware.Languages.C;

public interface IDeclarationVisitor {
    public void Accept(LocalVariableDeclaration decl);
    public void Accept(StaticVariableDeclaration decl);
    public void Accept(FunctionDeclaration decl);
    public void Accept(EnumDeclaration decl);
}