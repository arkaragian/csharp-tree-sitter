using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed partial class TSQueryCursor : IDisposable {
    private IntPtr Ptr { get; set; }

    private TSQueryCursor(IntPtr ptr) {
        Ptr = ptr;
    }

    public TSQueryCursor() {
        Ptr = ts_query_cursor_new();
    }

    public void Dispose() {
        if (Ptr != IntPtr.Zero) {
            ts_query_cursor_delete(Ptr);
            Ptr = IntPtr.Zero;
        }
    }

    public void Exec(TSQuery query, TSNode node) {
        ts_query_cursor_exec(Ptr, query.Ptr, node);
    }

    public bool DidExceedMatchLimit() {
        return ts_query_cursor_did_exceed_match_limit(Ptr);
    }

    public uint MatchLimit() {
        return ts_query_cursor_match_limit(Ptr);
    }

    public void SetMatchLimit(uint limit) {
        ts_query_cursor_set_match_limit(Ptr, limit);
    }

    public void SetRange(uint start, uint end) {
        ts_query_cursor_set_byte_range(Ptr, start * sizeof(ushort), end * sizeof(ushort));
    }

    public void SetPointRange(TSPoint start, TSPoint end) {
        ts_query_cursor_set_point_range(Ptr, start, end);
    }

    public bool NextMatch(out TSQueryMatch match, out TSQueryCapture[]? captures) {
        captures = null;
        if (ts_query_cursor_next_match(Ptr, out match)) {
            if (match.capture_count > 0) {
                captures = new TSQueryCapture[match.capture_count];
                for (ushort i = 0; i < match.capture_count; i++) {
                    nint intPtr = match.captures + (Marshal.SizeOf(typeof(TSQueryCapture)) * i);
                    captures[i] = Marshal.PtrToStructure<TSQueryCapture>(intPtr);
                }
            }
            return true;
        }
        return false;
    }

    public void RemoveMatch(uint id) {
        ts_query_cursor_remove_match(Ptr, id);
    }

    public bool NextCapture(out TSQueryMatch match, out uint index) {
        return ts_query_cursor_next_capture(Ptr, out match, out index);
    }

    #region PInvoke
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_query_cursor_new();

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_cursor_delete(IntPtr cursor);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_cursor_exec(IntPtr cursor, IntPtr query, TSNode node);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_query_cursor_did_exceed_match_limit(IntPtr cursor);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_query_cursor_match_limit(IntPtr cursor);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_cursor_set_match_limit(IntPtr cursor, uint limit);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_cursor_set_byte_range(IntPtr cursor, uint start_byte, uint end_byte);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_cursor_set_point_range(IntPtr cursor, TSPoint start_point, TSPoint end_point);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_query_cursor_next_match(IntPtr cursor, out TSQueryMatch match);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_query_cursor_remove_match(IntPtr cursor, uint id);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_query_cursor_next_capture(IntPtr cursor, out TSQueryMatch match, out uint capture_index);
    #endregion
}