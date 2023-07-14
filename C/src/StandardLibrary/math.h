#ifndef STD_MATH_H
#define STD_MATH_H

float pi() {
    float value;
    asm {
        @pi = 3.141592653589793
        load_const @pi
        store_local 0
    }
    return value;
}

float e() {
    float value;
    asm {
        @e = 2.718281828459045
        load_const @e
        store_local 0
    }
    return value;
}

int uint2int(uint value) {
    int result;
    asm {
        load_arg 0
        u32_to_i32
        store_local 0
    }
    return result;
}

int float2int(float value) {
    int result;
    asm {
        load_arg 0
        f32_to_i32
        store_local 0
    }
    return result;
}

uint int2uint(int value) {
    uint result;
    asm {
        load_arg 0
        i32_to_u32
        store_local 0
    }
    return result;
}

uint float2uint(float value) {
    uint result;
    asm {
        load_arg 0
        f32_to_u32
        store_local 0
    }
    return result;
}

float int2float(int value) {
    float result;
    asm {
        load_arg 0
        i32_to_f32
        store_local 0
    }
    return result;
}

float uint2float(uint value) {
    float result;
    asm {
        load_arg 0
        u32_to_f32
        store_local 0
    }
    return result;
}

#endif