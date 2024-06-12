using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed class TSTree : IDisposable {
    internal IntPtr Ptr { get; private set; }

    public TSTree(IntPtr ptr) {
        Ptr = ptr;
    }

    public void Dispose() {
        if (Ptr != IntPtr.Zero) {
            ts_tree_delete(Ptr);
            Ptr = IntPtr.Zero;
        }
    }

    public TSTree copy() {
        var ptr = ts_tree_copy(Ptr);
        return ptr != IntPtr.Zero ? new TSTree(ptr) : null;
    }
    public TSNode root_node() { return ts_tree_root_node(Ptr); }
    public TSNode root_node_with_offset(uint offsetBytes, TSPoint offsetPoint) { return ts_tree_root_node_with_offset(Ptr, offsetBytes, offsetPoint); }
    public TSLanguage language() {
        var ptr = ts_tree_language(Ptr);
        return ptr != IntPtr.Zero ? new TSLanguage(ptr) : null;
    }
    public void edit(TSInputEdit edit) { ts_tree_edit(Ptr, ref edit); }
    #region PInvoke
    /**
    * Create a shallow copy of the syntax tree. This is very fast.
    *
    * You need to copy a syntax tree in order to use it on more than one thread at
    * a time, as syntax trees are not thread safe.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_tree_copy(IntPtr tree);

    /**
    * Delete the syntax tree, freeing all of the memory that it used.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_tree_delete(IntPtr tree);

    /**
    * Get the root node of the syntax tree.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_tree_root_node(IntPtr tree);

    /**
    * Get the root node of the syntax tree, but with its position
    * shifted forward by the given offset.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSNode ts_tree_root_node_with_offset(IntPtr tree, uint offsetBytes, TSPoint offsetPoint);

    /**
    * Get the language that was used to parse the syntax tree.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_tree_language(IntPtr tree);

    /**
    * Get the array of included ranges that was used to parse the syntax tree.
    *
    * The returned pointer must be freed by the caller.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_tree_included_ranges(IntPtr tree, out uint length);

    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_tree_included_ranges_free(IntPtr ranges);

    /**
    * Edit the syntax tree to keep it in sync with source code that has been
    * edited.
    *
    * You must describe the edit both in terms of byte offsets and in terms of
    * (row, column) coordinates.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_tree_edit(IntPtr tree, ref TSInputEdit edit);

    /**
    * Compare an old edited syntax tree to a new syntax tree representing the same
    * document, returning an array of ranges whose syntactic structure has changed.
    *
    * For this to work correctly, the old syntax tree must have been edited such
    * that its ranges match up to the new tree. Generally, you'll want to call
    * this function right after calling one of the `ts_parser_parse` functions.
    * You need to pass the old tree that was passed to parse, as well as the new
    * tree that was returned from that function.
    *
    * The returned array is allocated using `malloc` and the caller is responsible
    * for freeing it using `free`. The length of the array will be written to the
    * given `length` pointer.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_tree_get_changed_ranges(IntPtr old_tree, IntPtr new_tree, out uint length);
    #endregion
}