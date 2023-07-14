# Mini-C Compiler
A compiler for a language similar to C but with a reduced feature set (and a few handy conveniences) dubbed Mini-C. Designed for use in a compiler course for students to implement within the confines of a single semester while considering the course-load of a full-time student.

The Mini-C reference compiler in this repo includes a parser for the Mini-C language, a few semantic validation passes, and a code emitter to produce bytecode compatible with my other project [Qkmaxware.Vm](https://github.com/qkmaxware/Qkmaxware.Vm) as a virtual machine for executing the bytecode. 

This is a **work-in-progress** language and as such the specific syntax and features are subject to change. 

## Syntax
The syntax of the Mini-C language is similar to that of the C language but with some distinct differences. There are no raw pointers and unlike C, [] for array types come after the type rather than after the identifier. The full syntax of the Mini-C language is described in the [grammar document](C.Web/docs/Grammar.md).

There are only a few basic types including: void, int, uint, float, char. Boolean values are just represented by integers as with C where 0 represents false and any other value is true. The keywords `true` and `false` therefor map to the values 1 and 0 respectively. Additionally, arrays can be used to store collections of values. Arrays can contain any of the value types (not void) and are always allocated to the heap. To learn more about the basic types you can read about them in the [types document](C.Web/docs/Types.md).

Like with C, memory management is manually managed. The only type that exists on the heap is the array. Everything else exists on the stack and will be cleaned up when a method is returned from. Failure to free used heap memory will result in a memory leak. Allocation of space on the heap is done using the `new` keyword and deleted with the `free` statement. To read more about memory management see the [memory management document](C.Web/docs/Memory%20Management.md).

## Preprocessor
The reference compiler provided here for Mini-C also includes a preprocessor and a few basic preprocessor directives. The most important of which is the `#include` directive which is used to inject the contents of another file replacing the directive. To read more about the preprocessor directives see [here](C.Web/docs/Directives.md).

To support the `#include` directive, a very tiny standard library (written in Mini-C as well) is included in the reference compiler. All of the source files for the standard library can be found [here](C/src/StandardLibrary/).

## Online Demo
An online Blazor web-app is provided for people to experiment with. You can access the app by visiting the github pages for this repository [here](https://qkmaxware.github.io/MiniC/).