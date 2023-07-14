using System.Collections;

namespace Qkmaxware.Languages.C;

/// <summary>
/// A single translation unit
/// </summary>
public class TranslationUnit : IEnumerable<InternalDeclaration> {
    /// <summary>
    /// Root namespace for the translation unit
    /// </summary>
    /// <returns>namespace</returns>
    public Namespace Namespace {get; private set;} = new Namespace();

    private List<InternalDeclaration> declarations = new List<InternalDeclaration>();

    public IEnumerator<InternalDeclaration> GetEnumerator() => declarations.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => declarations.GetEnumerator();

    public void AddDeclaration(InternalDeclaration declaration) {
        if (declaration is FunctionDeclaration funcl && funcl.Name.Value == "main" && funcl.FormalArguments.Count == 0) {
            this.Main = funcl;
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
        new Validation.AnalyzeExpressionTypes().Validate(this);
        new Validation.ValidateTypes().Validate(this);
    }
}
