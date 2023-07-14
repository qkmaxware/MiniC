using System.IO;
using System.Collections.Generic;
using Source = Qkmaxware.Languages.C.File;
using System.Text;
using System;
using Qkmaxware.Languages.C;

namespace Qkmaxware.Languages.C.Text;

public class PreprocessorDefinitionSet : Dictionary<string, string> {}

// Example Usage
/*
public class TextPreprocessorUsage {
	public void Main() {
		// Create a file system (use ActualDirectories for access to the Host's filesystem. Can mix and match virtual with host files)
		var libs = new VirtualDirectory("lib");
		var root = new VirtualDirectory("src");
		var fs = new FileSystem(
			libs: libs,
			root: root
		);
		var lib = new VirtualFile("math.h", "int square(int x) { return x * x; }");
		libs.Add(lib);
		var main = new VirtualFile("app.c", 
@"#include <math.h> 

int main() { 
	int y = square(5); 
	return 1; 
}"
		);
		root.Add(main);

		// Create the preprocessor
		var processor = new TextPreprocessor(new PreprocessorTextReader(main), fs, new PreprocessorDefinitionSet());
		// Add any optional preprocessor directives (many added automatically)
		//processor.AddDirective(new MyCustomDirective1());
		//processor.AddDirective(new MyCustomDirective2());

		// Read the text (or pass off to a parser of some sort)
		using (var writer = new StringWriter()) {
			processor.EmitProcessedText(writer);
			writer.Flush();
		
			Console.WriteLine(writer.ToString());
		}
	}
}
*/

public class TextPreprocessor {
    private FileSystem fs;
    private PreprocessorDefinitionSet definitions;
	private PreprocessorTextReader reader;

    private List<Directive> directives = new List<Directive> {
        new DefineDirective(),
		// - The IF set -- 
        new IfdefElseDirective(),
		new IfndefElseDirective(),
		new EndIfDirective(),
		// --------------
        new IncludeDirective(),
    };

	public TextPreprocessor(PreprocessorTextReader reader, FileSystem fs) {
        this.fs = fs;
        this.definitions = new PreprocessorDefinitionSet();
        this.reader = reader;
    }

    public TextPreprocessor(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs) {
        this.fs = fs;
        this.definitions = defs;
        this.reader = reader;
    }

    public void AddDirective(Directive directive) {
        this.directives.Add(directive);
    }

	private List<ReadCharacter?> buffer = new List<ReadCharacter?>();

	/// <summary>
	/// Ensure the preprocessor has enough buffered characters
	/// </summary>
	/// <param name="capacity">number of buffered characters</param>
	public void EnsureCapacity(int capacity) {
		while (buffer.Count < capacity) {
			buffer.Add(read());
		}
	}

    private ReadCharacter? read() {
		ReadCharacter? next = null;
		while ((next = reader.Peek()).HasValue) {
			if (next.Value.Character == '#') {
				reader.Read();
				
				// Handle directives
				var nameBuilder = new StringBuilder();
				while ((next = reader.Peek()).HasValue && (char.IsLetterOrDigit(next.Value.Character) || next.Value.Character == '_')) {
					nameBuilder.Append(next.Value.Character);
					next = reader.Read();
				} 
				var name = nameBuilder.ToString();
				
				foreach (var directive in directives) {
					if (directive.Name == name) {
						directive.Action(this.reader, this.fs, this.definitions);
						break;
					}
				}
				
				continue;
			} else {
				next = reader.Read();
				return next;
			}
		}
		return null;
	}

	/// <summary>
	/// Read processed characters
	/// </summary>
	/// <returns>processed characters or null</returns>
	public ReadCharacter? Read() {
		EnsureCapacity(1);
		var next = buffer[0];

		// Rotate buffer
		for (var i = 0; i < buffer.Count -1; i++) {
			buffer[i] = buffer[i+1];
		}
		buffer[buffer.Count - 1] = read();

		return next;

	}

	/// <summary>
	/// Continue reading characters until a condition is met
	/// </summary>
	/// <param name="condition">stop condition</param>
	/// <returns>read string</returns>
	public string ReadUntil(Predicate<char> condition) {
		var peek = this.Peek();
		StringBuilder sb = new StringBuilder();
		while ((peek = this.Peek()).HasValue && !condition(peek.Value.Character)) {
			sb.Append(peek.Value.Character);
			this.Read();
		}
		return sb.ToString();
	}

	/// <summary>
	/// Continue reading characters while a condition is met
	/// </summary>
	/// <param name="condition">stop condition</param>
	/// <returns>read string</returns>
	public string ReadWhile(Predicate<char> condition) {
		var peek = this.Peek();
		StringBuilder sb = new StringBuilder();
		while ((peek = this.Peek()).HasValue && condition(peek.Value.Character)) {
			sb.Append(peek.Value.Character);
			this.Read();
		}
		return sb.ToString();
	}

	/// <summary>
	/// Read processed characters, skipping whitespace
	/// </summary>
	public void SkipWhitespace() {
		ReadCharacter? peek = null;
		while ((peek = Peek()).HasValue && char.IsWhiteSpace(peek.Value.Character)) {
			Read();
		}
	}

	/// <summary>
	/// Peek the next character in the processed character stream
	/// </summary>
	/// <returns>next character or null</returns>
	public ReadCharacter? Peek() {
		EnsureCapacity(1);
		return buffer[0];
	}

	/// <summary>
	/// Peek the n'th character in the processed character stream
	/// </summary>
	/// <returns>next character or null</returns>
	public ReadCharacter? Peek(int n) {
		EnsureCapacity(n + 1);
		return buffer[n];
	}

