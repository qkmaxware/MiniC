using System.Diagnostics.CodeAnalysis;
using C.Web;

public class Result<T> where T:class {
    
    private T? value;
    private Exception? error;

    public Result(T value) {
        this.value = value;
        this.error = null;
    }
    public Result(Exception e) {
        this.value = default(T);
        this.error = e;
    }

    public bool TryUnwrap([NotNullWhen(true)]out T? value) {
        if (this.value != null) {
            value = this.value;
            return true;
        } else {
            value = null;
            return false;
        }
    }

    public bool TryUnwrapException([NotNullWhen(true)]out Exception? value) {
        if (this.error != null) {
            value = this.error;
            return true;
        } else {
            value = null;
            return false;
        }
    }
}