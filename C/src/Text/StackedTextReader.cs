using System.IO;
using System.Collections.Generic;
using Source = Qkmaxware.Languages.C.File;
using System.Text;

namespace Qkmaxware.Languages.C.Text;

class ReaderPosition {
	public Source Source;
    public TextReader Reader;
    public long Line = 1;
    public long Column = 1;
    public long Index = 0;

	public Position Position => new Position(this.Index, this.Line, this.Column);

	public ReaderPosition(Source source, TextReader reader) {
		this.Source = source;
		this.Reader = reader;
	}
}

public interface IReader<T> {
	public T Peek();
	public T Read();
}

public struct ReadCharacter {
	public char Character;
	public Source Source;

	public Position Position;
}

public class StackedTextReader : IReader<ReadCharacter?> {
    private Stack<ReaderPosition> stack = new Stack<ReaderPosition>();

	public Source CurrentSource {
		get {
			if (stack.Count == 0)
				throw new System.IndexOutOfRangeException();
			return stack.Peek().Source;
		}
	}
	public Position CurrentPositionInSource {
		get {
			if (stack.Count == 0)
				throw new System.IndexOutOfRangeException();
			return stack.Peek().Position;
		}
	}

	public void InjectSource(Source source, TextReader reader) {
		this.stack.Push(new ReaderPosition (source, reader));
	}

    public ReadCharacter? Peek() {
		while (stack.Count > 0) {
			if (stack.Peek().Reader.Peek() == -1) {
				var top = stack.Pop();
				top.Reader.Dispose();
			} else {
				break;
			}
		}
		if (stack.Count == 0)
			return null;
		
		var active = stack.Peek();
		var next = active.Reader.Peek();
		if (next == -1)
			return null;
		else 
			return new ReadCharacter {
				Character = (char)next,
				Source = active.Source,
				Position = new Position(active.Index, active.Line, active.Column)
			};
	}

	public ReadCharacter? Read() {
		while (stack.Count > 0) {
			if (stack.Peek().Reader.Peek() == -1) {
				var top = stack.Pop();
				top.Reader.Dispose();
			} else {
				break;
			}
		}
		if (stack.Count == 0)
			return null;
		
		var active = stack.Peek();
		var next = active.Reader.Read();
		if (next == -1)
			return null;

		var position = new Position(active.Index, active.Line, active.Column);
		active.Column++;
		active.Index++;
		if (next == '\n') {
			active.Line++;
			active.Column = 1;
		}
		
		return new ReadCharacter {
			Character = (char)next,
			Source = active.Source,
			Position = position
		};
	}
}
