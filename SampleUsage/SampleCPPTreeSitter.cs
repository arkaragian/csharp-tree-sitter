using TreeSitter.CSharp;

namespace SampleUsage;
public class SampleCPPTreeSitter {
    public static void PostOrderTraverseTree(TSTree? tree, TSLanguage lang, string sourceCode) {
        if (tree is null) {
            return;
        }
        PostOrderTraverseNode(tree.RootNode, lang, sourceCode);
    }

    public static void PostOrderTraverseNode(TSNode node, TSLanguage lang, string source) {
        TraverseNode(node, source);
        for (uint i = 0; i < node.ChildCount; i++) {
            TSNode child = node.Child(i);
            PostOrderTraverseNode(child, lang, source);
        }
    }

    public static void TraverseNode(TSNode node, string sourceCode) {
        Console.WriteLine("The node type is {0}, symbol is {1}", node.Type, node.SourceCode(sourceCode));
    }
}