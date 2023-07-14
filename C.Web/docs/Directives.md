---
title: Preprocessor
group: Language
---
# Preprocessor Directives
The Mini-C compiler includes a simple preprocessor that is used to transform source code text before compilation. The reference Mini-C compiler used in both the downloaded and web version provided here only includes a small number of directives.

## Define
The define directive allows you to create variables within the preprocessor which can be used as conditions to other directives.
<pre><code>#define PI 3.1415</code></pre>

## Ifdef
The ifdef directive allows for code to be included in the compilation only if the defined variable exists in the preprocessor's variable collection.
<pre><code>#ifdef PI
    doSomethingIfPiDefined();
#endif</code></pre>

## Ifndef
The ifndef directive allows for code to be included in the compilation only if the variable does not exist in the preprocessor's variable collection.
<pre><code>#ifndef PI
    doSomethingIfPiNotDefined();
#endif</code></pre>
The most common use of the ifndef directive is the common include guard pattern allowing a file to be only included once in a compilation.
<pre><code>#ifndef MY_FILENAME
#define MY_FILENAME
    ...
#endif</code></pre>

## Include 
The most important directive is arguably the include directive. When processed, the entire contents of one file will be inserted into the current file at the position of the directive. You may use angled brackets or double quotes to surround file names. If angled brackets are used, the file is searched for in the standard library path and if double quotes are used, the file is searched for in the same directory as the current file. In the web compiler, since there only is one file (the one you are editing), using double quotes is pointless; however, there is a minimal standard library which can be included using the angled brackets syntax. You can read more about the standard library's functionality in the Standard Library document.
<pre><code>#include &lt;math.h&gt;</code></pre>