namespace Qkmaxware.Languages.C;

/// <summary>
/// Function argument
/// </summary>
public class FormalArgument : IVariableDeclaration {
    /// <summary>
    /// Argument name
    /// </summary>
    /// <value>name</value>
    public Name Name {get; private set;}
    /// <summary>
    /// Type specifier
    /// </summary>
    /// <value>type specifier</value>
    public TypeSpecifier Type {get; private set;}
    /// <summary>
    /// Scope local is declared in
    /// </summary>
    public FunctionDeclaration Scope {get; private set;}
    /// <summary>
    /// Index of the argument in it's method signature
    /// </summary>
    /// <value>index</value>
    public int Index => Scope.FormalArguments.IndexOf(this);
    /// <summary>
    /// Create a new formal argument
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="type">type specifier</param>
    internal FormalArgument(FunctionDeclaration scope, Name name, TypeSpecifier type) {
        this.Scope = scope;
        this.Name = name;
        this.Type = type;
    }

    public override string ToString() => $"{Type} {Name}";
}