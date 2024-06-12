using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed class TSQuery : IDisposable {
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

    public uint pattern_count() { return ts_query_pattern_count(Ptr); }
    public uint capture_count() { return ts_query_capture_count(Ptr); }
    public uint string_count() { return ts_query_string_count(Ptr); }
    public uint start_offset_for_pattern(uint patternIndex) { return ts_query_start_byte_for_pattern(Ptr, patternIndex) / sizeof(ushort); }
    public IntPtr predicates_for_pattern(uint patternIndex, out uint length) { return ts_query_predicates_for_pattern(Ptr, patternIndex, out length); }
    public bool is_pattern_rooted(uint patternIndex) { return ts_query_is_pattern_rooted(Ptr, patternIndex); }
    public bool is_pattern_non_local(uint patternIndex) { return ts_query_is_pattern_non_local(Ptr, patternIndex); }
    public bool is_pattern_guaranteed_at_offset(uint offset) { return ts_query_is_pattern_guaranteed_at_step(Ptr, offset / sizeof(ushort)); }
    public string capture_name_for_id(uint id, out uint length) { return Marshal.PtrToStringAnsi(ts_query_capture_name_for_id(Ptr, id, out length)); }
    public TSQuantifier capture_quantifier_for_id(uint patternId, uint captureId) { return ts_query_capture_quantifier_for_id(Ptr, patternId, captureId); }
    public string string_value_for_id(uint id, out uint length) { return Marshal.PtrToStringAnsi(ts_query_string_value_for_id(Ptr, id, out length)); }
    public void disable_capture(string captureName) { ts_query_disable_capture(Ptr, captureName, (uint)captureName.Length); }
    public void disable_pattern(uint patternIndex) { ts_query_disable_pattern(Ptr, patternIndex); }

    #region PInvoke
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_delete(IntPtr query);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_query_pattern_count(IntPtr query);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_query_capture_count(IntPtr query);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_query_string_count(IntPtr query);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_query_start_byte_for_pattern(IntPtr query, uint patternIndex);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_query_predicates_for_pattern(IntPtr query, uint patternIndex, out uint length);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_query_is_pattern_rooted(IntPtr query, uint patternIndex);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_query_is_pattern_non_local(IntPtr query, uint patternIndex);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_query_is_pattern_guaranteed_at_step(IntPtr query, uint byteOffset);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_query_capture_name_for_id(IntPtr query, uint id, out uint length);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSQuantifier ts_query_capture_quantifier_for_id(IntPtr query, uint patternId, uint captureId);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_query_string_value_for_id(IntPtr query, uint id, out uint length);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_disable_capture(IntPtr query, [MarshalAs(UnmanagedType.LPUTF8Str)] string captureName, uint captureNameLength);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_disable_pattern(IntPtr query, uint patternIndex);
    #endregion
}