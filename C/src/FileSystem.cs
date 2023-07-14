using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qkmaxware.Languages.C;

public class FileSystem {
	public Directory LibraryPath;
	public Directory RootPath;

    public FileSystem(Directory libs, Directory root) {
        this.LibraryPath = libs;
        this.RootPath = root;
    }
}

public abstract class FileSystemObject {
    public abstract Directory? GetParent();
    internal abstract void SetParent(Directory dir);
}

public abstract class Directory : FileSystemObject {
	public abstract string Name {get;}
	public abstract IEnumerable<Directory> Subdirectories();
	public abstract IEnumerable<File> Files();

    public File? FindNestedFile(string path) {
        var parts = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

        var dir = this;
        for (var i = 0; i < parts.Length; i++) {
            bool isFilename = i == parts.Length - 1;

            if (isFilename) {
                return Files().Where(file => file.Name == parts[i]).FirstOrDefault();
            } else {
                var next_dir = Subdirectories().Where(dir => dir.Name == parts[i]).FirstOrDefault();
                if (next_dir == null)
                    return null;
                dir = next_dir;
                continue;
            }
        }

        return null;
    }
}
public class VirtualDirectory : Directory {
    private string _name;
    public override string Name => _name;
    private List<Directory> subs = new List<Directory>();
    private List<File> files = new List<File>();

    public VirtualDirectory(string name) {
        this._name = name;
    }

    public override IEnumerable<Directory> Subdirectories() => subs.AsReadOnly();
	public override IEnumerable<File> Files() => files.AsReadOnly();

    private Directory? parent;
    public override Directory? GetParent() => parent;
    internal override void SetParent(Directory dir) => parent = dir;

    public void Add(Directory dir) {
        this.subs.Add(dir);
        dir.SetParent(this);
    }
    public void Remove(Directory dir) {
        this.subs.Remove(dir);
        dir.SetParent(this);
    }
    public void Add(File file) {
        this.files.Add(file);
        file.SetParent(this);
    }
    public void Remove(File file) {
        this.files.Remove(file);
        file.SetParent(this);
    }
}
public class ActualDirectory : Directory {
    private DirectoryInfo info;
    public override string Name => info.Name;

    public ActualDirectory(DirectoryInfo info) {
        this.info = info;
    }

    public override Directory? GetParent() => info.Parent != null ? new ActualDirectory(info.Parent) : null;
    internal override void SetParent(Directory dir) {}

    public override IEnumerable<Directory> Subdirectories() => info.EnumerateDirectories().Select(dir => (Directory)new ActualDirectory(dir));
    
	public override IEnumerable<File> Files() => info.EnumerateFiles().Select(file => (File)new ActualFile(this, file));
}

public abstract class File : FileSystemObject  {
	public abstract string Name {get;}
	public abstract TextReader Open();

    public override string ToString() => Name;
}
public class VirtualFile : File {
    private string _name;
    private string _content;

    public VirtualFile(string name, string? content = null) {
        this._name = name;
        this._content = content ?? string.Empty;
    }
    
    public string GetContent() => _content;
    public void SetContent(string content) {
        this._content = content;
    }

    public override string Name => _name;
	public override TextReader Open() => new StringReader(_content);

    private Directory? parent;
    public override Directory? GetParent() => parent;
    internal override void SetParent(Directory dir) => parent = dir;
}
public class ActualFile : File {
    private FileInfo info;
    public override string Name => info.Name;

    public ActualFile(FileInfo file) {
        this.info = file;
    }
    public ActualFile(Directory parent, FileInfo file) {
        this.info = file;
        this.SetParent(parent);
    }

    public override Directory? GetParent() => info.Directory != null ? new ActualDirectory(info.Directory) : null;
    internal override void SetParent(Directory dir) {}

    public override TextReader Open() => new StreamReader(info.OpenRead());
}