	/// <summary>
	/// Test if there is another character in the processed character stream
	/// </summary>
	/// <returns>true if next character exists</returns>
	public bool HasNext() {
		return Peek().HasValue;
	}

	/// <summary>
	/// Debug utility allows the entire text of a processed source to be emitted
	/// </summary>
	/// <param name="writer">writer to write processed text to</param>
	public void EmitProcessedText(TextWriter writer) {
		ReadCharacter? read = null;
		while ((read = Read()).HasValue) {
			writer.Write(read.Value.Character);
		}
	}
}

/// <summary>
/// A preprocessor directive
/// </summary>
public abstract class Directive {
	/// <summary>
	/// Directive name
	/// </summary>
    public abstract string Name {get;}
	/// <summary>
	/// The action performed by the preprocessor when this directive is discovered in code
	/// </summary>
	/// <param name="reader">text reader</param>
	/// <param name="fs">file system access</param>
	/// <param name="defs">defined values</param>
    public abstract void Action(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs);
}

/// <summary>
/// Directive to define constants in the preprocessor
/// </summary>
public class DefineDirective : Directive {
	public override string Name => "define";
	public string Format => "#define {identifier} {value}";
	public string Description => "define a value with a given identifer";
	public override void Action(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs) {
		reader.SkipWhitespaceExceptNewlines();
		var id = reader.ReadWord();
		reader.SkipWhitespaceExceptNewlines();
		var value = reader.ReadWord();
		
		if (!string.IsNullOrEmpty(id))
			defs[id] = value; // Done, that's it
	}
}

/// <summary>
/// Directive to conditionally include content based on a definition
/// </summary>
public class IfdefElseDirective : Directive {
	public override string Name => "ifdef";
	public string Format => "#ifdef {identifier} ... #endif";
	public string Description => "Conditionally include parts of a file if the identifier is defined";
	public override void Action(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs) {
		reader.SkipWhitespaceExceptNewlines();
		var id = reader.ReadWord();
        var end_tag = "#endif";

		if (!defs.ContainsKey(id)){				// It is not defined, consume it
			while (!reader.IsNext(end_tag)) {
				reader.Read();
			}
            // Consume the endif
            for (var i = 0; i < end_tag.Length; i++)
                reader.Read();
		}
	}
}

/// <summary>
/// Directive to conditionally include content based on a definition
/// </summary>
public class IfndefElseDirective : Directive {
	public override string Name => "ifndef";
	public string Format => "#ifndef {identifier} ... #endif";
	public string Description => "Conditionally include parts of a file if the identifier is not defined";
	public override void Action(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs) {
		reader.SkipWhitespaceExceptNewlines();
		var id = reader.ReadWord();
		
        var end_tag = "#endif";
		if (defs.ContainsKey(id)) {				// It is defined, consume it
			while (!reader.IsNext(end_tag)) {
				reader.Read();
			}
            // Consume the endif
            for (var i = 0; i < end_tag.Length; i++)
                reader.Read();
		}
	}
}

/// <summary>
/// Directive that marks the end of a conditional
/// </summary>
public class EndIfDirective : Directive {
	public override string Name => "endif";
	public string Format => "#endif";
	public string Description => "Directive placed at the end of a conditional";
	public override void Action(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs) {
		// Do nothing
	}
}

/// <summary>
/// Directive to insert the contents of one file at this spot in another file
/// </summary>
public class IncludeDirective : Directive {
	public override string Name => "include";
	public string Format => "#include <{path}>";
	public string Description => "Insert the text of the file at the given path in the current position of the current file";
	public override void Action(PreprocessorTextReader reader, FileSystem fs, PreprocessorDefinitionSet defs) {
		reader.SkipWhitespaceExceptNewlines();
		
		StringBuilder filePath = new StringBuilder();
		char endChar = '>';
        ReadCharacter? peeked = null;
		if ((peeked = reader.Peek()).HasValue && peeked.Value.Character == '<') {
			endChar = '>';
			reader.Read();
			
			peeked = null;
			while ((peeked = reader.Peek()).HasValue && peeked.Value.Character != endChar) {
				filePath.Append(peeked.Value.Character);
                reader.Read();
			}
			if (!(peeked = reader.Peek()).HasValue || peeked.Value.Character != endChar) {
				throw new PreprocessorException(reader.Source, reader.Position, $"Malformed file-path on include, missing closing '{endChar}' character on string");
			}
			reader.Read();
			
			var path = filePath.ToString();
			var source = fs.LibraryPath.FindNestedFile(path);
			if (source != null)
				reader.InjectSource(source);
			else 
				throw new PreprocessorException(reader.Source, reader.Position, new FileNotFoundException($"File '{path}' not found"));
		} else if ((peeked = reader.Peek()).HasValue && peeked.Value.Character == '\"') {
			endChar = '"';
			
			peeked = null;
			while ((peeked = reader.Peek()).HasValue && peeked.Value.Character != endChar) {
				// TODO escaped characters
				filePath.Append(peeked.Value.Character);
                reader.Read();
			}
			if (!(peeked = reader.Peek()).HasValue || peeked.Value.Character != endChar) {
				throw new PreprocessorException(reader.Source, reader.Position, $"Malformed file-path on include, missing closing '{endChar}' character on string");
			}
			reader.Read();
			
			var path = filePath.ToString();
			var source = reader.Source.GetParent()?.FindNestedFile(path);
			if (source != null)
				reader.InjectSource(source);
			else 
				throw new PreprocessorException(reader.Source, reader.Position, new FileNotFoundException($"File '{path}' not found"));
		} else {
            throw new PreprocessorException(reader.Source, reader.Position, "Include is mising file-path");
        }
	}
}