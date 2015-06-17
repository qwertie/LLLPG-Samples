# The Loyc LL(k) Parser Generator (LLLPG)

The source code is in the [Loyc repository](https://github.com/qwertie/Loyc/tree/master/Main/LLLPG).

Articles about LLLPG are published on CodeProject:
- Part 1: http://www.codeproject.com/Articles/664785/A-New-Parser-Generator-for-Csharp
- Part 2: http://www.codeproject.com/Articles/688152/The-Loyc-LL-k-Parser-Generator-Part-2
- Part 3: http://www.codeproject.com/Articles/732222/The-Loyc-LL-k-Parser-Generator-Part-3
- Part 4: http://www.codeproject.com/Articles/733426/The-Loyc-LL-k-Parser-Generator-Part-4

This repo contains four demos, in addition to the binaries in `LLLPG/`:
- Calculator demo (standalone)
- Calculator demo (based on Loyc libraries)
- Calculator demo (produces [Loyc trees](https://github.com/qwertie/LoycCore/wiki/Loyc-trees))
- Enhanced C# parser

LLLPG is Linux-compatible. For those without Visual Studio, you may find the built-in editor in LLLPG/LeMP.exe handy (to learn more about LeMP, [read this](http://www.codeproject.com/Articles/995264/Avoid-tedious-coding-with-LeMP-Part)). For those _with_ Visual Studio, you'll want to register the custom tool by running LLLPG/LoycFileGeneratorForVs.exe, and install syntax highlighting by running LLLPG/LoycSyntaxForVs.vsix. All demo projects were made for Visual Studio 2010 or later.
