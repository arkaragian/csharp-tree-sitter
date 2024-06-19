using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;

/// <summary>
/// A TSNode represents a single node in the syntax tree. It tracks its start
/// and end positions in the source code, as well as its relation to other nodes
/// like its parent, siblings and children.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct TSNode {
    private uint context0;
    private uint context1;
    private uint context2;
    private uint context3;
    public IntPtr id;
    private IntPtr tree;

    public void Clear() {
        id = IntPtr.Zero;
        tree = IntPtr.Zero;
    }

    public readonly bool IsZero => id == IntPtr.Zero && tree == IntPtr.Zero;

    public readonly string? Type => Marshal.PtrToStringAnsi(ts_node_type(this));

    // Maybe this should be a language property
    // public string Type(TSLanguage lang) {
    //     return lang.SymbolName(Symbol());
    // }

    public readonly ushort Symbol => ts_node_symbol(this);

    public readonly uint StartOffset => ts_node_start_byte(this) / sizeof(ushort);
    public readonly uint EndOffset => ts_node_end_byte(this) / sizeof(ushort);

    public readonly TSPoint StartPoint {
        get {
            TSPoint pt = ts_node_start_point(this);
            return new TSPoint(pt.Row, pt.Column / sizeof(ushort));
        }
    }


    public readonly TSPoint EndPoint {
        get {
            TSPoint pt = ts_node_end_point(this);
            return new TSPoint(pt.Row, pt.Column / sizeof(ushort));
        }
    }
    ///
    /// <summary>
    /// Returns the syntax tree of the node. That is the lisp like strucure
    /// </summary>
    public readonly string? InnerSyntaxTree {
        get {
            nint dat = ts_node_string(this);
            string? str = Marshal.PtrToStringAnsi(dat);
            ts_node_string_free(dat);
            return str;
        }
    }


    //public readonly bool IsNull => ts_node_is_null(this);

    /// <summary>
    /// Tree-sitter produces concrete syntax trees - trees that contain nodes
    /// for every individual token in the source code, including things like
    /// commas and parentheses. This is important for use-cases that deal with
    /// individual tokens, like syntax highlighting. But some types of code
    /// analysis are easier to perform using an abstract syntax tree - a tree
    /// in which the less important details have been removed. Tree-sitter’s
    /// trees support these use cases by making a distinction between named
    /// and anonymous nodes.
    /// </summary>
    public readonly bool IsNamed => ts_node_is_named(this);

    public readonly bool IsMissing => ts_node_is_missing(this);

    public readonly bool IsExtra => ts_node_is_extra(this);

    public readonly bool HasChanges => ts_node_has_changes(this);

    public readonly bool HasError => ts_node_has_error(this);

    public readonly TSNode Parent => ts_node_parent(this);

    /// <summary>
    /// Returns the child node that resides in the index specified by <paramref name="index"/>
    /// </summary>
    public readonly TSNode Child(uint index) {
        //TODO: Check Bounds
        return ts_node_child(this, index);
    }

    public readonly IntPtr FieldNameForChild(uint index) {
        return ts_node_field_name_for_child(this, index);
    }
    public readonly uint ChildCount => ts_node_child_count(this);

    public readonly TSNode NamedChild(uint index) {
        //TODO: Check bounds
        return ts_node_named_child(this, index);
    }

    public readonly uint NamedChildCount => ts_node_named_child_count(this);

    public readonly TSNode ChildByFieldName(string fieldName) {
        return ts_node_child_by_field_name(this, fieldName, (uint)fieldName.Length);
    }

    public readonly TSNode ChildByFieldId(ushort fieldId) {
        return ts_node_child_by_field_id(this, fieldId);
    }

    public readonly TSNode? NextSibling {
        get {
            TSNode result = ts_node_next_sibling(this);
            if (ts_node_is_null(result)) {
                return null;
            }
            return result;
        }
    }//ts_node_next_sibling(this);

    public readonly TSNode PrevSibling => ts_node_prev_sibling(this);

    public readonly TSNode NextNamedSibling => ts_node_next_named_sibling(this);

    public readonly TSNode PrevNamedSibling => ts_node_prev_named_sibling(this);

    public readonly TSNode FirstChildForOffset(uint offset) {
        return ts_node_first_child_for_byte(this, offset * sizeof(ushort));
    }

    public readonly TSNode FirstNamedChildForOffset(uint offset) {
        return ts_node_first_named_child_for_byte(this, offset * sizeof(ushort));
    }

    public readonly TSNode DescendantForOffsetRange(uint start, uint end) {
        return ts_node_descendant_for_byte_range(this, start * sizeof(ushort), end * sizeof(ushort));
    }

    public readonly TSNode DescendantForPointRange(TSPoint start, TSPoint end) {
        return ts_node_descendant_for_point_range(this, start, end);
    }

    public readonly TSNode NamedDescendantForOffsetRange(uint start, uint end) {
        return ts_node_named_descendant_for_byte_range(this, start * sizeof(ushort), end * sizeof(ushort));
    }

    public readonly TSNode NamedDescendantForPointRange(TSPoint start, TSPoint end) {
        return ts_node_named_descendant_for_point_range(this, start, end);
    }

    public readonly bool IsEqualTo(TSNode other) {
        return ts_node_eq(this, other);
    }

    /// <summary>
    /// Returns the source code that this node encompases. The actual source
    /// data is drawn from the <paramref name="rawFileContents"/> input.
    /// </summary>
    /// <remarks>
    ///  The <paramref name="rawFileContents"/> must contain the complete source
    ///  of the file.
    /// </remarks>
    public readonly string SourceCode(string rawFileContents) {
        uint beg = StartOffset;
        uint end = EndOffset;
        //TODO: Maybe throw an exception when something bad happens
        return rawFileContents.Substring((int)beg, (int)(end - beg));
    }

    #region PInvoke
    /**
    * Get the node's type as a null-terminated string.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_node_type(TSNode node);

    /**
    * Get the node's type as a numerical id.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial ushort ts_node_symbol(TSNode node);

    /**
    * Get the node's start byte.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_node_start_byte(TSNode node);

    /**
    * Get the node's start position in terms of rows and columns.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSPoint ts_node_start_point(TSNode node);

    /**
    * Get the node's end byte.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_node_end_byte(TSNode node);

    /**
    * Get the node's end position in terms of rows and columns.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSPoint ts_node_end_point(TSNode node);

    /**
    * Get an S-expression representing the node as a string.
    *
    * This string is allocated with `malloc` and the caller is responsible for
    * freeing it using `free`.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_node_string(TSNode node);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_node_string_free(IntPtr str);

    /**
    * Check if the node is null. Functions like `ts_node_child` and
    * `ts_node_next_sibling` will return a null node to indicate that no such node
    * was found.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_is_null(TSNode node);

    /**
    * Check if the node is *named*. Named nodes correspond to named rules in the
    * grammar, whereas *anonymous* nodes correspond to string literals in the
    * grammar.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_is_named(TSNode node);

    /**
    * Check if the node is *missing*. Missing nodes are inserted by the parser in
    * order to recover from certain kinds of syntax errors.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_is_missing(TSNode node);

    /**
    * Check if the node is *extra*. Extra nodes represent things like comments,
    * which are not required the grammar, but can appear anywhere.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_is_extra(TSNode node);

    /**
    * Check if a syntax node has been edited.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_has_changes(TSNode node);

    /**
    * Check if the node is a syntax error or contains any syntax errors.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_has_error(TSNode node);

    /**
    * Get the node's immediate parent.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_parent(TSNode node);

    /**
    * Get the node's child at the given index, where zero represents the first
    * child.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_child(TSNode node, uint index);

    /**
    * Get the field name for node's child at the given index, where zero represents
    * the first child. Returns NULL, if no field is found.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_node_field_name_for_child(TSNode node, uint index);

    /**
    * Get the node's number of children.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_node_child_count(TSNode node);

    /**
    * Get the node's number of *named* children.
    *
    * See also `ts_node_is_named`.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_named_child(TSNode node, uint index);

    /**
    * Get the node's number of *named* children.
    *
    * See also `ts_node_is_named`.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial uint ts_node_named_child_count(TSNode node);

    /**
    * Get the node's child with the given numerical field id.
    *
    * You can convert a field name to an id using the
    * `ts_language_field_id_for_name` function.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_child_by_field_name(TSNode self, [MarshalAs(UnmanagedType.LPUTF8Str)] string field_name, uint field_name_length);

    /**
    * Get the node's child with the given numerical field id.
    *
    * You can convert a field name to an id using the
    * `ts_language_field_id_for_name` function.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_child_by_field_id(TSNode self, ushort fieldId);

    /**
    * Get the node's next / previous sibling.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_next_sibling(TSNode self);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_prev_sibling(TSNode self);

    /**
    * Get the node's next / previous *named* sibling.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_next_named_sibling(TSNode self);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_prev_named_sibling(TSNode self);

    /**
    * Get the node's first child that extends beyond the given byte offset.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_first_child_for_byte(TSNode self, uint byteOffset);

    /**
    * Get the node's first named child that extends beyond the given byte offset.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_first_named_child_for_byte(TSNode self, uint byteOffset);

    /**
    * Get the smallest node within this node that spans the given range of bytes
    * or (row, column) positions.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_descendant_for_byte_range(TSNode self, uint startByte, uint endByte);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_descendant_for_point_range(TSNode self, TSPoint startPoint, TSPoint endPoint);

    /**
    * Get the smallest named node within this node that spans the given range of
    * bytes or (row, column) positions.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_named_descendant_for_byte_range(TSNode self, uint startByte, uint endByte);

    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial TSNode ts_node_named_descendant_for_point_range(TSNode self, TSPoint startPoint, TSPoint endPoint);

    /**
    * Check if two nodes are identical.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ts_node_eq(TSNode node1, TSNode node2);
    #endregion
}