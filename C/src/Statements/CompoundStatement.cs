using System.Collections;

namespace Qkmaxware.Languages.C;

/// <summary>
/// Base class for all statements
/// </summary>
public class CompoundStatement : Statement, IEnumerable<Statement> {
    private List<Statement> stmts = new List<Statement>();

    public CompoundStatement() {}

    public CompoundStatement(params Statement[] statements) {
        this.stmts.AddRange(statements);
    }

    public CompoundStatement(IEnumerable<Statement> statements) {
        this.stmts.AddRange(statements);
    }

    public void Add(params Statement[] statements) {
        stmts.AddRange(statements);
    }

    public override void Visit(IStatementVisitor visitor) => visitor.Accept(this);

    public IEnumerator<Statement> GetEnumerator() => stmts.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => stmts.GetEnumerator();
}

