using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Node
{
    public Tree()
    {
        _name = "Tree Root";
    }

    public Tree(string name)
    {
        _name = name;
    }
    
    #region Editor Specific Code

#if UNITY_EDITOR

    struct NodeLevel
    {
        public Node node;
        public int level;

        public NodeLevel(Node node, int level)
        {
            this.node = node;
            this.level = level;
        }
    }

    public void PrintTree()
    {
        string treePrintout = "";
        Stack<NodeLevel> nodeStack = new Stack<NodeLevel>();
        nodeStack.Push(new NodeLevel(this, 0));

        while (nodeStack.Count > 0)
        {
            NodeLevel currentNode = nodeStack.Pop();
            treePrintout += $"{new string('\t', currentNode.level)} {currentNode.node.Name}\n";

            for (int i = currentNode.node.ChildrenNodes.Count - 1; i >= 0; i--)
                nodeStack.Push(new NodeLevel(currentNode.node.ChildrenNodes[i], currentNode.level + 1));
        }

        Debug.Log(treePrintout);
    }

#endif

    #endregion
}
