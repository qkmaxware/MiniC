---
group: Language
---
# Memory Management
The Mini-C language operates under a dual stack+heap memory management system.

## Stack
The stack is where all primitive values, int, uint, float, as well as all computation intermediate values are stored. The stack is a first-in, last out data-structure. Unlike the traditional definition of a stack, there is some degree of random access which is used to access "local" variables or variables declared within the current function. 

The stack is managed automatically. When a function returns it removes all values from its local stack frame.

## Heap
The heap is a random access linear memory structure in which larger, more complex, values are stored. Currently the only type of complex value that exists on the heap are arrays.


All memory is manually managed in Mini-C through the usage of the `new` and `free` keywords. New is used to allocate memory onto the heap. The exact size of memory allocated by the new expression depends on the type being initialized. For instance for the expression below would be 8 * 4 = 16 since there are 8 values and each value takes up 4 bytes of memory;
```
int[] values = new int[8];
```

Free is used to clear up used memory and allow that memory to be reused by other allocations. Free looks like a function call, but is in reality its own special statement type that takes in the identifier of a variable and frees it from memory. Only pointer-like variables are acceptable as arguments, in the current build this means only array variables names can be used. 
```
free(values);
```

**NOTICE** Since ALL arrays are on the heap that means even string literals are allocated to the heap and need to be cleaned up. 
```
char[] hello = "Hello World";
free(hello);
```
is analogous with the following sequence of assignments:
```
char[] hello = new char[11];
hello[0] = int2char(72);
hello[1] = int2char(101);
hello[2] = int2char(108);
hello[3] = int2char(108);
hello[4] = int2char(111);
hello[5] = int2char(32);
hello[6] = int2char(87);
hello[7] = int2char(111);
hello[8] = int2char(114);
hello[9] = int2char(108);
hello[10] = int2char(100);
free(hello);
```