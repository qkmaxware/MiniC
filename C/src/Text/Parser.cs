namespace Qkmaxware.Languages.C.Text;

/// <summary>
/// Mini C parser
/// </summary>
public class Parser {
    public TranslationUnit Parse(TokenStream stream) {
        return parseUnit(stream);
    }
    private TranslationUnit parseUnit(TokenStream stream) {
        TranslationUnit unit = new TranslationUnit();
        while (stream.HasNext()) {
            parseDecl(unit, stream);
        }
        return unit;
    }
    private InternalDeclaration parseDecl(TranslationUnit unit, TokenStream stream) {
        // Parse type-spec
        var returnType = parseTypeSpec(unit, stream);

        // Parse ident
        var ident = parseIdentifier(stream);
        if (unit.Namespace.Exists(ident.Text)) {
            throw new ParserException(ident.Source, ident.StartPosition, $"Identifier '{ident.Text}' is already declared in this translation unit");
        }
        var name = new Name(unit.Namespace, ident.Text);

        // Branch on function or static variable
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();
        if (peek is Semicolon) {
            // Is a static variable
            stream.Advance(); // eat the ;
            var decl = new StaticVariableDeclaration(new Name(unit.Namespace, ident.Text), returnType);
            decl.Tag<File>(peek.Source); decl.Tag<Position>(peek.StartPosition);
            unit.AddDeclaration(decl);

            return decl;
        } else if (peek is OpenParenthesis) {
            var func = new FunctionDeclaration(name);
            func.Tag<File>(ident.Source); func.Tag<Position>(ident.StartPosition);
            func.ReturnType = returnType;

            // Is a function definition
            stream.Advance();

            // Read in function arguments
            while ((peek = stream.Peek(0)) != null && peek is not CloseParenthesis) {
                // Read type
                var arg_type = parseTypeSpec(unit, stream);

                // Read argument name
                var arg_id = parseIdentifier(stream); 

                // Add arg
                var arg = func.MakeArg(arg_id.Text, arg_type);

                // Read comma
                peek = stream.Peek(0);
                if (peek != null && peek is Comma) {
                    stream.Advance();
                    continue;
                } else {
                    break;
                }
            }

            // Read in closing parenthesis
            peek = stream.Peek(0);
            if (peek == null)
                throw new EndOfStreamException();

            if (peek is not CloseParenthesis) {
                throw new ParserException(peek.Source, peek.StartPosition, "Missing close parenthesis on function arguments list.");
            } 
            stream.Advance();

            // Parse body
            unit.AddDeclaration(func);
            parseCompoundStatement(unit, func, func.Body, stream);

            // Return it
            return func;
        } else {
            throw new ParserException(ident.Source, ident.StartPosition, "Expected semicolon for variable declaration or argument list for function declaration.");
        }
    }

    private Identifier parseIdentifier(TokenStream stream) {
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();
        if (peek is not Identifier id)
            throw new ParserException(peek.Source, peek.StartPosition, "Identifier expected.");
        stream.Advance();
        return id;
    }

    private TypeSpecifier parseTypeSpec(TranslationUnit unit, TokenStream stream) {
        var typePeek = stream.Peek(0);
        if (typePeek == null)
            throw new EndOfStreamException();
        var simple = parseSimpleTypeSpec(unit, stream);

        var peek = stream.Peek(0);
        if (peek != null && peek is OpenBracket) {
            stream.Advance();
            peek = stream.Peek(0);
            if (peek == null)   
                throw new EndOfStreamException();
            if (peek is not CloseBracket) {
                throw new ParserException(peek.Source, peek.StartPosition, "Missing closing ] on array type specification.");
            }
            stream.Advance();
            if (simple is not ValueTypeSpecifier value)
                throw new ParserException(typePeek.Source, typePeek.StartPosition, $"Arrays cannot contain values of type '{simple}'.");
            return new Array(value);
        } else {
            return simple;
        }
    }

    private TypeSpecifier parseSimpleTypeSpec(TranslationUnit unit, TokenStream stream) {
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();

        switch (peek) {
            case IntKeyword i:
                stream.Advance();
                return Integer.Instance;
            case UIntKeyword u:
                stream.Advance();
                return UnsignedInteger.Instance;
            case FloatKeyword f:
                stream.Advance();
                return Float.Instance;
            case BoolKeyword b:
                stream.Advance();
                return Integer.Instance;
            case CharKeyword c:
                stream.Advance();
                return Char.Instance;
            case VoidKeyword v:
                stream.Advance();
                return Void.Instance;
            default:
                throw new ParserException(peek.Source, peek.StartPosition, "Missing type specification");
        }   
    }

