namespace Qkmaxware.Languages.C.Test;

[TestClass]
public class TestStandardLibrary {
    [TestMethod]
    public void EnsureLibraryExists() {
        var lib = C.StandardLib.LibraryLoader.Load();

        var files = lib.Files().ToList();
        Assert.AreNotEqual(0, files.Count);
    }
}