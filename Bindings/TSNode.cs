using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
[StructLayout(LayoutKind.Sequential)]
public struct TSNode {
    private uint context0;
    private uint context1;
    private uint context2;
    private uint context3;
    public IntPtr id;
    private IntPtr tree;

    public void clear() { id = IntPtr.Zero; tree = IntPtr.Zero; }
    public bool is_zero() { return (id == IntPtr.Zero && tree == IntPtr.Zero); }
    public string type() { return Marshal.PtrToStringAnsi(ts_node_type(this)); }
    public string type(TSLanguage lang) { return lang.symbol_name(symbol()); }
    public ushort symbol() { return ts_node_symbol(this); }
    public uint start_offset() { return ts_node_start_byte(this) / sizeof(ushort); }
    public TSPoint start_point() { var pt = ts_node_start_point(this); return new TSPoint(pt.row, pt.column / sizeof(ushort)); }
    public uint end_offset() { return ts_node_end_byte(this) / sizeof(ushort); }
    public TSPoint end_point() { var pt = ts_node_end_point(this); return new TSPoint(pt.row, pt.column / sizeof(ushort)); }
    public string to_string() { var dat = ts_node_string(this); var str = Marshal.PtrToStringAnsi(dat); ts_node_string_free(dat); return str; }
    public bool is_null() { return ts_node_is_null(this); }
    public bool is_named() { return ts_node_is_named(this); }
    public bool is_missing() { return ts_node_is_missing(this); }
    public bool is_extra() { return ts_node_is_extra(this); }
    public bool has_changes() { return ts_node_has_changes(this); }
    public bool has_error() { return ts_node_has_error(this); }
    public TSNode parent() { return ts_node_parent(this); }
    public TSNode child(uint index) { return ts_node_child(this, index); }
    public IntPtr field_name_for_child(uint index) { return ts_node_field_name_for_child(this, index); }
    public uint child_count() { return ts_node_child_count(this); }
    public TSNode named_child(uint index) { return ts_node_named_child(this, index); }
    public uint named_child_count() { return ts_node_named_child_count(this); }
    public TSNode child_by_field_name(string field_name) { return ts_node_child_by_field_name(this, field_name, (uint)field_name.Length); }
    public TSNode child_by_field_id(ushort fieldId) { return ts_node_child_by_field_id(this, fieldId); }
    public TSNode next_sibling() { return ts_node_next_sibling(this); }
    public TSNode prev_sibling() { return ts_node_prev_sibling(this); }
    public TSNode next_named_sibling() { return ts_node_next_named_sibling(this); }
    public TSNode prev_named_sibling() { return ts_node_prev_named_sibling(this); }
    public TSNode first_child_for_offset(uint offset) { return ts_node_first_child_for_byte(this, offset * sizeof(ushort)); }
    public TSNode first_named_child_for_offset(uint offset) { return ts_node_first_named_child_for_byte(this, offset * sizeof(ushort)); }
    public TSNode descendant_for_offset_range(uint start, uint end) { return ts_node_descendant_for_byte_range(this, start * sizeof(ushort), end * sizeof(ushort)); }
    public TSNode descendant_for_point_range(TSPoint start, TSPoint end) { return ts_node_descendant_for_point_range(this, start, end); }
    public TSNode named_descendant_for_offset_range(uint start, uint end) { return ts_node_named_descendant_for_byte_range(this, start * sizeof(ushort), end * sizeof(ushort)); }
    public TSNode named_descendant_for_point_range(TSPoint start, TSPoint end) { return ts_node_named_descendant_for_point_range(this, start, end); }
    public bool eq(TSNode other) { return ts_node_eq(this, other); }

    public string text(string data) {
        uint beg = start_offset();
        uint end = end_offset();
        return data.Substring((int)beg, (int)(end - beg));
    }