    private CompoundStatement parseCompoundStatement(TranslationUnit unit, FunctionDeclaration func, CompoundStatement block, TokenStream stream) {
        // {
        var open = stream.Peek(0);
        if (open == null)
            throw new EndOfStreamException();
        if (open is not OpenBrace) {
            throw new ParserException(open.Source, open.StartPosition, "Missing opening { on block of code.");
        }
        stream.Advance();

        // Body
        var peek = stream.Peek(0);
        while ((peek = stream.Peek(0)) != null && peek is not CloseBrace) {
            var stmt = parseStatement(unit, func, block, stream);
            if (stmt != null)
                block.Add(stmt);
        }

        // }
        var close = stream.Peek(0);
        if (close == null)
            throw new EndOfStreamException();
        if (close is not CloseBrace) {
            throw new ParserException(close.Source, close.StartPosition, "Missing closing } on block of code.");
        }
        stream.Advance();

        block.Tag<File>(open.Source); block.Tag<Position>(open.StartPosition);

        return block;
    }

    private Statement? parseStatement(TranslationUnit unit, FunctionDeclaration func, CompoundStatement block, TokenStream stream) {
        // compound_stmt | if_stmt | while_stmt | return_stmt | break_stmt | expr_stmt
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();

        switch (peek) {
            case OpenBrace:
                return parseCompoundStatement(unit, func, new CompoundStatement(), stream);
            case IntKeyword i:
            case UIntKeyword u:
            case FloatKeyword f:
            case BoolKeyword b:
            case CharKeyword c:
            case VoidKeyword v:
                // All valid types go here ^^ the same as in parseSimpleTypeSpec
                var localAssignment = parseLocalDecl(unit, func, stream);
                return localAssignment;
            case IfKeyword:
                return parseIfStatement(unit, func, stream);
            case WhileKeyword:
                return parseWhileStatement(unit, func, stream);
            case ForKeyword:
                return parseForStatement(unit, func, stream);
            case ReturnKeyword:
                return parseReturnStatement(unit, func, stream);
            case BreakKeyword:
                return parseBreakStatement(unit, func, stream);
            case ContinueKeyword:
                return parseContinueStatement(unit, func, stream);
            case AssemblyToken:
                return parseAssembly(unit, func, stream);
            case Semicolon:
                stream.Advance();
                return null; // Skip this
            case FreeKeyword size: 
                return parseFree(unit, func, stream);
            default:
                // Assignment or function call is based on 1 lookahead (both start with an ident)
                var next = stream.Peek(1);
                if (next == null) {
                    throw new EndOfStreamException();
                }

                Statement stmt;
                if (next is OpenParenthesis) {
                    // Is function call
                    stmt = new ExprStatement(parseFunctionCall(unit, func, stream));
                } else {
                    // Is assignment or error
                    stmt = parseAssignment(unit, func, stream);
                }

                var semicolon = stream.Peek(0);
                if (semicolon == null) {
                    throw new EndOfStreamException();
                }
                if (semicolon is not Semicolon) {
                    throw new ParserException(semicolon.Source, semicolon.StartPosition, "Missing semicolon after expression statement");
                } 
                stream.Advance();
                return stmt;
        }
    }   

    private Statement parseFree(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();
        if (peek is not FreeKeyword)
            throw new ParserException(peek.Source, peek.StartPosition, "Free expression must start with the 'free' keyword.");
        // free
        stream.Advance();
        // (
        var size_next = stream.Peek(0);
        if (size_next == null)
            throw new EndOfStreamException();
        if (size_next is not OpenParenthesis)
            throw new ParserException(size_next.Source, size_next.StartPosition, "Free expression must be followed by a parenthesis.");
        stream.Advance();   

        // IDENT
        var ident = parseIdentifier(stream);

        // )
        size_next = stream.Peek(0);
        if (size_next == null)
            throw new EndOfStreamException();
        if (size_next is not CloseParenthesis)
            throw new ParserException(size_next.Source, size_next.StartPosition, "Free expression must end in a parenthesis.");
        stream.Advance();

        var free_semicolon = stream.Peek(0);
        if (free_semicolon == null) {
            throw new EndOfStreamException();
        }
        if (free_semicolon is not Semicolon) {
            throw new ParserException(free_semicolon.Source, free_semicolon.StartPosition, "Missing semicolon after free statement");
        } 
        stream.Advance();
        var free = new FreeExpression(getVar(unit, func, ident));
        free.Tag<File>(peek.Source); free.Tag<Position>(peek.StartPosition);
        return new ExprStatement(free);
    }

