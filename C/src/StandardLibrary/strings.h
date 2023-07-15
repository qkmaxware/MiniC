#ifndef STD_STRINGS_H
#define STD_STRINGS_H

char[] int2str(int value) {
    char[] result;
    asm {
        load_arg 0
        i32_to_string
        store_local 0
    }
    return result;
}

char int2char(int value) {
    char result;
    asm {
        load_arg 0
        store_local 0
    }
    return result;
}

char[] uint2str(uint value) {
    char[] result;
    asm {
        load_arg 0
        u32_to_string
        store_local 0
    }
    return result;
}

char[] float2str(uint value) {
    char[] result;
    asm {
        load_arg 0
        f32_to_string
        store_local 0
    }
    return result;
}

char[] str_concat(char[] first, char[] second) {
    char[] result = new char[len(first) + len(second)];
    int i = 0;
    while (i < len(first)) {
        result[i] = first[i];
        i = i + 1;
    }
    i = 0;
    while (i < len(second)) {
        result[len(first) + i] = second[i];
        i = i + 1;
    }
    return result;
}

#endif
