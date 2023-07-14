#ifndef STD_CONSOLE_H
#define STD_CONSOLE_H

char read_char() {
    char value;
    asm {
        readchar
        store_local 0
    }
    return value;
}

char[] read_ln() {
    int buffer_length = 10;
    char[] buffer = new char[buffer_length];
    int length = 0;
    char next = read_char();
    char[] newline ="\n";
    while (next != newline[0]) {
        buffer[length] = next;
        length = length + 1;
        // Buffer swap
        if (length > buffer_length - 1) {
            int new_buffer_length = buffer_length * 2;
            char[] swap_buffer = new char[new_buffer_length];
            int i = 0; 
            while (i < buffer_length) {
                swap_buffer[i] = buffer[i];
                i = i + 1;
            }
            char[] swap_temp = buffer;
            buffer = swap_buffer;
            free(swap_buffer);
        }
    }

    // Copy to output
    char[] str = new char[length];
    int j = 0; 
    while (j < length) {
        str[i] = buffer[i];
        i = i + 1;
    }

    // Cleanup memory
    free(buffer);
    free(newline);
    return str;
}

void print_char(char c) {
    asm {
        load_arg 0
        putchar
    }
    return;
}

void print_str(char[] string) {
    int l = len(string);
    int i = 0;
    while (i < l) {
        print_char(string[i]);
        i = i + 1;
    }
    return;
}

#endif