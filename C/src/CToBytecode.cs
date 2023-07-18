using Qkmaxware.Languages.C.Text;
using Qkmaxware.Vm;
using Qkmaxware.Vm.Instructions;

namespace Qkmaxware.Languages.C;

public class CToBytecode {

    public (Vm.Module, Dictionary<Breakpoint, long>) ConvertWithBreakpoints(TranslationUnit unit, List<Breakpoint> breakpoints) { 
        using var builder = new ModuleBuilder();

        // Write the header
        writeHeader(builder, unit, null); 

        // Encode statements
        Label? main = null;
        var stmts = new BytecodeBuilder(builder);
        stmts.Breakpoints = breakpoints;
        foreach (var declaration in unit) {
            if (declaration is FunctionDeclaration funcdef) {
                var l = stmts.Visit(funcdef);
                if (funcdef == unit.Main) {
                    main = l;
                }
            } else {
                declaration.Visit(stmts);
            }
        }
        
        // Redo header
        builder.RewindStream(0);
        writeHeader(builder, unit, main);
        
        return (builder.ToModule(), stmts.BreakpointMap);
    }

    public Vm.Module Convert(TranslationUnit unit) {
        using var builder = new ModuleBuilder();

        // Write the header
        writeHeader(builder, unit, null); 

        // Encode statements
        Label? main = null;
        var stmts = new BytecodeBuilder(builder);
        foreach (var declaration in unit) {
            if (declaration is FunctionDeclaration funcdef) {
                var l = stmts.Visit(funcdef);
                if (funcdef == unit.Main) {
                    main = l;
                }
            } else {
                declaration.Visit(stmts);
            }
        }
        
        // Redo header
        builder.RewindStream(0);
        writeHeader(builder, unit, main);
        
        return builder.ToModule();
    }

    private void writeHeader(ModuleBuilder builder, TranslationUnit unit, Label? mainLocation = null) {
        if (!unit.IsExecutable || unit.Main == null) {
            builder.Exit(0);
            return;
        }

        // Call the main method
        if (mainLocation == null) {
            builder.Call(0, 0); // No main method... yet. Just fill in a temp value for now.
        } else {
            builder.Call(mainLocation, 0);
        }
        
        // Handle the return type
        //if (unit.Main.ReturnType != Void.Instance) {
            // If its got a return, remove it
            //builder.PopStackTop();
        //}
        // Remove the return value
        builder.PopStackTop();

        // Finally exit
        builder.Exit(0);
    }
}

class BytecodeBuilder : IDeclarationVisitor, IStatementVisitor, IExpressionVisitor, IOperatorVisitor {
    private ModuleBuilder builder;
    public BytecodeBuilder(ModuleBuilder builder) {
        this.builder = builder;
    }

    public List<Breakpoint>? Breakpoints;
    public Dictionary<Breakpoint, long> BreakpointMap {get; private set;} = new Dictionary<Breakpoint, long>();
    private void checkBreakpoint(AstNode node) {
        if (Breakpoints == null)
            return;

        File? src; Position? pos;
        if (node.TryGetTag<File>(out src) && node.TryGetTag<Position>(out pos)) {
            // If there is a breakpoint for this, mark it's position
            foreach (var breakpoint in Breakpoints) {
                if (breakpoint.File == src && breakpoint.LineNumber == pos.LineNumber && !BreakpointMap.ContainsKey(breakpoint)) {
                    BreakpointMap[breakpoint] = builder.Anchor();
                }
            }
        } 
    }

    private Dictionary<FunctionDeclaration, Label> labels = new Dictionary<FunctionDeclaration, Label>();
    private Dictionary<FunctionDeclaration, List<Action<Label>>> function_label_thunks = new Dictionary<FunctionDeclaration, List<Action<Label>>>();
    public void Accept(LocalVariableDeclaration decl) {
        throw new NotImplementedException();
    }

    public void Accept(StaticVariableDeclaration decl) {
        checkBreakpoint(decl);
        var @ref = builder.AddStatic(Operand.From(0));
        decl.Tag(@ref);
    }

    public void Accept(EnumDeclaration decl) { /*does nothing*/ } 

