namespace Qkmaxware.Languages.C.Test;

[TestClass]
public class TestCompilation {
    private void emit(TranslationUnit unit, string name) {
        var emitter = new CToBytecode();
        var module = emitter.Convert(unit);
        using (var writer = new BinaryWriter(new FileStream(name, FileMode.Create))) {
            module.EncodeFile(writer);
        }
    }

    [TestMethod]
    public void HelloWorldAsm() {
        /*
        void main() {
            asm {
                !immediate_utf8 "Hello World"
                !printstr
            }
            return;
        }
        */
        var file = new TranslationUnit();
        file.AddDeclaration(new FunctionDeclaration(
            returnType: Void.Instance, 
            name: new Name(file.Namespace, "main"), 
            args: new FormalArgument[]{}, 
            body: new CompoundStatement(
                new AsmStatement (
                    @"!immediate_utf8 ""Hello World\r\n""
                    !printstr"
                ),
                new ReturnStatement()
            )
        ));
    }

    [TestMethod]
    public void HelloWorld() {
var code = new VirtualFile("main.c", 
@"#include <console.h>

/*
This is a block comment
*/
int main() {
    // Not sure if this needs to be free'd
    char[] str = ""Hello World"";
    print_str(str);
    free(str);
    return 0;
}");
        var fs = new FileSystem(libs: StandardLib.LibraryLoader.Load(), new VirtualDirectory("src"));
        ((VirtualDirectory)fs.RootPath).Add(code);
        var preprocessor = new Text.TextPreprocessor(new Text.PreprocessorTextReader(code), fs);
        var lexer = new Text.Lexer();
        var parser = new Text.Parser();
        try {
            //Console.WriteLine(string.Join(", ", lexer.Tokenize(preprocessor).ToList()));
            var unit = parser.Parse(lexer.Tokenize(preprocessor));
            emit(unit, "TestCompilation.HelloWorld.qkbc");
        } catch (Text.LexicalException lx) {
            Console.Write(lx.PrettyPrint());
        } catch (Text.ParserException px) {
            Console.Write(px.PrettyPrint());
        }
    }

    [TestMethod]
    public void TestConstantReturn() {
        var file = new TranslationUnit();
        var pi = new TranslationUnit();

        var method = new FunctionDeclaration(new Name(file.Namespace, "pi"));
        file.AddDeclaration(method);
        method.Body.Add(
            new AsmStatement(
                @"@pi = 3.1415926f
                load_const @pi
                return_function")
        );

        emit(file, "TestCompilation.TestConstantReturn.qkbc");
    }

    [TestMethod]
    public void TestIf() {
        var file = new TranslationUnit();
        var main = new FunctionDeclaration(new Name(file.Namespace, "main"));
        var cond = main.MakeArg("cond", Integer.Instance);
        main.Body.Add(
            new IfStatement(cond: new LtExpression(new LoadVarExpression(cond), new LiteralIntExpression(5)), new CompoundStatement(
                new ReturnStatement(new LiteralIntExpression(1))
            )).ElseIf(new LiteralIntExpression(1), new CompoundStatement(
                new ReturnStatement(new LiteralIntExpression(0))
            ))
        );
        file.AddDeclaration(main);

        emit(file, "TestCompilation.TestIf.qkbc");
    }

    [TestMethod]
    public void FibonachiBytecode() {
        var file = new TranslationUnit();

        var main = new FunctionDeclaration(new Name(file.Namespace, "main"), new CompoundStatement());
        var method = new FunctionDeclaration(new Name(file.Namespace, "Fibonachi"), new CompoundStatement());

        file.AddDeclaration(main);
        var arg = main.MakeLocal("arg", Integer.Instance);
        var res = main.MakeLocal("res", Integer.Instance);
        main.Body.Add(
            new AssignmentStatement(arg, new LiteralIntExpression(5)),
            new AssignmentStatement(res, new CallExpression(method, new LoadVarExpression(arg))),
            new ReturnStatement()
        );

        var n = method.MakeArg("n", Integer.Instance);
        method.Body.Add(
            //if (n < 1) {
            //    return 1
            //}
            new IfStatement(
                new LtExpression(new LoadVarExpression(n), new LiteralIntExpression(1)),
                new CompoundStatement(
                    new ReturnStatement(new LiteralIntExpression(1))
                )
            ),
            //return n * fib(n-1)
            new ReturnStatement(new MulExpression(new LoadVarExpression(n), new CallExpression(method, new SubExpression(new LoadVarExpression(n), new LiteralIntExpression(1)))))
        );
        file.AddDeclaration(method);

        emit(file, "TestCompilation.FibonachiBytecode.qkbc");
    }

    [TestMethod]
    public void Fibonachi() {
        var code = new VirtualFile("main.c", 
@"#include <math.h>

int fib (int i) {
    if (i < 1) {
        return 1;
    } else if (i == 1) {
        return 1;
    } else {
        return i * fib(i - 1);
    }
}
void main() {
    int arg = 5;
    int value = fib(arg);
    return;
}");
        var fs = new FileSystem(libs: StandardLib.LibraryLoader.Load(), new VirtualDirectory("src"));
        ((VirtualDirectory)fs.RootPath).Add(code);
        var preprocessor = new Text.TextPreprocessor(new Text.PreprocessorTextReader(code), fs);
        var lexer = new Text.Lexer();
        var parser = new Text.Parser();
        try {
            //Console.WriteLine(string.Join(", ", lexer.Tokenize(preprocessor).ToList()));
            var unit = parser.Parse(lexer.Tokenize(preprocessor));
            emit(unit, "TestCompilation.Fibonachi.qkbc");
        } catch (Text.LexicalException lx) {
            Console.Write(lx.PrettyPrint());
        } catch (Text.ParserException px) {
            Console.Write(px.PrettyPrint());
        }
    }
}