namespace Qkmaxware.Languages.C.Text;

public class PreprocessorException : System.Exception {

    public File CodeSource {get; private set;}
    public Position PositionInSource {get; private set;}

    public PreprocessorException(File source, Position position, string message) : base(message) {
        this.CodeSource = source;
        this.PositionInSource = position;
    }

    public PreprocessorException(File source, Position position, Exception inner) : base(inner.Message, inner) {
        this.CodeSource = source;
        this.PositionInSource = position;
    }

}