    public Label Visit(FunctionDeclaration decl) {
        checkBreakpoint(decl);
        Accept(decl);
        return labels[decl];
    }
    public void Accept(FunctionDeclaration decl) {
        checkBreakpoint(decl);
        // DONE
        this.builder.ExportSubprogram(decl.Name.Value);
        var label = this.builder.Label(decl.Name.Value);
        labels[decl] = label;

        List<Action<Label>>? thunks;
        if (function_label_thunks.TryGetValue(decl, out thunks)) {
            foreach (var action in thunks)
                action(label);
            function_label_thunks.Remove(decl);
        }
        // Do locals (simply allocate stack space)
        foreach (var local in decl.Locals) {
            switch (local.Type) {
                case Float f:   builder.PushFloat32(0f); break;
                default:        builder.PushInt32(0); break;
            }
        } 
        // Do body
        this.Accept(decl.Body);
        
        // Fallback return in case none was provided (in the case of void methods this is common)
        Accept(new ReturnStatement());
    }

    private void labelOf(FunctionDeclaration decl, Action<Label> action) {
        Label? label;
        if (labels.TryGetValue(decl, out label)) {
            action(label);
        } else {
            List<Action<Label>>? thunk;
            if (function_label_thunks.TryGetValue(decl, out thunk)) {
                thunk.Add(action);
            } else {
                thunk = new List<Action<Label>>();
                thunk.Add(action);
                function_label_thunks[decl] = thunk;
            }
        }
    }

    public void Accept(AsmStatement stmt) {
        // DONE 
        checkBreakpoint(stmt);
        var assembler = new Vm.Assembly.Asm1xParser();
        using var reader = new StringReader(stmt.Assembly);
        var submodule = assembler.Parse(reader);
        builder.Append(submodule);
    }