    #region PInvoke
    /**
    * Get the node's type as a null-terminated string.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_node_type(TSNode node);

    /**
    * Get the node's type as a numerical id.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort ts_node_symbol(TSNode node);

    /**
    * Get the node's start byte.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_node_start_byte(TSNode node);

    /**
    * Get the node's start position in terms of rows and columns.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSPoint ts_node_start_point(TSNode node);

    /**
    * Get the node's end byte.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_node_end_byte(TSNode node);

    /**
    * Get the node's end position in terms of rows and columns.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSPoint ts_node_end_point(TSNode node);

    /**
    * Get an S-expression representing the node as a string.
    *
    * This string is allocated with `malloc` and the caller is responsible for
    * freeing it using `free`.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_node_string(TSNode node);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_node_string_free(IntPtr str);

    /**
    * Check if the node is null. Functions like `ts_node_child` and
    * `ts_node_next_sibling` will return a null node to indicate that no such node
    * was found.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_is_null(TSNode node);

    /**
    * Check if the node is *named*. Named nodes correspond to named rules in the
    * grammar, whereas *anonymous* nodes correspond to string literals in the
    * grammar.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_is_named(TSNode node);

    /**
    * Check if the node is *missing*. Missing nodes are inserted by the parser in
    * order to recover from certain kinds of syntax errors.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_is_missing(TSNode node);

    /**
    * Check if the node is *extra*. Extra nodes represent things like comments,
    * which are not required the grammar, but can appear anywhere.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_is_extra(TSNode node);

    /**
    * Check if a syntax node has been edited.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_has_changes(TSNode node);

    /**
    * Check if the node is a syntax error or contains any syntax errors.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_has_error(TSNode node);

    /**
    * Get the node's immediate parent.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_parent(TSNode node);

    /**
    * Get the node's child at the given index, where zero represents the first
    * child.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_child(TSNode node, uint index);

    /**
    * Get the field name for node's child at the given index, where zero represents
    * the first child. Returns NULL, if no field is found.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_node_field_name_for_child(TSNode node, uint index);

    /**
    * Get the node's number of children.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_node_child_count(TSNode node);

    /**
    * Get the node's number of *named* children.
    *
    * See also `ts_node_is_named`.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_named_child(TSNode node, uint index);

    /**
    * Get the node's number of *named* children.
    *
    * See also `ts_node_is_named`.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_node_named_child_count(TSNode node);

    /**
    * Get the node's child with the given numerical field id.
    *
    * You can convert a field name to an id using the
    * `ts_language_field_id_for_name` function.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_child_by_field_name(TSNode self, [MarshalAs(UnmanagedType.LPUTF8Str)] string field_name, uint field_name_length);

    /**
    * Get the node's child with the given numerical field id.
    *
    * You can convert a field name to an id using the
    * `ts_language_field_id_for_name` function.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_child_by_field_id(TSNode self, ushort fieldId);

    /**
    * Get the node's next / previous sibling.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_next_sibling(TSNode self);
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_prev_sibling(TSNode self);

    /**
    * Get the node's next / previous *named* sibling.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_next_named_sibling(TSNode self);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_prev_named_sibling(TSNode self);

    /**
    * Get the node's first child that extends beyond the given byte offset.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_first_child_for_byte(TSNode self, uint byteOffset);

    /**
    * Get the node's first named child that extends beyond the given byte offset.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_first_named_child_for_byte(TSNode self, uint byteOffset);

    /**
    * Get the smallest node within this node that spans the given range of bytes
    * or (row, column) positions.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_descendant_for_byte_range(TSNode self, uint startByte, uint endByte);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_descendant_for_point_range(TSNode self, TSPoint startPoint, TSPoint endPoint);

    /**
    * Get the smallest named node within this node that spans the given range of
    * bytes or (row, column) positions.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_named_descendant_for_byte_range(TSNode self, uint startByte, uint endByte);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_node_named_descendant_for_point_range(TSNode self, TSPoint startPoint, TSPoint endPoint);

    /**
    * Check if two nodes are identical.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern bool ts_node_eq(TSNode node1, TSNode node2);
    #endregion
}