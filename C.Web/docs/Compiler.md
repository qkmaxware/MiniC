---
group: Introduction
---
# The Compiler
The Mini-C compiler will convert code written in the Mini-C language into [Qkmaxware.Vm Bytecode](https://github.com/qkmaxware/Qkmaxware.Vm). This bytecode is very simple and the virtual machine for the bytecode is quite simple as well. As such students could make their own bytecode interpreter as well the the compiler. 

## Bytecode Modules
Each file is compiled into a single executable bytecode file referred to as a module. Bytecode modules are binary files with a special bytecode format. You can read more about the structure of a Qkmaxware.Vm bytecode module online [here](https://github.com/qkmaxware/Qkmaxware.Vm/blob/root/Qkmaxware.Vm.Console/docs/Module%20Structure.md).

## Supported Instructions
You can read all about the supported instructions as well as how they work by using the code found online [here](https://github.com/qkmaxware/Qkmaxware.Vm/tree/root/Qkmaxware.Vm/src/Instructions).