    public void Accept(AssignmentStatement stmt) {
        // TODO 
        checkBreakpoint(stmt);
        switch (stmt.Variable) {
            case LocalVariableDeclaration local:
                stmt.Value.Visit(this);
                int local_index = local.LocalIndex;
                builder.StoreLocal(local_index);
                break;
            case StaticVariableDeclaration global:
                // TODO 
                // Load address to global
                // Load the value to save
                var index = global.StaticIndex;
                stmt.Value.Visit(this);
                builder.AddInstruction("store_static", Operand.From(global.StaticIndex));
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Accept(ArrayAssignmentStatement stmt) {
        if (stmt.Variable == null)
            throw new ArgumentException("Cannot index element from null variable");
        checkBreakpoint(stmt);
        // Pointer on stack
        new LoadVarExpression(stmt.Variable).Visit(this);
        // Offset on stack
        stmt.Index.Visit(this);
        // Value on stack
        stmt.Value.Visit(this);

        builder.AddInstruction("set_element");
    }

    public void Accept(CompoundStatement stmts) {
        // DONE
        checkBreakpoint(stmts);
        foreach (var stmt in stmts) {
            stmt.Visit(this); // Do whatever the sub-statements need to do
        }
    }

    public void Accept(IfStatement stmt) {
        // DONE 
        /*
            .if_block
                .first_condition
                    ...
                    goto_if_zero .second_condition
                    ...
                    goto .if_end
                .second_condition
                    ...
                    goto_if_zero .third_condition
                    ...
                    goto .if_end
                ...
                .last_condition
                    goto_if_zero .if_end
                    ...
                    goto .if_end
            .if_end
        */

        // Create initial code
        checkBreakpoint(stmt);
        var if_block = builder.Anchor();
        var if_end = if_block;
        var branch_anchors = new List<(long start, long false_jump, long true_jump, long end)>();
        foreach (var branch in stmt.Branches) {
            var branch_anchor = builder.Anchor();

            // Evaluate condition
            branch.Condition.Visit(this);

            // Check condition and branch if false
            var jump_anchor = builder.Anchor();
            builder.GotoIfStackTopZero(0); // FIX 1: Temporarily set this to 0 to fix later

            // Do body
            this.Accept(branch.Branch);
            var branch_end = builder.Anchor();
            builder.Goto(0);               // FIX 2: Temporarily set this to 0 to fix later
            branch_anchors.Add((start: branch_anchor, false_jump: jump_anchor, true_jump: branch_end, end: builder.Anchor()));
        }
        if_end = builder.Anchor();

        // Fix labels
        for (var i = 0; i < branch_anchors.Count; i++) {
            // Fix condition jump
            var now = builder.Anchor();
            var next_anchor = ((i + 1) < branch_anchors.Count) ? branch_anchors[i + 1].start : if_end;
            builder.RewindStream(branch_anchors[i].false_jump);
            builder.GotoIfStackTopZero(next_anchor); // FIX 1: Fill in jump to next branch

            // Fix end jump
            builder.RewindStream(branch_anchors[i].true_jump);
            builder.Goto(if_end); // FIX 2: Fill in jump to the end of the if statement
            builder.RewindStream(now);
        }
    }

    public void Accept(ReturnStatement stmt) {
        // DONE
        checkBreakpoint(stmt);
        if (stmt.ReturnedValue == null) {
            builder.PushInt32(0);
            builder.ReturnResult();
        } else {
            stmt.ReturnedValue.Visit(this);
            builder.ReturnResult();
        }
    }

    class LoopData {
        public long Start;
        public long End;
        public List<long> BreakAddresses = new List<long>();
    }
    private Stack<LoopData> loops = new Stack<LoopData>();
    public void Accept(WhileStatement stmt) {
        // DONE
        /*
            .loop_start
                condition
                goto_if_zero .loop_end
                ...
                goto .loop_start
            .loop_end
        */
        // Loop start
        checkBreakpoint(stmt);
        var data = new LoopData();
        var loop_start_anchor = builder.Anchor();
        data.Start = loop_start_anchor;
        loops.Push(data);
        stmt.Condition.Visit(this);
        
        var condition_exit_anchor = builder.Anchor();
        builder.GotoIfStackTopZero(loop_start_anchor);

        // Loop body
        this.Accept(stmt.Body);
        builder.Goto(loop_start_anchor);

        // Loop end (replace the failure branch to go to the end of the loop)
        var end_anchor = builder.Anchor();
        data.End = end_anchor;
        builder.RewindStream(condition_exit_anchor);
        builder.GotoIfStackTopZero(end_anchor);
        builder.RewindStream(end_anchor);

        // Repair any breaks
        foreach (var @break in data.BreakAddresses) {
            builder.RewindStream(@break);
            builder.Goto(data.End); // Fix break here
            builder.RewindStream(end_anchor);
        }
        loops.Pop(); // remove the data
    }

    public void Accept(BreakStatement stmt) {
        LoopData? data;
        checkBreakpoint(stmt);
        if (loops.TryPeek(out data)) {
            data.BreakAddresses.Add(builder.Anchor());
            builder.Goto(data.Start); // FIX break later
        } else {
            throw new FormatException("Break statements must be contained within a loop.");
        }
    }

    public void Accept(ContinueStatement stmt) {
        LoopData? data;
        checkBreakpoint(stmt);
        if (loops.TryPeek(out data)) {
            builder.Goto(data.Start);
        } else {
            throw new FormatException("Continue statements must be contained within a loop.");
        }
    }

    public void Accept(ExprStatement stmt) {
        checkBreakpoint(stmt);
        stmt.Expression.Visit(this);
        builder.PopStackTop(); // remove the expression value from the stack (keep the stack clean)
    }

    // -----------------------------------------------------------------------------------------------------------

    private TypeSpecifier? exprResultType;

    public void Accept(LoadVarExpression expr) {
        // TODO 
        checkBreakpoint(expr);
        if (expr.Variable != null) {
            switch (expr.Variable) {
                case FormalArgument arg:
                    int arg_index = arg.Index;
                    builder.PushArgument(arg_index);
                    break;
                case LocalVariableDeclaration local:
                    int local_index = local.LocalIndex;
                    builder.PushLocal(local_index);
                    break;
                case StaticVariableDeclaration global:
                    var static_index = global.StaticIndex;
                    builder.AddInstruction("load_static", Operand.From(static_index));
                    break;
                default:
                    throw new NotImplementedException();
            }
            exprResultType = expr.Variable.Type;
        } else {
            throw new ArgumentException("Missing variable reference");
        }
    }

    public void Accept(LoadArrayElementExpression expr) {
        if (expr.Variable == null)
            throw new ArgumentException("Cannot index element from null variable");

        // Put the pointer on top of the stack
        checkBreakpoint(expr);
        new LoadVarExpression(expr.Variable).Visit(this);
        // Put the index on the stack
        expr.Index.Visit(this);
        // Call the specific function
        builder.AddInstruction("get_element");
        exprResultType = ((Array)expr.Variable.Type).ElementType;
    }

    public void Accept(LoadEnumConstant expr) {
        builder.PushInt32(expr.Constant.Value);
    }

    private int sizeOf(ValueTypeSpecifier type) => 4; // Each element is size 4 in the bytecode

    public void Accept(NewArrayExpression expr) {
        // Put size on the stack
        checkBreakpoint(expr);
        expr.Size.Visit(this);
        builder.PushInt32(sizeOf(expr.Type.ElementType)); 
        builder.MultiplyInt32(); // Size(bytes) = Size(elements) * Size(element_type)
        // Allocate
        builder.Allocate();
    }

    public void Accept(LiteralIntExpression expr) {
        checkBreakpoint(expr);
        builder.PushInt32(expr.Value);
        exprResultType = Integer.Instance;
    }

    public void Accept(LiteralUIntExpression expr) {
        checkBreakpoint(expr);
        builder.PushUInt32(expr.Value);
        exprResultType = UnsignedInteger.Instance;
    }
    
    public void Accept(LiteralFloatExpression expr) {
        checkBreakpoint(expr);
        builder.PushFloat32(expr.Value);
        exprResultType = Float.Instance;
    }

    public void Accept(LiteralStringExpression expr) {
        checkBreakpoint(expr);
        // Option 1, use constants
        //var @const = builder.AddConstantUtf8String(expr.Value);
        //builder.PushConstant(@const);
        // Option 2, use heap (consistent with other arrays)
        var size = expr.Value.Length * sizeOf(Char.Instance);
        builder.Allocate(size);                         // allocate space
        for (var i = 0; i < expr.Value.Length; i++) {   // add each character
            builder.DuplicateStackTop();
            builder.SetArrayElement(i, expr.Value[i]);
        }
        // top of stack is the string address for returning
        exprResultType = Array.StringType;
    }

    public void Accept(CallExpression expr) {
        checkBreakpoint(expr);
        if (expr.Arguments.Count != expr.Function.FormalArguments.Count)
            throw new ArgumentException("Argument count mismatch");

        // Evaluate arguments in order
        foreach (var arg in expr.Arguments) {
            arg.Visit(this);
        }
        // Call the procedure
        var location = builder.Anchor();
        builder.Call(0, expr.Arguments.Count);          // FIX 1 fill temp value so we have the right bytecode size
        this.labelOf(expr.Function, (label) => {
            var now = builder.Anchor();
            builder.RewindStream(location);
            builder.Call(label, expr.Arguments.Count);  // FIX 1 replace temp value
            builder.RewindStream(now);
        });
        // Clear the arguments
        for (var i = 0; i < expr.Function.FormalArguments.Count; i++) {
            builder.SwapStackTop();
            builder.PopStackTop();
        }
    }

    public void Accept(OrExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Or?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The or operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(AndExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.And?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The and operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(XorExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Addition?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The xor operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(AndIntInt op) {
        builder.AndInt32();
    }
    public void Accept(AndUIntUInt op) {
        builder.AndUInt32();
    }

    public void Accept(OrIntInt op) {
        builder.OrInt32();
    }
    public void Accept(OrUIntUInt op) {
        builder.AndUInt32();
    }

    public void Accept(XorIntInt op) {
        builder.XorInt32();
    }
    public void Accept(XorUIntUInt op) {
        builder.XorUInt32();
    }

    public void Accept(AddExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Addition?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The + operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(AddIntInt op) {
        builder.AddInt32();
    }

    public void Accept(AddUIntUInt op) {
        builder.AddUInt32();
    }

    public void Accept(AddFloatFloat op) {
        builder.AddFloat32();
    }

    public void Accept(SubExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Subtraction?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The - operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(SubIntInt op) {
        builder.SubtractInt32();
    }

    public void Accept(SubUIntUInt op) {
        builder.SubtractUInt32();
    }

    public void Accept(SubFloatFloat op) {
        builder.SubtractFloat32();
    }

    public void Accept(MulExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Multiplication?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The * operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(MulIntInt op) {
        builder.MultiplyInt32();
    }

    public void Accept(MulUIntUInt op) {
        builder.MultiplyUInt32();
    }

    public void Accept(MulFloatFloat op) {
        builder.MultiplyFloat32();
    }

    public void Accept(DivExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Division?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The / operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(DivIntInt op) {
        builder.DivideInt32();
    }

    public void Accept(DivUIntUInt op) {
        builder.DivideUInt32();
    }

    public void Accept(DivFloatFloat op) {
        builder.DivideFloat32();
    }

    public void Accept(RemExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Remainder?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The % operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(RemIntInt op) {
        builder.RemainderInt32();
    }

    public void Accept(RemUIntUInt op) {
        builder.RemainderUInt32();
    }

    public void Accept(RemFloatFloat op) {
        builder.RemainderFloat32();
    }

    public void Accept(GtExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.GreaterThan?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The > operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(GtIntInt op) {
        builder.AddInstruction("set_gt_i32");
    }
    public void Accept(GtUIntUInt op) {
        builder.AddInstruction("set_gt_u32");
    }
    public void Accept(GtFloatFloat op) {
        builder.AddInstruction("set_gt_f32");
    }

    public void Accept(LtExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.LessThan?.Where(o => o.LhsType == lhsType && o.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The < operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(LtIntInt op) {
        builder.AddInstruction("set_lt_i32");
    }
    public void Accept(LtUIntUInt op) {
        builder.AddInstruction("set_lt_u32");
    }
    public void Accept(LtFloatFloat op) {
        builder.AddInstruction("set_lt_f32");
    }

    public void Accept(EqExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Equality?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The == operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(EqIntInt op) {
        builder.AddInstruction("set_eq_i32");
    }
    public void Accept(EqUIntUInt op) {
        builder.AddInstruction("set_eq_u32");
    }
    public void Accept(EqFloatFloat op) {
        builder.AddInstruction("set_eq_f32");
    }
    public void Accept(EqCharChar op) {
        builder.AddInstruction("set_neq_u32");
    }
    
    public void Accept(NeqExpression expr) {
        checkBreakpoint(expr);
        // Evaluate args
        expr.LhsOperand.Visit(this);
        var lhsType = exprResultType;
        expr.RhsOperand.Visit(this);
        var rhsType = exprResultType;
        
        // Get the operator
        var op = lhsType?.Operators?.Inequality?.Where(op => op.LhsType == lhsType && op.RhsType == rhsType)?.FirstOrDefault();
        if (op == null) {
            throw new NotImplementedException($"The != operator is not implemented between types {lhsType?.GetType()?.Name} and {rhsType?.GetType()?.Name}.");
        }

        // Do the operator
        op.Visit(this);

        // Store the result type
        exprResultType = op.ResultType;
    }

    public void Accept(NeqIntInt op) {
        builder.AddInstruction("set_neq_i32");
    }
    public void Accept(NeqUIntUInt op) {
        builder.AddInstruction("set_neq_u32");
    }
    public void Accept(NeqFloatFloat op) {
        builder.AddInstruction("set_neq_f32");
    }
    public void Accept(NeqCharChar op) {
        builder.AddInstruction("set_neq_u32");
    }

    public void Accept(LengthExpression expr) {
        checkBreakpoint(expr);
        expr.Loader.Visit(this);
        exprResultType = Integer.Instance;
        builder.ArrayLength();
    }

    public void Accept(SizeOfExpression expr) {
        checkBreakpoint(expr);
        expr.Loader.Visit(this);
        exprResultType = Integer.Instance;
        builder.ObjectSize();
    }
    
    public void Accept(FreeExpression expr) {
        checkBreakpoint(expr);
        expr.Loader.Visit(this); // Load var on top of stack
        builder.DuplicateStackTop();
        builder.Free(); // Free memory
        // Top of the stack is still the pointer (this is an expression after all)
    }
}