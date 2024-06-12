using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed partial class TSCursor : IDisposable {
    [StructLayout(LayoutKind.Sequential)]
    public struct TSTreeCursor {
        private IntPtr Tree;
        private IntPtr Id;
        private uint Context0;
        private uint Context1;
    }


    private IntPtr Ptr;
    private TSTreeCursor cursor;
    public TSLanguage lang { get; private set; }

    public TSCursor(TSTreeCursor cursor, TSLanguage lang) {
        this.cursor = cursor;
        this.lang = lang;
        Ptr = new IntPtr(1);
    }

    public TSCursor(TSNode node, TSLanguage lang) {
        this.cursor = ts_tree_cursor_new(node);
        this.lang = lang;
        Ptr = new IntPtr(1);
    }

    public void Dispose() {
        if (Ptr != IntPtr.Zero) {
            ts_tree_cursor_delete(ref cursor);
            Ptr = IntPtr.Zero;
        }
    }

    public void reset(TSNode node) { ts_tree_cursor_reset(ref cursor, node); }
    public TSNode current_node() { return ts_tree_cursor_current_node(ref cursor); }
    public string current_field() { return lang.fields[current_field_id()]; }
    public string current_symbol() {
        ushort symbol = ts_tree_cursor_current_node(ref cursor).symbol();
        return (symbol != UInt16.MaxValue) ? lang.symbols[symbol] : "ERROR";
    }
    public ushort current_field_id() { return ts_tree_cursor_current_field_id(ref cursor); }
    public bool goto_parent() { return ts_tree_cursor_goto_parent(ref cursor); }
    public bool goto_next_sibling() { return ts_tree_cursor_goto_next_sibling(ref cursor); }
    public bool goto_first_child() { return ts_tree_cursor_goto_first_child(ref cursor); }
    public long goto_first_child_for_offset(uint offset) { return ts_tree_cursor_goto_first_child_for_byte(ref cursor, offset * sizeof(ushort)); }
    public long goto_first_child_for_point(TSPoint point) { return ts_tree_cursor_goto_first_child_for_point(ref cursor, point); }
    public TSCursor copy() { return new TSCursor(ts_tree_cursor_copy(ref cursor), lang); }

    #region PInvoke
    /// <summary>
    /// Create a new tree cursor starting from the given node.
    /// A tree cursor allows you to walk a syntax tree more efficiently than is
    /// possible using the `TSNode` functions. It is a mutable object that is always
    /// on a certain syntax node, and can be moved imperatively to different nodes.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSTreeCursor ts_tree_cursor_new(TSNode node);

    /// <summary>
    /// Delete a tree cursor, freeing all of the memory that it used.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_tree_cursor_delete(ref TSTreeCursor cursor);

    /// <summary>
    /// Re-initialize a tree cursor to start at a different node.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_tree_cursor_reset(ref TSTreeCursor cursor, TSNode node);

    /// <summary>
    /// Get the tree cursor's current node.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_tree_cursor_current_node(ref TSTreeCursor cursor);

    /// <summary>
    /// Get the field name of the tree cursor's current node.
    /// This returns `NULL` if the current node doesn't have a field.
    /// See also `ts_node_child_by_field_name`.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_tree_cursor_current_field_name(ref TSTreeCursor cursor);

    /// <summary>
    /// Get the field id of the tree cursor's current node.
    /// This returns zero if the current node doesn't have a field.
    /// See also `ts_node_child_by_field_id`, `ts_language_field_id_for_name`.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial ushort ts_tree_cursor_current_field_id(ref TSTreeCursor cursor);

    /// <summary>
    /// Move the cursor to the parent of its current node.
    /// This returns `true` if the cursor successfully moved, and returns `false`
    /// if there was no parent node (the cursor was already on the root node).
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_tree_cursor_goto_parent(ref TSTreeCursor cursor);

    /// <summary>
    /// Move the cursor to the next sibling of its current node.
    /// This returns `true` if the cursor successfully moved, and returns `false`
    /// if there was no next sibling node.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_tree_cursor_goto_next_sibling(ref TSTreeCursor cursor);

    /// <summary>
    /// Move the cursor to the first child of its current node.
    /// This returns `true` if the cursor successfully moved, and returns `false`
    /// if there were no children.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_tree_cursor_goto_first_child(ref TSTreeCursor cursor);

    /// <summary>
    /// Move the cursor to the first child of its current node that extends beyond
    /// the given byte offset
    /// This returns the index of the child node if one was found, and returns -1
    /// if no such child was found.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial long ts_tree_cursor_goto_first_child_for_byte(ref TSTreeCursor cursor, uint byteOffset);

    /// <summary>
    /// Move the cursor to the first child of its current node that extends beyond
    /// the given point.
    /// This returns the index of the child node if one was found, and returns -1
    /// if no such child was found.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial long ts_tree_cursor_goto_first_child_for_point(ref TSTreeCursor cursor, TSPoint point);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSTreeCursor ts_tree_cursor_copy(ref TSTreeCursor cursor);
    #endregion
}