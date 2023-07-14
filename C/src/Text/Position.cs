namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// A position within a source file
/// </summary>
public class Position {
    /// <summary>
    /// Character index in the source file
    /// </summary>
    public long Index {get; private set;}
    /// <summary>
    /// Line number in the source file
    /// </summary>
    public long LineNumber {get; private set;}
    /// <summary>
    /// Column number in the source file
    /// </summary>
    public long Column {get; private set;}

    internal Position(long index, long line, long column) {
        this.Index = index;
        this.LineNumber = line;
        this.Column = column;
    }

    public override string ToString() => $"line {LineNumber}, column {Column}";
}