    private AsmStatement parseAssembly(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not AssemblyToken tok)
            throw new ParserException(next.Source, next.StartPosition, "Inline assembly should be contained in an assembly block.");
        stream.Advance();

        var asm = new AsmStatement(tok.Text);
        asm.Tag<File>(next.Source); asm.Tag<Position>(next.StartPosition);
        return asm;
    }

    private Statement? parseLocalDecl(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();
        // type_spec
        var type = parseTypeSpec(unit, stream);

        // IDENT
        var name = parseIdentifier(stream);
        // TODO this is not quite right
        foreach (var local in func.Locals) {
            if (local.Name.Value == name.Text) {
                throw new ParserException(name.Source, name.StartPosition, $"A local variable '{name.Text}' has already been defined in the scope or the encapsulating scope.");
            }
        }

        // (= expr)?
        Expression? assignment = null;
        var equals = stream.Peek(0);
        if (equals == null) {
            throw new EndOfStreamException();
        } 
        if (equals is AssignmentOperator) {
            stream.Advance();
            assignment = parseExpression(unit, func, stream);
        }

        // ;
        var semicolon = stream.Peek(0);
        if (semicolon == null) {
            throw new EndOfStreamException();
        }
        if (semicolon is not Semicolon) {
            throw new ParserException(semicolon.Source, semicolon.StartPosition, "Missing semicolon after expression.");
        } 
        stream.Advance();

        // Make the variable
        var declaration = func.MakeLocal(name.Text, type);
        declaration.Tag<File>(peek.Source); declaration.Tag<Position>(peek.StartPosition);

        // If we have an assignment, add an assignment statement. 
        // int x = 5; is equivalent to: int x; x = 5;
        if (assignment != null) {
            var ag = new AssignmentStatement(declaration, assignment);
            ag.Tag<File>(equals.Source); ag.Tag<Position>(equals.StartPosition);
            return ag;
        } else {
            return null;
        }
    }

    private CallExpression parseFunctionCall(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();
        var ident = parseIdentifier(stream);
    
        // (
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not OpenParenthesis)
            throw new ParserException(next.Source, next.StartPosition, "Missing open parenthesis on argument list.");
        stream.Advance();

        // Arg-list
        List<Expression> args = new List<Expression>();
        while ((next = stream.Peek(0)) != null && next is not CloseParenthesis) {
            args.Add(parseExpression(unit, func, stream));
            
            next = stream.Peek(0);
            if (next != null && next is Comma)
                continue;
            else 
                break;
        }

        // )    
        next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not CloseParenthesis)
            throw new ParserException(next.Source, next.StartPosition, "Missing close parenthesis on argument list.");
        stream.Advance();

        // Locate function
        var function = (FunctionDeclaration?)unit.Where(decl => decl is FunctionDeclaration funcdef && funcdef.Name.Value == ident.Text).FirstOrDefault();
        if (function == null) {
            throw new ParserException(ident.Source, ident.StartPosition, $"Function '{ident.Text}' is not declared in the visible scope.");
        }
        if (function.FormalArguments.Count != args.Count) {
            throw new ParserException(ident.Source, ident.StartPosition, $"Argument count mismatch.");
        }

        var call = new CallExpression(function, args.ToArray());
        call.Tag<File>(peek.Source); call.Tag<Position>(peek.StartPosition);
        return call;
    }

    private Statement parseAssignment(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var peek = stream.Peek(0);
        if (peek == null)
            throw new EndOfStreamException();
        var ident = parseIdentifier(stream);

        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();

        if (next is AssignmentOperator) {
            // ident = value
            stream.Advance();
            var value = parseExpression(unit, func, stream);

            var local = getVar(unit, func, ident);

            return new AssignmentStatement(local, value);
        } else if (next is OpenBracket) {
            // ident[index] = value
            stream.Advance();
            var index = parseExpression(unit, func, stream);

            var close = stream.Peek(0);
            if (close == null)
                throw new EndOfStreamException();
            if (close is not CloseBracket) 
                throw new ParserException(close.Source, close.StartPosition, "Missing closing ] on array index.");
            stream.Advance();

            next = stream.Peek(0);
            if (next == null)
                throw new EndOfStreamException();
            if (next is not AssignmentOperator)
                throw new ParserException(next.Source, next.StartPosition, "Missing assignment operator.");
            stream.Advance();

            var value = parseExpression(unit, func, stream);
            var local = getVar(unit, func, ident);

            var ag = new ArrayAssignmentStatement(local, index, value);
            ag.Tag<File>(peek.Source); ag.Tag<Position>(peek.StartPosition);
            return ag;
        } else {
            // Error
            throw new ParserException(next.Source, next.StartPosition, "Assignments must be of the form 'name = value' or 'name[index] = value'");
        }
    }

    private IfStatement parseIfStatement(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var key = stream.Peek(0);
        if (key == null)
            throw new EndOfStreamException();
        if (key is not IfKeyword)
            throw new ParserException(key.Source, key.StartPosition, "If statements should start with the 'if' keyword.");
        stream.Advance();

        var first_condition = parseExpression(unit, func, stream);
        var first_body = parseCompoundStatement(unit, func, new CompoundStatement(), stream);
        var iff = new IfStatement(first_condition, first_body);
        iff.Tag<File>(key.Source); iff.Tag<Position>(key.StartPosition);

        while ((key = stream.Peek(0)) != null && key is ElseKeyword) {
            stream.Advance();

            key = stream.Peek(0);
            if (key != null && key is IfKeyword) {
                // Else if 
                stream.Advance();

                var condition = parseExpression(unit, func, stream);
                var body = parseCompoundStatement(unit, func, new CompoundStatement(), stream);
                iff = iff.ElseIf(condition, body);
            } else {
                // Else only
                var body = parseCompoundStatement(unit, func, new CompoundStatement(), stream);
                var tautology = new EqExpression(new LiteralIntExpression(1), new LiteralIntExpression(1));
                iff = iff.ElseIf(tautology, body);
                break; // Else's end the chain
            }
        }

        return iff;
    }

    private WhileStatement parseWhileStatement(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var key = stream.Peek(0);
        if (key == null)
            throw new EndOfStreamException();
        if (key is not WhileKeyword)
            throw new ParserException(key.Source, key.StartPosition, "While statements should start with the 'while' keyword.");
        stream.Advance();

        var condition = parseExpression(unit, func, stream);
        var body = parseCompoundStatement(unit, func, new CompoundStatement(), stream);
        var s = new WhileStatement(condition, body);
        s.Tag<File>(key.Source); s.Tag<Position>(key.StartPosition);
        return s;
    }

    private Statement parseForStatement(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var key = stream.Peek(0);
        if (key == null)
            throw new EndOfStreamException();
        if (key is not ForKeyword)
            throw new ParserException(key.Source, key.StartPosition, "For statements should start with the 'for' keyword.");
        stream.Advance();

        openParen(stream);
        var declAssignment = parseLocalDecl(unit, func, stream);
        //semicolon(stream); // local decl already parses for a semi-colon
        var condition = parseExpression(unit, func, stream);
        semicolon(stream);
        var increment = parseAssignment(unit, func, stream);
        closeParen(stream);
        var body = parseCompoundStatement(unit, func, new CompoundStatement(), stream);

        var block = new CompoundStatement();
        if (declAssignment != null)
            block.Add(declAssignment);
        body.Add(increment);
        var @while = new WhileStatement(condition, body);
        block.Add(@while);

        @while.Tag<File>(key.Source); @while.Tag<Position>(key.StartPosition);
        @block.Tag<File>(key.Source); @block.Tag<Position>(key.StartPosition);

        return block;
    }

    private void openParen(TokenStream stream) {
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not OpenParenthesis)
            throw new ParserException(next.Source, next.StartPosition, "Expression must start with a parenthesis.");
        stream.Advance();
    }
    private void closeParen(TokenStream stream) {
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not CloseParenthesis)
            throw new ParserException(next.Source, next.StartPosition, "Expression must end with a close parenthesis.");
        stream.Advance();
    }

    private void semicolon(TokenStream stream) {
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not Semicolon)
            throw new ParserException(next.Source, next.StartPosition, "Missing semicolon.");
        stream.Advance();
    }


    private ReturnStatement parseReturnStatement(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var key = stream.Peek(0);
        if (key == null)
            throw new EndOfStreamException();
        if (key is not ReturnKeyword)
            throw new ParserException(key.Source, key.StartPosition, "Return statements should start with the 'return' keyword.");
        stream.Advance();

        var semicolon = stream.Peek(0);
        if (semicolon == null) {
            throw new EndOfStreamException();
        }
        if (semicolon is Semicolon) {
            stream.Advance();
            var s = new ReturnStatement();
            s.Tag<File>(key.Source); s.Tag<Position>(key.StartPosition);
            return s;
        } else {
            var expr = parseExpression(unit, func, stream);
            semicolon = stream.Peek(0);
            if (semicolon == null) {
                throw new EndOfStreamException();
            }
            if (semicolon is not Semicolon) {
                throw new ParserException(semicolon.Source, semicolon.StartPosition, "Missing semicolon after return expression.");
            } 
            stream.Advance();

            var s = new ReturnStatement(expr);
            s.Tag<File>(key.Source); s.Tag<Position>(key.StartPosition);
            return s;
        }
    }

    private BreakStatement parseBreakStatement(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var key = stream.Peek(0);
        if (key == null)
            throw new EndOfStreamException();
        if (key is not BreakKeyword)
            throw new ParserException(key.Source, key.StartPosition, "Break statements should start with the 'break' keyword.");
        stream.Advance();

        var semicolon = stream.Peek(0);
        if (semicolon == null) {
            throw new EndOfStreamException();
        }
        if (semicolon is Semicolon) {
            stream.Advance();
            var s = new BreakStatement();
            s.Tag<File>(key.Source); s.Tag<Position>(key.StartPosition);
            return s;
        } else {
            throw new ParserException(semicolon.Source, semicolon.StartPosition, "Missing semicolon after break statement.");
        }
    }

    private ContinueStatement parseContinueStatement(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var key = stream.Peek(0);
        if (key == null)
            throw new EndOfStreamException();
        if (key is not BreakKeyword)
            throw new ParserException(key.Source, key.StartPosition, "Continue statements should start with the 'continue' keyword.");
        stream.Advance();

        var semicolon = stream.Peek(0);
        if (semicolon == null) {
            throw new EndOfStreamException();
        }
        if (semicolon is Semicolon) {
            stream.Advance();
            var s = new ContinueStatement();
            s.Tag<File>(key.Source); s.Tag<Position>(key.StartPosition);
            return s;
        } else {
            throw new ParserException(semicolon.Source, semicolon.StartPosition, "Missing semicolon after continue statement.");
        }
    }

    private Expression parseExpression(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        // https://en.wikipedia.org/wiki/Shunting_yard_algorithm#The_algorithm_in_detail
        Stack<Expression> outputQueue = new Stack<Expression>();
        Stack<IOperator> operatorStack = new Stack<IOperator>();

        // While there are tokens to be read
        var first = stream.Peek(0);
        if ((first = stream.Peek(0)) == null) {
            throw new EndOfStreamException();
        }
        var peek = first;
        while ((peek = stream.Peek(0)) != null) {
            // If the token is an operator
            if (peek is IOperator o1) {
                stream.Advance();
                IOperator o2;
                while (operatorStack.Count > 0 && ((o2 = operatorStack.Peek()) != null) && (o2.Precedence > o1.Precedence || o1.Precedence == o2.Precedence && o1.Associativity == Associativity.Left)) {
                    o2 = operatorStack.Pop();
                    // Do operation to construct a new expression
                    Expression[] operands = new Expression[o2.Arity];
                    for (var i = 0; i < o2.Arity; i++) {
                        operands[o2.Arity - 1 - i] = outputQueue.Pop();
                    }
                    var combined = o2.Combine(operands);
                    if (o2 is Token tok) {
                        combined.Tag<File>(tok.Source); combined.Tag<Position>(tok.StartPosition);
                    }
                    outputQueue.Push(combined);
                }
                operatorStack.Push(o1);
            }
            // Else parse an expression literal
            // Needs to be fixed, not everything is an atom
            else {
                var expr = parseAtom(unit, func, stream);
                if (expr == null)
                    break;
                outputQueue.Push(expr);
            }
        }

        // While there are tokens on the operator stack
        while (operatorStack.Count > 0) {
            var op = operatorStack.Pop();

            // Do operation to construct a new expression
            Expression[] operands = new Expression[op.Arity];
            for (var i = 0; i < op.Arity; i++) {
                operands[op.Arity - 1 - i] = outputQueue.Pop();
            }
            var combined = op.Combine(operands);
            if (op is Token tok) {
                combined.Tag<File>(tok.Source); combined.Tag<Position>(tok.StartPosition);
            }
            outputQueue.Push(combined);
        }
        if (outputQueue.Count == 0) {
            throw new ParserException(first.Source, first.StartPosition, "Missing expression value.");
        } else if (outputQueue.Count > 1) {
            throw new ParserException(first.Source, first.StartPosition, "Missing operator between expression values.");
        }
        return outputQueue.Pop();
    }

    private Expression parseBracketedExpression(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not OpenParenthesis)
            throw new ParserException(next.Source, next.StartPosition, "Bracketed expressions must start with a parenthesis.");
        stream.Advance();

        var expr = parseExpression(unit, func, stream);
        //expr.Tag<File>(next.Source); expr.Tag<Position>(next.StartPosition);

        next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        if (next is not CloseParenthesis)
            throw new ParserException(next.Source, next.StartPosition, "Bracketed expressions must end with a parenthesis.");
        stream.Advance();

        return expr;
    }

    private IVariableDeclaration getVar(TranslationUnit unit, FunctionDeclaration func, Identifier ident) {
        var local = (IVariableDeclaration?)func.Locals.Where(local => local.Name.Value == ident.Text).FirstOrDefault();
        if (local == null)
            local = (IVariableDeclaration?)func.FormalArguments.Where(arg => arg.Name.Value == ident.Text).FirstOrDefault();
        if (local == null)
            local = (IVariableDeclaration?)unit.Where(decl => decl is StaticVariableDeclaration global && global.Name.Value == ident.Text).FirstOrDefault();
        if (local == null)
            throw new ParserException(ident.Source, ident.StartPosition, $"Variable `{ident.Text}` is not declared in the visible scope.");
        return local;
    }

    public Expression? parseAtom(TranslationUnit unit, FunctionDeclaration func, TokenStream stream) {
        /* → ( expr )
           → IDENT | IDENT [ expr ] | IDENT ( args ) | len(IDENT) | sizeof(IDENT)
           → BOOL_LIT | INT_LIT  | FLOAT_LIT | NEW type_spec [ expr ]
        */
        var next = stream.Peek(0);
        if (next == null)
            throw new EndOfStreamException();
        switch (next) {
            case OpenParenthesis:
                // bracketed-expression
                return parseBracketedExpression(unit, func, stream);
            case LengthKeyword len: {
                    // len
                    stream.Advance();
                    // (
                    next = stream.Peek(0);
                    if (next == null)
                        throw new EndOfStreamException();
                    if (next is not OpenParenthesis)
                        throw new ParserException(next.Source, next.StartPosition, "Length expression must be followed by a parenthesis.");
                    stream.Advance();   

                    // IDENT
                    var ident = parseIdentifier(stream);

                    // )
                    next = stream.Peek(0);
                    if (next == null)
                        throw new EndOfStreamException();
                    if (next is not CloseParenthesis)
                        throw new ParserException(next.Source, next.StartPosition, "Length expression must end in a parenthesis.");
                    stream.Advance();

                    var e = new LengthExpression(getVar(unit, func, ident));
                    e.Tag<File>(len.Source); e.Tag<Position>(len.StartPosition);
                    return e;
                }
            case SizeKeyword size:{
                    // sizeof
                    stream.Advance();
                    // (
                    next = stream.Peek(0);
                    if (next == null)
                        throw new EndOfStreamException();
                    if (next is not OpenParenthesis)
                        throw new ParserException(next.Source, next.StartPosition, "Length expression must be followed by a parenthesis.");
                    stream.Advance();   

                    // IDENT
                    var ident = parseIdentifier(stream);

                    // )
                    next = stream.Peek(0);
                    if (next == null)
                        throw new EndOfStreamException();
                    if (next is not CloseParenthesis)
                        throw new ParserException(next.Source, next.StartPosition, "Length expression must end in a parenthesis.");
                    stream.Advance();

                    var e = new SizeOfExpression(getVar(unit, func, ident));
                    e.Tag<File>(size.Source); e.Tag<Position>(size.StartPosition);
                    return e;
                }
            case Identifier id:
                // One of many different possible things
                next = stream.Peek(1);
                if (next == null) {
                    // Simple identifier
                    stream.Advance();
                    var e = new LoadVarExpression(getVar(unit, func, id));
                    e.Tag<File>(id.Source); e.Tag<Position>(id.StartPosition);
                    return e;
                } else if (next is OpenBracket) {
                    // Array index
                    stream.Advance();
                    stream.Advance();

                    var index = parseExpression(unit, func, stream);

                    var close = stream.Peek(0);
                    if (close == null)
                        throw new EndOfStreamException();
                    if (close is not CloseBracket) 
                        throw new ParserException(close.Source, close.StartPosition, "Missing closing ] on array index.");
                    stream.Advance();

                    var e = new LoadArrayElementExpression(getVar(unit, func, id), index);
                    e.Tag<File>(id.Source); e.Tag<Position>(id.StartPosition);
                    return e;
                } else if (next is OpenParenthesis) {
                    // Function call
                    return parseFunctionCall(unit, func, stream);
                } else {
                    // Simple identifier
                    stream.Advance();
                    var e = new LoadVarExpression(getVar(unit, func, id));
                    e.Tag<File>(id.Source); e.Tag<Position>(id.StartPosition);
                    return e;
                }
            case BooleanLiteralToken b: {
                stream.Advance();
                var e = new LiteralIntExpression(b.Value ? 1 : 0);
                e.Tag<File>(b.Source); e.Tag<Position>(b.StartPosition);
                return e;
            }
            case IntegerLiteralToken i: {
                stream.Advance();
                var e = new LiteralIntExpression(i.Value);
                e.Tag<File>(i.Source); e.Tag<Position>(i.StartPosition);
                return e;
            }
            case UIntegerLiteralToken u: {
                stream.Advance();
                var e = new LiteralUIntExpression(u.Value);
                e.Tag<File>(u.Source); e.Tag<Position>(u.StartPosition);
                return e;
            }
            case FloatLiteralToken f: {
                stream.Advance();
                var e = new LiteralFloatExpression(f.Value);
                e.Tag<File>(f.Source); e.Tag<Position>(f.StartPosition);
                return e;
            }
            case StringToken str: {
                stream.Advance();
                var e = new LiteralStringExpression(str.Text);
                e.Tag<File>(str.Source); e.Tag<Position>(str.StartPosition);
                return e;
            }
            case NewKeyword n: {
                stream.Advance();
                var simpleType = parseSimpleTypeSpec(unit, stream);
                if (simpleType is not ValueTypeSpecifier elementType)
                    throw new ParserException(next.Source, next.StartPosition, $"Values of type '{simpleType}' cannot be stored within an array");
                
                // [
                var openNew = stream.Peek(0);
                if (openNew == null)
                    throw new EndOfStreamException();
                if (openNew is not OpenBracket) 
                    throw new ParserException(openNew.Source, openNew.StartPosition, "Missing opening [ on array size definition.");
                stream.Advance();

                var elementCount = parseExpression(unit, func, stream); 

                // ]
                var closeNew = stream.Peek(0);
                if (closeNew == null)
                    throw new EndOfStreamException();
                if (closeNew is not CloseBracket) 
                    throw new ParserException(closeNew.Source, closeNew.StartPosition, "Missing closing ] on array size definition.");
                stream.Advance();

                var e = new NewArrayExpression(elementType, elementCount);
                e.Tag<File>(n.Source); e.Tag<Position>(n.StartPosition);
                return e;
            }
            default:
                //throw new ParserException(next.Source, next.StartPosition, "Expected one of IDENTIFIER, BOOLEAN, INTEGER, UNSIGNED INTEGER, FLOAT, or INSTANTIATION.");
                return null;
        }
    }
}