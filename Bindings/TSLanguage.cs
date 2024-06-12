using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;
public sealed class TSLanguage : IDisposable {
    internal IntPtr Ptr { get; private set; }

    public TSLanguage(IntPtr ptr) {
        Ptr = ptr;

        symbols = new String[symbol_count() + 1];
        for (ushort i = 0; i < symbols.Length; i++) {
            symbols[i] = Marshal.PtrToStringAnsi(ts_language_symbol_name(Ptr, i));
        }

        fields = new String[field_count() + 1];
        fieldIds = new Dictionary<string, ushort>((int)field_count() + 1);

        for (ushort i = 0; i < fields.Length; i++) {
            fields[i] = Marshal.PtrToStringAnsi(ts_language_field_name_for_id(Ptr, i));
            if (fields[i] != null) {
                fieldIds.Add(fields[i], i); // TODO: check for dupes, and throw if found
            }
        }

#if false
            for (int i = 0; i < symbols.Length; i++) {
                for (int j = 0; j < i; j++) {
                    Debug.Assert(symbols[i] != symbols[j]);
                }
            }

            for (int i = 0; i < fields.Length; i++) {
                for (int j = 0; j < i; j++) {
                    Debug.Assert(fields[i] != fields[j]);
                }
            }
#endif
    }

    public void Dispose() {
        if (Ptr != IntPtr.Zero) {
            //ts_query_cursor_delete(Ptr);
            Ptr = IntPtr.Zero;
        }
    }

    public TSQuery query_new(string source, out uint error_offset, out TSQueryError error_type) {
        var ptr = ts_query_new(Ptr, source, (uint)source.Length, out error_offset, out error_type);
        return ptr != IntPtr.Zero ? new TSQuery(ptr) : null;
    }

    public String[] symbols;
    public String[] fields;
    public Dictionary<String, ushort> fieldIds;

    public uint symbol_count() { return ts_language_symbol_count(Ptr); }
    public string symbol_name(ushort symbol) { return (symbol != UInt16.MaxValue) ? symbols[symbol] : "ERROR"; }
    public ushort symbol_for_name(string str, bool is_named) { return ts_language_symbol_for_name(Ptr, str, (uint)str.Length, is_named); }
    public uint field_count() { return ts_language_field_count(Ptr); }
    public string field_name_for_id(ushort fieldId) { return fields[fieldId]; }
    public ushort field_id_for_name(string str) { return ts_language_field_id_for_name(Ptr, str, (uint)str.Length); }
    public TSSymbolType symbol_type(ushort symbol) { return ts_language_symbol_type(Ptr, symbol); }

    #region PInvoke
    /**
    * Create a new query from a string containing one or more S-expression
    * patterns. The query is associated with a particular language, and can
    * only be run on syntax nodes parsed with that language.
    *
    * If all of the given patterns are valid, this returns a `TSQuery`.
    * If a pattern is invalid, this returns `NULL`, and provides two pieces
    * of information about the problem:
    * 1. The byte offset of the error is written to the `error_offset` parameter.
    * 2. The type of error is written to the `error_type` parameter.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_query_new(IntPtr language, [MarshalAs(UnmanagedType.LPUTF8Str)] string source, uint source_len, out uint error_offset, out TSQueryError error_type);

    /**
    * Get the number of distinct node types in the language.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_language_symbol_count(IntPtr language);

    /**
    * Get a node type string for the given numerical id.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_language_symbol_name(IntPtr language, ushort symbol);

    /**
    * Get the numerical id for the given node type string.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort ts_language_symbol_for_name(IntPtr language, [MarshalAs(UnmanagedType.LPUTF8Str)] string str, uint length, bool is_named);

    /**
    * Get the number of distinct field names in the language.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_language_field_count(IntPtr language);

    /**
    * Get the field name string for the given numerical id.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ts_language_field_name_for_id(IntPtr language, ushort fieldId);

    /**
    * Get the numerical id for the given field name string.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern ushort ts_language_field_id_for_name(IntPtr language, [MarshalAs(UnmanagedType.LPUTF8Str)] string str, uint length);

    /**
    * Check whether the given node type id belongs to named nodes, anonymous nodes,
    * or a hidden nodes.
    *
    * See also `ts_node_is_named`. Hidden nodes are never returned from the API.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern TSSymbolType ts_language_symbol_type(IntPtr language, ushort symbol);

    /**
    * Get the ABI version number for this language. This version number is used
    * to ensure that languages were generated by a compatible version of
    * Tree-sitter.
    *
    * See also `ts_parser_set_language`.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint ts_language_version(IntPtr language);
    #endregion
}