/*using Qkmaxware.Languages.C;

// Create a file system (use ActualDirectories for access to the Host's filesystem. Can mix and match virtual with host files)
var libs = Qkmaxware.Languages.C.StandardLib.LibraryLoader.Load();
var root = new VirtualDirectory("src");
var fs = new FileSystem(
    libs: libs,
    root: root
);
var main = new VirtualFile("app.c", 
@"#include <math.h>

int main() { 
    int y = square(5); 
    return 1; 
}"
);
root.Add(main);

Console.WriteLine("Before Processing...");
Console.WriteLine(main.Open().ReadToEnd());

// Create the preprocessor
var processor = new TextPreprocessor(new PreprocessorTextReader(main), fs, new PreprocessorDefinitionSet());
// Add any optional preprocessor directives (a few added automatically)
//processor.AddDirective(new MyCustomDirective1());
//processor.AddDirective(new MyCustomDirective2());

// Read the text (or pass off to a parser of some sort)
Console.WriteLine("After Processing...");
using (var writer = new StringWriter()) {
    processor.EmitProcessedText(writer);
    writer.Flush();

    Console.WriteLine(writer.ToString());
}*/