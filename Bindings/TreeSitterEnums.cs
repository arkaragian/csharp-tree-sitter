using System.Runtime.InteropServices;

//namespace GitHub.TreeSitter
namespace TreeSitter.CSharp;

//csharp does not like adding the enum name as a prefix to the enum members.
//That is logical. However since the tree-sitter project defines the bindings
//like that. There is little we can do other than mutting the warnings.
#pragma warning disable CA1712
public enum TSInputEncoding {
    TSInputEncodingUTF8,
    TSInputEncodingUTF16
}

public enum TSSymbolType {
    TSSymbolTypeRegular,
    TSSymbolTypeAnonymous,
    TSSymbolTypeAuxiliary,
}

public enum TSLogType {
    TSLogTypeParse,
    TSLogTypeLex,
}

public enum TSQuantifier {
    TSQuantifierZero = 0, // must match the array initialization value
    TSQuantifierZeroOrOne,
    TSQuantifierZeroOrMore,
    TSQuantifierOne,
    TSQuantifierOneOrMore,
}

public enum TSQueryPredicateStepType {
    TSQueryPredicateStepTypeDone,
    TSQueryPredicateStepTypeCapture,
    TSQueryPredicateStepTypeString,
}

public enum TSQueryError {
    TSQueryErrorNone = 0,
    TSQueryErrorSyntax,
    TSQueryErrorNodeType,
    TSQueryErrorField,
    TSQueryErrorCapture,
    TSQueryErrorStructure,
    TSQueryErrorLanguage,
}
#pragma warning restore CA1712

[StructLayout(LayoutKind.Sequential)]
public struct TSPoint {
    public uint row;
    public uint column;

    public TSPoint(uint row, uint column) {
        this.row = row;
        this.column = column;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct TSRange {
    public TSPoint start_point;
    public TSPoint end_point;
    public uint start_byte;
    public uint end_byte;
}

[StructLayout(LayoutKind.Sequential)]
public struct TSInputEdit {
    public uint start_byte;
    public uint old_end_byte;
    public uint new_end_byte;
    public TSPoint start_point;
    public TSPoint old_end_point;
    public TSPoint new_end_point;
}

[StructLayout(LayoutKind.Sequential)]
public struct TSQueryCapture {
    public TSNode node;
    public uint index;
}

[StructLayout(LayoutKind.Sequential)]
public struct TSQueryMatch {
    public uint id;
    public ushort pattern_index;
    public ushort capture_count;
    public IntPtr captures;
}

[StructLayout(LayoutKind.Sequential)]
public struct TSQueryPredicateStep {
    public TSQueryPredicateStepType type;
    public uint value_id;
}

public delegate void TSLogger(TSLogType logType, string message);