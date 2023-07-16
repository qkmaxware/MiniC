---
group: Language
---
# Control Flow
Control flow constructs allow for changing the flow of execution of a program based on some logic within the program itself. Most languages have many different methods of control flow, most of which exist as statements within the language. 

Mini-C has a very small set of control flow structures. 

## Selection Statements
Selection statements are used to select what code is executed next. Usually only one of the possible options is every selected at any given time. The only selection statement Mini-C supports is the traditional If/Else branching statement. The syntax for these statements are identical to how they exist in other languages, particularly of those with C-like syntax. You may provide multiple branches with **if/else** and a fallback branch with just **else**.

<pre><code>
if (x > 0) {
    doSomething();
} else if (x > 0) {
    doSomethingElse();
} else {
    doFallback();
}
</code></pre>

## Iterative Statements
Iterative statements are used to repeatedly run code, usually until a condition is met. Both FOR and WHILE loops are currently supported by Mini-C. However, the for loop is just syntactic sugar for a while loop. For loops can easily be decomposed into a while loop. So technically Mini-C only supports while loops, but there is syntax for a for loop.
<pre><code>
for (int i = 0; condition(i); i = i + 1) {
    doSomething();
}
</code></pre>
is the same as
<pre><code>
int i = 0;
while (condition(i)) {
    doSomething();
    i = i + 1;
}
</code></pre>

## Jump Statements
Jump statements are statements that immediately transfer execution to another spot in the code. Unlike C, Mini-C does not support any kind of arbitrary jump statement. Mini-C supports the break/continue statements which are to be used in conjunction with loops in order to influence the control flow of the loop's body. Both break and continue statements are standalone and are not used with any arguments. Mini-C also supports function calls as a method of control flow jumping. Functions can be called with any number of arguments being passed to them. 

<pre><code>
    callMyFunction();
</code></pre>