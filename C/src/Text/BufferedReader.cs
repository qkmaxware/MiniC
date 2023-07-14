using System.Text;

namespace Qkmaxware.Languages.C.Text;

public class BufferedReader<T> : IReader<T> {
	protected IReader<T> Underlying {get; private set;}
	private List<T> buffer = new List<T>();

	public BufferedReader(IReader<T> underlying) {
		this.Underlying = underlying;
	}

	public void EnsureCapacity(int capacity) {
		while (buffer.Count < capacity) {
			buffer.Add(Underlying.Read());
		}
	}

    public T Peek() {
        EnsureCapacity(1);
		return buffer[0];
    }
	public T Peek(int n) {
        EnsureCapacity(n + 1);
		return buffer[n];
    }

    public T Read() {
        EnsureCapacity(1);
		var next = buffer[0];

		// Rotate buffer
		for (var i = 0; i < buffer.Count -1; i++) {
			buffer[i] = buffer[i+1];
		}
		buffer[buffer.Count - 1] = Underlying.Read();

		return next;
    }
}