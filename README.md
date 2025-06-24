# csharp-tree-sitter

## Introduction
This module provides C# bindings to the
[tree-sitter](https://github.com/tree-sitter/tree-sitter) parsing library,
which can enable c# developers be able to invoke the tree-sitter libraries
through P/Invoke from their c# code.

## Using the Bindings

In order to use the we need two two components:

1. A build of the C# bindings to do this call `dotnet build` in the root of
   this repository.
2. A build of a tree sitter.
3. A dll of a parser implementation

## Cloning

This repo includes the needed tree-sitter repos as submodules.  Remember to use
the `--recursive` option with git clone.

```cmd
git clone https://github.com/tree-sitter/csharp-tree-sitter.git --recursive
```

## Building

Requirements:
- Windows-only (the Makefile in the `tree-sitter` has OS-specific stuff in it so far)
- .NET 7

We'll first need to build the dependencies, and then the C# project.

- get dependencies built
  - `cd tree-sitter`
  - `nmake`
  - `cd ..`
- `dotnet build csharp-tree-sitter.csproj`

## Testing

A good demo is the following, it is a test written in c# which walks the AST tree in post order by calling `tree-sitter-cpp` parser with these bindings.

Assuming you're in VS Code:
- navigate to a C++ file to be parsed
- press `F5` to run with the default 'launch' configuration

Otherwise, you may manually run with:

```cmd
csharp-tree-sitter.exe -files [your test cpp files]
```

TODO: continue here

## Contributing

TODO: Explain how other users and developers can contribute to make your code better.
