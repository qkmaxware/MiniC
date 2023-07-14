namespace Qkmaxware.Languages.C.StandardLib;

public class LibraryLoader {
    public static VirtualDirectory Load() {
        var dir = new VirtualDirectory("lib");

        // Load embedded resources as virtual files
        var assembly = typeof(LibraryLoader).Assembly;
        var prefix = assembly.GetName().Name + ".src.StandardLibrary.";
        //Console.WriteLine(prefix);
        foreach (var name in assembly.GetManifestResourceNames()) {
            //Console.WriteLine(name);
            if (name.StartsWith(prefix)) {
                var name_only = name.Remove(0, prefix.Length);
                using var builder = new StringWriter();
                Stream? stream = assembly.GetManifestResourceStream(name);
                if (stream != null) {
                    using (var reader = new StreamReader(stream)) {
                        builder.Write(reader.ReadToEnd());
                    }
                }
                dir.Add(new VirtualFile(name_only, builder.ToString()));
            }
        }

        return dir;
    }
}