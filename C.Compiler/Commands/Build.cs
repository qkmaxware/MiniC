using System;
using System.IO;
using System.Linq;
using CommandLine;
using Qkmaxware.Languages.C.Text;

namespace Qkmaxware.Languages.C.Terminal.Commands;

[Verb("build", HelpText = "Build source files")]
public class Build : BaseCommand {
    
    [Option('f', "file", HelpText = "Path to source files", Required = true)]
    public IEnumerable<string>? FileNames {get; set;}

    public string EmitDirectory = "obj";
    public string ExeDirectory = "bin";

    public override void Execute() {
        var dir = new DirectoryInfo(Environment.CurrentDirectory);
        var fs = new FileSystem(
            libs: StandardLib.LibraryLoader.Load(), 
            root: new ActualDirectory(dir)
        );
        if (FileNames == null)
            return;

        Namespace ns = new Namespace();
        Dictionary<File, string> unique_names = new Dictionary<File, string>();
        var allFiles = FileNames.Select(name => {
            var file = new FileInfo(name);
            var code = new Qkmaxware.Languages.C.ActualFile(file);
            var unique_name = ns.MakeUnique(file.Name);
            unique_names[code] = unique_name;
            return code;
        });

        // Parse everything
        if (!System.IO.Directory.Exists(EmitDirectory)) {
            System.IO.Directory.CreateDirectory(EmitDirectory);
        }
        var tasks = allFiles.Select(c => new Task<Vm.Module>(() => parse(fs, unique_names[c], c))).ToArray();
        Task.WhenAll(tasks);

        // Auto-link together?
    }

    private Vm.Module parse(FileSystem fs, string uid, File file) {
        var preprocessor = new Text.TextPreprocessor(new Text.PreprocessorTextReader(file), fs);
        var lexer = new Text.Lexer();
        var parser = new Text.Parser();

        var tokens = lexer.Tokenize(preprocessor);
        var unit = parser.Parse(tokens);

        unit.Validate();

        var emitter = new CToBytecode();
        var module = emitter.Convert(unit);

        using (var writer = new BinaryWriter(new FileStream(Path.Combine(EmitDirectory, uid + ".qkbc"), FileMode.Create))) {
            module.EncodeFile(writer);
        }

        return module;
    }

}