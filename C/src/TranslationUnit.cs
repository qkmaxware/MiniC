using System.Collections;

namespace Qkmaxware.Languages.C;

/// <summary>
/// A single translation unit
/// </summary>
public class TranslationUnit : IEnumerable<GlobalDeclaration> {
    /// <summary>
    /// Root namespace for the translation unit
    /// </summary>
    /// <returns>namespace</returns>
    public Namespace Namespace {get; private set;} = new Namespace();

    private List<GlobalDeclaration> declarations = new List<GlobalDeclaration>();
    private int static_pool_count = 0;

    public IEnumerator<GlobalDeclaration> GetEnumerator() => declarations.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => declarations.GetEnumerator();

    public void AddDeclaration(GlobalDeclaration declaration) {
        if (declaration is FunctionDeclaration funcl && funcl.Name.Value == "main" && funcl.FormalArguments.Count == 0) {
            this.Main = funcl;
        }
        if (declaration is StaticVariableDeclaration vardecl) {
            vardecl.StaticIndex = static_pool_count;
            static_pool_count++;
        }
        this.declarations.Add(declaration);
    }

    /// <summary>
    /// Check if this translation unit is executable
    /// </summary>
    public bool IsExecutable => Main != null;
    /// <summary>
    /// Main function 
    /// </summary>
    /// <value>main function</value>
    public FunctionDeclaration? Main {get; private set;}

    /// <summary>
    /// Validate a that a module is created correctly
    /// </summary>
    public void Validate() {
        // Passes go here
        new Validation.AnalyzeExpressionTypes().Validate(this);     // Compute the types for all expressions
        new Validation.ValidateTypes().Validate(this);              // Validate the types for expressions/operators/statements
        new Validation.ValidateLoopControlFlow().Validate(this);    // Validate that all break/control are inside a loop
    }
}
