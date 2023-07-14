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
Iterative statements are used to repeatedly run code, usually until a condition is met. Currently only iterative statement Mini-C supports is the while loop. This kind of loop will repeat code as long as a condition evaluates to true. The reason why the while loop is the only supported loop type at this time is because other loop types can be represented using while loops.
<pre><code>
while (condition) {
    doSomething();
}
</code></pre>

## Jump Statements
Jump statements are statements that immediately transfer execution to another spot in the code. Unlike C, Mini-C does not support any kind of arbitrary jump statement. Mini-C supports the break/continue statements which are to be used in conjunction with loops in order to influence the control flow of the loop's body. Both break and continue statements are standalone and are not used with any arguments. Mini-C also supports function calls as a method of control flow jumping. Functions can be called with any number of arguments being passed to them. 

<pre><code>
    callMyFunction();
</code></pre>