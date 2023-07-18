namespace Qkmaxware.Languages.C;

/// <summary>
/// A namespace is a collection of unique names
/// </summary>
public class Namespace {
    private HashSet<string> uids = new HashSet<string>();

    /// <summary>
    /// Given a desired name, create a new name that is a unique version of the desired name
    /// </summary>
    /// <param name="name">desired name in the namespace</param>
    /// <returns>a unique version of the desired name</returns>
    public string MakeUnique(string name) {
        var unique = name;
        var id = 0;
        while (uids.Contains(unique)) {
            unique = name + (++id);
        }
        return unique;
    }

    public bool Exists(string name) => uids.Contains(name);

    public Name ExitsOrThrow(string name) {
        if (uids.Contains(name)) {
            return new Name(this, name, false);
        } else {
            throw new ArgumentException("Identifier not found");
        }
    }
}

/// <summary>
/// An identifier or name
/// </summary>
public class Name : IEquatable<Name> {
    /// <summary>
    /// The value of the name
    /// </summary>
    /// <value>unique value</value>
    public string Value {get; private set;}
    /// <summary>
    /// The namespace this name is declared in
    /// </summary>
    /// <value>namespace</value>
    public Namespace DeclaredNamespace {get; private set;}
    /// <summary>
    /// Create a new name within the desired namespace
    /// </summary>
    /// <param name="ns">namespace to make the name unique</param>
    /// <param name="desired">desired name</param>
    public Name (Namespace ns, string desired) {
        this.DeclaredNamespace = ns;
        this.Value = ns.MakeUnique(desired);
    }

    internal Name(Namespace ns, string desired, bool makeUnique) {
        this.DeclaredNamespace = ns;
        this.Value = makeUnique ? ns.MakeUnique(desired) : desired;
    }

    public override string ToString() => Value;

    public bool Equals(Name? other) {
        return other != null && other.Value == this.Value && other.DeclaredNamespace == this.DeclaredNamespace;
    }
}

