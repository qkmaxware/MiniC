using Qkmaxware.Languages.C.Text;

namespace Qkmaxware.Languages.C.Validation;

/// <summary>
/// Base interface for validation passes
/// </summary>
public interface IValidationPass {
    /// <summary>
    /// Validate this module against this validation pass
    /// </summary>
    /// <param name="unit">translation unit to validate</param>
    public void Validate(TranslationUnit unit);
}

/// <summary>
/// Base class for applying a validation passes
/// </summary>
public abstract class BaseValidationPass : IValidationPass {
    /// <summary>
    /// Validate this module against this validation pass
    /// </summary>
    /// <param name="unit">translation unit to validate</param>
    public abstract void Validate(TranslationUnit unit);

    protected void ThrowSemanticError(AstNode on, string Message) {
        //foreach (var tags in on.Tags()) {
            //Console.WriteLine(on.GetType());
            //Console.WriteLine($"> " + tags.GetType().Name + " = " + tags);
        //}
        File? src; Position? pos;
        if (on.TryGetTag<File>(out src) && on.TryGetTag<Position>(out pos)) {
            throw new SematicException(src, pos, Message);
        } else {
            throw new SematicException(Message);
        }
    }

    protected void ThrowSemanticError(AstNode on, string Message, Exception inner) {
        File? src; Position? pos;
        if (on.TryGetTag<File>(out src) && on.TryGetTag<Position>(out pos)) {
            throw new SematicException(src, pos, Message, inner);
        } else {
            throw new SematicException(Message, inner);
        }
    }
}