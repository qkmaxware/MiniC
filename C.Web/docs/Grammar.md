---
group: Language
---
# Mini-C Grammar
The following BNF grammar specification describes the full syntax if the Mini-C programming language. Note, this language has undergone numerous changes during it's development and as such I cannot guarantee that there are not minor differences between the grammar and the actual compiler. 

<pre>
<code>
program			→ decl_list
decl_list		→ decl_list decl | decl
decl			→ type_spec IDENT ; | fun_decl
var_decl		→ type_spec IDENT ; | type_spec IDENT = expr ;
type_spec		→ simple_type | simple_type [ ]
simple_type     → VOID | INT | UINT | FLOAT | CHAR
fun_decl		→ type_spec IDENT ( params ) compound_stmt
params			→ param_list | VOID
param_list		→ param_list , param | param
param			→ type_spec IDENT 
stmt_list		→ stmt_list stmt | ε
stmt			→ local_decl | compound_stmt | if_stmt | while_stmt | 
				  return_stmt | break_stmt | continue_stmt | asm_stmt | expr_stmt
expr_stmt		→ IDENT = expr ; | IDENT [ expr ] = expr ; | IDENT ( args ) | free(IDENT) ;
while_stmt		→ WHILE ( expr ) compound_stmt
compound_stmt	→ { local_decls stmt_list }
local_decl		→ LOCAL type_spec IDENT ;
if_stmt			→ IF expr compound_stmt | IF expr compound_stmt ELSE compound_stmt 
return_stmt		→ RETURN ; | RETURN expr ;
break_stmt	    → BREAK ;
continue_stmt   → CONTINUE ;
asm_stmt        → ASM { ASSEMBLY_INSTRUCTION* }
expr			→ expr OR expr | expr AND expr
				→ expr EQ expr | expr NE expr 
				→ expr LE expr | expr < expr | expr GE expr  | expr > expr
				→ expr + expr | expr - expr 
				→ expr * expr | expr / expr  | expr % expr
				→ ! expr
				→ ( expr )
				→ IDENT | IDENT [ expr ] | IDENT ( args ) | len(IDENT) | sizeof(IDENT)
				→ BOOL_LIT | INT_LIT | UINT_LIT | FLOAT_LIT | NEW type_spec [ expr ]
arg_list		→ arg_list , expr | expr
args			→ arg_list | ε
</code>
</pre>