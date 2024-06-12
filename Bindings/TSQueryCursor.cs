using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed class TSQueryCursor : IDisposable {
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

    public void exec(TSQuery query, TSNode node) { ts_query_cursor_exec(Ptr, query.Ptr, node); }
    public bool did_exceed_match_limit() { return ts_query_cursor_did_exceed_match_limit(Ptr); }
    public uint match_limit() { return ts_query_cursor_match_limit(Ptr); }
    public void set_match_limit(uint limit) { ts_query_cursor_set_match_limit(Ptr, limit); }
    public void set_range(uint start, uint end) { ts_query_cursor_set_byte_range(Ptr, start * sizeof(ushort), end * sizeof(ushort)); }
    public void set_point_range(TSPoint start, TSPoint end) { ts_query_cursor_set_point_range(Ptr, start, end); }
    public bool next_match(out TSQueryMatch match, out TSQueryCapture[] captures) {
        captures = null;
        if (ts_query_cursor_next_match(Ptr, out match)) {
            if (match.capture_count > 0) {
                captures = new TSQueryCapture[match.capture_count];
                for (ushort i = 0; i < match.capture_count; i++) {
                    var intPtr = match.captures + Marshal.SizeOf(typeof(TSQueryCapture)) * i;
                    captures[i] = Marshal.PtrToStructure<TSQueryCapture>(intPtr);
                }
            }
            return true;
        }
        return false;
    }
    public void remove_match(uint id) { ts_query_cursor_remove_match(Ptr, id); }
    public bool next_capture(out TSQueryMatch match, out uint index) { return ts_query_cursor_next_capture(Ptr, out match, out index); }

    #region PInvoke
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_query_cursor_new();

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_cursor_delete(IntPtr cursor);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_cursor_exec(IntPtr cursor, IntPtr query, TSNode node);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_query_cursor_did_exceed_match_limit(IntPtr cursor);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_query_cursor_match_limit(IntPtr cursor);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_cursor_set_match_limit(IntPtr cursor, uint limit);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_cursor_set_byte_range(IntPtr cursor, uint start_byte, uint end_byte);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_cursor_set_point_range(IntPtr cursor, TSPoint start_point, TSPoint end_point);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_query_cursor_next_match(IntPtr cursor, out TSQueryMatch match);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_query_cursor_remove_match(IntPtr cursor, uint id);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_query_cursor_next_capture(IntPtr cursor, out TSQueryMatch match, out uint capture_index);
    #endregion
}