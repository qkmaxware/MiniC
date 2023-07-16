namespace Qkmaxware.Languages.C;

public class Breakpoint {
    public File File {get; private set;}
    public long LineNumber {get; private set;}

    public Breakpoint(File file, long line) {
        this.File = file;
        this.LineNumber = line;
    }
}