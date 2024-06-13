using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed partial class TSQuery : IDisposable {
    internal IntPtr Ptr { get; private set; }

    public TSQuery(IntPtr ptr) {
        Ptr = ptr;
    }

    public void Dispose() {
        if (Ptr != IntPtr.Zero) {
            ts_query_delete(Ptr);
            Ptr = IntPtr.Zero;
        }
    }

    public uint PatternCount => ts_query_pattern_count(Ptr);
    public uint CaptureCount => ts_query_capture_count(Ptr);
    public uint StringCount => ts_query_string_count(Ptr);

    public uint StartOffsetForPattern(uint patternIndex) {
        return ts_query_start_byte_for_pattern(Ptr, patternIndex) / sizeof(ushort);
    }

    public IntPtr PredicatesForPattern(uint patternIndex, out uint length) {
        return ts_query_predicates_for_pattern(Ptr, patternIndex, out length);
    }

    public bool IsPatternRooted(uint patternIndex) {
        return ts_query_is_pattern_rooted(Ptr, patternIndex);
    }

    public bool IsPatternNonLocal(uint patternIndex) {
        return ts_query_is_pattern_non_local(Ptr, patternIndex);
    }

    public bool IsPatternGuaranteedAtOffset(uint offset) {
        return ts_query_is_pattern_guaranteed_at_step(Ptr, offset / sizeof(ushort));
    }

    public string? CaptureNameForId(uint id, out uint length) {
        return Marshal.PtrToStringAnsi(ts_query_capture_name_for_id(Ptr, id, out length));
    }

    public TSQuantifier CaptureQuantifierForId(uint patternId, uint captureId) {
        return ts_query_capture_quantifier_for_id(Ptr, patternId, captureId);
    }

    public string? StringValueForId(uint id, out uint length) {
        return Marshal.PtrToStringAnsi(ts_query_string_value_for_id(Ptr, id, out length));
    }

    public void DisableCapture(string captureName) {
        ts_query_disable_capture(Ptr, captureName, (uint)captureName.Length);
    }

    public void DisablePattern(uint patternIndex) {
        ts_query_disable_pattern(Ptr, patternIndex);
    }

    #region PInvoke
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_delete(IntPtr query);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_query_pattern_count(IntPtr query);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_query_capture_count(IntPtr query);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_query_string_count(IntPtr query);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_query_start_byte_for_pattern(IntPtr query, uint patternIndex);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_query_predicates_for_pattern(IntPtr query, uint patternIndex, out uint length);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_query_is_pattern_rooted(IntPtr query, uint patternIndex);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_query_is_pattern_non_local(IntPtr query, uint patternIndex);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_query_is_pattern_guaranteed_at_step(IntPtr query, uint byteOffset);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_query_capture_name_for_id(IntPtr query, uint id, out uint length);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSQuantifier ts_query_capture_quantifier_for_id(IntPtr query, uint patternId, uint captureId);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_query_string_value_for_id(IntPtr query, uint id, out uint length);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_disable_capture(IntPtr query, [MarshalAs(UnmanagedType.LPUTF8Str)] string captureName, uint captureNameLength);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_disable_pattern(IntPtr query, uint patternIndex);
    #endregion
}