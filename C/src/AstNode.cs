using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Qkmaxware.Languages.C;

public abstract class AstNode {
    private Dictionary<Type, object> tags = new Dictionary<Type, object>();

    /// <summary>
    /// Add a metadata tag to this node.
    /// </summary>
    /// <typeparam name="T">tag type</typeparam>
    public void Tag<T>(T data) where T:class {
        tags[typeof(T)] = data;
    }

    /// <summary>
    /// Add a metadata tag to this node.
    /// </summary>
    /// <typeparam name="T">tag type</typeparam>
    public void Tag(params object[] data) {
        foreach (var d in data) {
            tags[d.GetType()] = d;
        }
    }

    /// <summary>
    /// List all metadata tags on this node
    /// </summary>
    /// <returns>list of tags</returns>
    public IEnumerable<object> Tags() => tags.Values;

    /// <summary>
    /// Fetch a metadata tag on this node if it exists. Returns true if a tag of the given type exists.
    /// </summary>
    /// <typeparam name="T">tag type</typeparam>
    public bool TryGetTag<T>([NotNullWhen(true)]out T? data) where T:class {
        object? value;
        if (tags.TryGetValue(typeof(T), out value)) {
            data = (T)value;
            return true;
        } else {
            data = null;
            return false;
        }
    }
}