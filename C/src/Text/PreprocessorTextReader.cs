using System.IO;
using System.Collections.Generic;
using Source = Qkmaxware.Languages.C.File;
using System.Text;

namespace Qkmaxware.Languages.C.Text;

public class PreprocessorTextReader : BufferedReader<ReadCharacter?> {
	private StackedTextReader stacked;
	public PreprocessorTextReader(Source starts) : base(new StackedTextReader()) {
		this.stacked = (StackedTextReader)this.Underlying;
		stacked.InjectSource(starts, starts.Open());
	}

    public Source Source => stacked.CurrentSource;
    public Position Position => stacked.CurrentPositionInSource;

    public void InjectSource(Source source) {
        this.stacked.InjectSource(source, source.Open());
    }

	public void SkipWhitespace() {
		ReadCharacter? peek = null;
		while ((peek = Peek()).HasValue && char.IsWhiteSpace(peek.Value.Character)) {
			Read();
		}
	}
	public void SkipWhitespaceExceptNewlines() {
		ReadCharacter? peek = null;
		while ((peek = Peek()).HasValue && char.IsWhiteSpace(peek.Value.Character) && peek.Value.Character != '\n') {
			Read();
		}
	}
	
	public string ReadWord() {
		var builder = new StringBuilder();
		ReadCharacter? peek = null;
		while ((peek = Peek()).HasValue && !char.IsWhiteSpace(peek.Value.Character)) {
			Read();
			builder.Append(peek.Value.Character);
		}
		return builder.ToString();
	}

	public bool IsNext(string pattern) {
		EnsureCapacity(pattern.Length);
		for (var i = 0; i < pattern.Length; i++) {
			var peeked = this.Peek(i);
			if (peeked.HasValue && peeked.Value.Character == pattern[i]) {
				continue;
			} else {
				return false;
			}
		}
		return true;
	}
}