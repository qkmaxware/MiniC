---
group: Language
---
# Basic Types
Mini-C contains many different basic types. These types are similar to those found in other languages.

## Void
The void or *nothing* type is used to represent places where no value is expected. These are used mainly on function declarations to indicate that a function does not return a value. 

## Int
The int type is used to represent 32bit signed integers. Integers can store values between –2,147,483,648 and 2,147,483,647. 

## Unsigned Int
The uint type is used to represent 32bit unsigned (positive only) integers. Unsigned integers can store values between 0 and 4,294,967,295.

## Float
The float type is used to represent real numbers using 32bit floating point precision. Floats can store decimal values between -3.40282347E+38 and 3.40282347E+38. As with all floating point precision types, not all real numbers can be represented between the max and min float values.

## Char
The character type is used to store letters. Technically this type is compiled down to an integer and as such shares the same behaviors and restrictions as integers do.

## Arrays
Arrays are the last type supported in Mini-C. Arrays represent contiguous segment of memory containing multiple values one after the other. Arrays can contain values of any of the other data types. 