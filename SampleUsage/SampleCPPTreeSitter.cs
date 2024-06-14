using TreeSitter.CSharp;

namespace SampleUsage;
public class SampleCPPTreeSitter {

    public static void PostOrderTraverse(string path, string filetext, TSCursor cursor) {
        TSCursor rootCursor = cursor;

        for (; ; ) {
            int so = (int)cursor.CurrentNode.StartOffset;
            int eo = (int)cursor.CurrentNode.EndOffset;
            int sl = (int)cursor.CurrentNode.StartPoint.Row + 1;
            string field = cursor.CurrentField;
            string type = cursor.CurrentSymbolString;
            bool hasChildren = cursor.GotoFirstChild();

            ReadOnlySpan<char> span = filetext.AsSpan(so, eo - so);

            if (hasChildren) {
                continue;
            }

            Console.Error.WriteLine("The node type is {0}, symbol is {1}", type, span.ToString());

            if (cursor.GotoNextSibling()) {
                continue;
            }

            do {
                _ = cursor.GotoParent();
                int so_p = (int)cursor.CurrentNode.StartOffset;
                int eo_p = (int)cursor.CurrentNode.EndOffset;
                string type_p = cursor.CurrentSymbolString;
                ReadOnlySpan<char> span_p = filetext.AsSpan(so_p, eo_p - so_p);

                Console.Error.WriteLine("The node type is {0}, symbol is {1}", type_p, span_p.ToString());

                if (rootCursor == cursor) {
                    Console.Error.WriteLine("done!");
                    return;
                }
            } while (!cursor.GotoNextSibling());
        }
    }
}