---
title: Web Compiler
group: Usage
---
# Web Compiler Usage
The compiler provided on this website can be used to compile single file Mini-C programs. It remains up to date with the latest version of the Mini-C compiler. Simply write Mini-C code and you can then either run the compile the code and download the bytecode module, or you can run it directly in the browser. 

## Saving / Loading
The browser editor supports a basic saving and loading feature that leverages your browser's local storage. This means that all saved files are stored in your browser's cache and can be lost when your cache gets cleared. 

## Interaction
Unlike when running bytecode locally on your own machine, interaction with code running in the web-browser is more restrictive. You may provide a string of text as the values provided to the "standard-input" stream in the settings menu. When code runs, it can read the values from this standard-input string. 

## Output
All output from running and compiling code will appear on the bottom of the window in the OUTPUT section. If there are compiler errors they will also be displayed here. 

You may clear the output by pressing the small x located next to the OUTPUT title. Additionally, the output is automatically cleared each time a new compilation is performed. 

## Compiling and Running
The download button can be used to compile code to bytecode and download the resulting bytecode module. The play button can button can be used to compile code to bytecode and then run the code in the included bytecode virtual machine. 