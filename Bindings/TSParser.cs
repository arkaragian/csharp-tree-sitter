using System.Runtime.InteropServices;

namespace TreeSitter.CSharp;

/// <summary>
/// A stateful object that can be assigned a TSLanguage and used to produce a
/// TSTree based on some source code.
/// </summary>
public partial class TSParser : IDisposable {
    private IntPtr ParserPointer { get; set; }

    public TSParser() {
        ParserPointer = ts_parser_new();
    }

    public void Dispose() {
        if (ParserPointer != IntPtr.Zero) {
            ts_parser_delete(ParserPointer);
            ParserPointer = IntPtr.Zero;
        }
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The language defition of the parser
    /// </summary>
    public TSLanguage? Language {
        get {
            nint ptr = ts_parser_language(ParserPointer);
            return ptr != IntPtr.Zero ? new TSLanguage(ptr) : null;
        }
        set {
            if (value is null) {
                return;
            }
            bool ok = ts_parser_set_language(ParserPointer, value.LanguagePointer);
            if (!ok) {
                throw new InvalidOperationException("incompatible Language!");
            }
        }
    }

    /// <summary>
    /// By default, the parser will always include entire documents. This property
    /// allows you to parse only a *portion* of a document but still return a syntax
    /// tree whose ranges match up with the document as a whole. You can also pass
    /// multiple disjoint ranges.
    ///
    /// If `count` is zero, then the entire document will be parsed. Otherwise,
    /// the given ranges must be ordered from earliest to latest in the document,
    /// and they must not overlap. That is, the following must hold for all:
    ///
    /// `i < count - 1`: `ranges[i].end_byte <= ranges[i + 1].start_byte`
    ///
    /// If this requirement is not satisfied, the operation will fail, the ranges
    /// will not be assigned, and this function will return `false`. On success,
    /// this function returns `true`
    /// </summary>
    /// <remarks>
    /// The parser does *not* take ownership of these ranges; it copies
    /// the data, so it doesn't matter how these ranges are allocated.
    /// </remarks>
    public TSRange[] IncludedRanges {
        get => ts_parser_included_ranges(ParserPointer, out _);
        set {
            bool ok = ts_parser_set_included_ranges(ParserPointer, value, (uint)value.Length);
            if (!ok) {
                throw new InvalidOperationException("Could not set ranges!");
            }
        }
    }

    /// <summary>
    /// Generates a tree from the string input defined by <paramref name="input"/>
    /// </summary>
    public TSTree? ParseString(TSTree? oldTree, string input) {
        IntPtr old_ptr = oldTree?.TreePointer ?? IntPtr.Zero;
        nint ptr = ts_parser_parse_string_encoding(ParserPointer, old_ptr, input, (uint)input.Length * 2, TSInputEncoding.TSInputEncodingUTF16);
        if (ptr != IntPtr.Zero) {
            return new TSTree(ptr);
        } else {
            return null;
        }
    }

    public void Reset() {
        ts_parser_reset(ParserPointer);
    }

    public void SetTimeoutMicros(ulong timeout) {
        ts_parser_set_timeout_micros(ParserPointer, timeout);
    }

    public ulong TimeoutMicros() {
        return ts_parser_timeout_micros(ParserPointer);
    }

    public void SetLogger(TSLogger? logger) {
        if (logger is null) {
            return;
        }
        TSLoggerCode code = new(logger);
        //TODO: Refactor this.
        TSLoggerData data = new() {
            Log = new TSLogCallback(code.LogCallback)
        };
        ts_parser_set_logger(ParserPointer, data);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TSLoggerData {
        private readonly IntPtr Payload;
        internal TSLogCallback Log;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void TSLogCallback(IntPtr payload, TSLogType logType, [MarshalAs(UnmanagedType.LPUTF8Str)] string message);

    private class TSLoggerCode {
        private readonly TSLogger _logger;

        internal TSLoggerCode(TSLogger logger) {
            _logger = logger;
        }

        internal void LogCallback(IntPtr payload, TSLogType logType, string message) {
            _logger(logType, message);
        }
    }

    #region PInvoke
    [LibraryImport("tree-sitter-cpp.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr tree_sitter_cpp();

    [LibraryImport("tree-sitter-c-sharp.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr tree_sitter_c_sharp();

    [LibraryImport("tree-sitter-rust.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr tree_sitter_rust();


    /**
    * Create a new parser.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_parser_new();

    /**
    * Delete the parser, freeing all of the memory that it used.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_parser_delete(IntPtr parser);

    /**
    */
    /// <summary>
    /// Set the language that the parser should use for parsing.
    ///
    /// Returns a boolean indicating whether or not the language was successfully
    /// assigned. True means assignment succeeded. False means there was a version
    /// mismatch: the language was generated with an incompatible version of the
    /// Tree-sitter CLI. Check the language's version using `ts_language_version`
    /// and compare it to this library's `TREE_SITTER_LANGUAGE_VERSION` and
    /// `TREE_SITTER_MIN_COMPATIBLE_LANGUAGE_VERSION` constants.
    /// </summary>
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.I1)]
    private static partial bool ts_parser_set_language(IntPtr parser, IntPtr language);

    /**
    * Get the parser's current language.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_parser_language(IntPtr parser);


    /// <summary>
    /// Set the ranges of text that the parser should include when parsing.
    ///
    /// By default, the parser will always include entire documents. This function
    /// allows you to parse only a *portion* of a document but still return a syntax
    /// tree whose ranges match up with the document as a whole. You can also pass
    /// multiple disjoint ranges.
    ///
    /// The second and third parameters specify the location and length of an array
    /// of ranges. The parser does *not* take ownership of these ranges; it copies
    /// the data, so it doesn't matter how these ranges are allocated.
    ///
    /// If `length` is zero, then the entire document will be parsed. Otherwise,
    /// the given ranges must be ordered from earliest to latest in the document,
    /// and they must not overlap. That is, the following must hold for all
    /// `i` < `length - 1`: ranges[i].end_byte <= ranges[i + 1].start_byte
    ///
    /// If this requirement is not satisfied, the operation will fail, the ranges
    /// will not be assigned, and this function will return `false`. On success,
    /// this function returns `true`
    /// </summary>
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    //[return: MarshalAs(UnmanagedType.I1)]
    private static extern bool ts_parser_set_included_ranges(IntPtr parser, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] TSRange[] ranges, uint length);

    /**
    * Get the ranges of text that the parser will include when parsing.
    *
    * The returned pointer is owned by the parser. The caller should not free it
    * or write to it. The length of the array will be written to the given
    * `length` pointer.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    [return: MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
    private static partial TSRange[] ts_parser_included_ranges(IntPtr parser, out uint length);

    /**
    * Use the parser to parse some source code stored in one contiguous buffer.
    * The first two parameters are the same as in the `ts_parser_parse` function
    * above. The second two parameters indicate the location of the buffer and its
    * length in bytes.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_parser_parse_string(IntPtr parser, IntPtr oldTree, [MarshalAs(UnmanagedType.LPUTF8Str)] string input, uint length);

    /**
    * Use the parser to parse some source code stored in one contiguous buffer.
    * The first two parameters are the same as in the `ts_parser_parse` function
    * above. The second two parameters indicate the location of the buffer and its
    * length in bytes.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    //private static extern IntPtr ts_parser_parse_string_encoding(IntPtr parser, IntPtr oldTree, [MarshalAs(UnmanagedType.LPUTF8Str)] string input, uint length, TSInputEncoding encoding);
    private static partial IntPtr ts_parser_parse_string_encoding(IntPtr parser, IntPtr oldTree, [MarshalAs(UnmanagedType.LPWStr)] string input, uint length, TSInputEncoding encoding);

    /**
    * Instruct the parser to start the next parse from the beginning.
    *
    * If the parser previously failed because of a timeout or a cancellation, then
    * by default, it will resume where it left off on the next call to
    * `ts_parser_parse` or other parsing functions. If you don't want to resume,
    * and instead intend to use this parser to parse some other document, you must
    * call `ts_parser_reset` first.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_parser_reset(IntPtr parser);

    /**
    * Set the maximum duration in microseconds that parsing should be allowed to
    * take before halting.
    *
    * If parsing takes longer than this, it will halt early, returning NULL.
    * See `ts_parser_parse` for more information.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_parser_set_timeout_micros(IntPtr parser, ulong timeout);

    /**
    * Get the duration in microseconds that parsing is allowed to take.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial ulong ts_parser_timeout_micros(IntPtr parser);

    /**
    * Set the parser's current cancellation flag pointer.
    *
    * If a non-null pointer is assigned, then the parser will periodically read
    * from this pointer during parsing. If it reads a non-zero value, it will
    * halt early, returning NULL. See `ts_parser_parse` for more information.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial void ts_parser_set_cancellation_flag(IntPtr parser, ref IntPtr flag);

    /**
    * Get the parser's current cancellation flag pointer.
    */
    [LibraryImport("tree-sitter.dll")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    private static partial IntPtr ts_parser_cancellation_flag(IntPtr parser);

    /**
    * Set the logger that a parser should use during parsing.
    *
    * The parser does not take ownership over the logger payload. If a logger was
    * previously assigned, the caller is responsible for releasing any memory
    * owned by the previous logger.
    */
    [DllImport("tree-sitter.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ts_parser_set_logger(IntPtr parser, TSLoggerData logger);
    #endregion

}