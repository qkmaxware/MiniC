namespace Qkmaxware.Languages.C;

/// <summary>
/// A function declaration
/// </summary>
public class FunctionDeclaration : GlobalDeclaration {
    /// <summary>
    /// A namespace for function arguments and locals
    /// </summary>
    /// <returns>namespace</returns>
    public Namespace Namespace {get; private set;} = new Namespace();

    private List<LocalVariableDeclaration> locals = new List<LocalVariableDeclaration>();

    public FunctionDeclaration(Name name) : base(name) {
        this.Body = new CompoundStatement();
    }

    public FunctionDeclaration(Name name, CompoundStatement body) : base(name) {
        this.Body = body;
    }

    public FunctionDeclaration(TypeSpecifier returnType, Name name, FormalArgument[] args, CompoundStatement body) : base(name) {
        this.Body = body;
        this.ReturnType = returnType;
        this.FormalArguments.AddRange(args);
    }

    /// <summary>
    /// Return type
    /// </summary>
    /// <value>type specifier</value>
    public TypeSpecifier ReturnType {get; set;} = Void.Instance;
    
    /// <summary>
    /// List of function arguments
    /// </summary>
    /// <typeparam name="Argument">argument</typeparam>
    /// <returns>list of arguments</returns>
    public List<FormalArgument> FormalArguments {get; private set;} = new List<FormalArgument>();

    /// <summary>
    /// The body of the function
    /// </summary>
    /// <value>statement</value>
    public CompoundStatement Body {get; set;}

    /// <summary>
    /// Function local variables
    /// </summary>
    /// <returns>variables</returns>
    public List<LocalVariableDeclaration> Locals {get; private set;} = new List<LocalVariableDeclaration>();

    public FormalArgument MakeArg(string name, TypeSpecifier type) {
        var local = new FormalArgument(this, new Name(this.Namespace, name), type);
        FormalArguments.Add(local);
        return local;
    }

    public LocalVariableDeclaration MakeLocal(string name, TypeSpecifier type) {
        var local = new LocalVariableDeclaration(this, new Name(this.Namespace, name), type);
        Locals.Add(local);
        return local;
    }

    public override string ToString() => $"{ReturnType} {Name} ({string.Join(',', FormalArguments)});";

    public override void Visit(IDeclarationVisitor visitor) => visitor.Accept(this);
}
