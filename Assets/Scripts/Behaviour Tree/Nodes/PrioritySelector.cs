using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySelector : Selector
{
    private bool _nodesAreSorted;

    public PrioritySelector(string name, int priority = 0) : base(name, priority) { }

    public PrioritySelector(string name, bool invert, int priority = 0) : base(name, invert, priority)
    { }

    public override void AddChild(Node child)
    {
        base.AddChild(child);
        _nodesAreSorted = false;
    }

    private void OrderNodes()
    {
        if (!_nodesAreSorted)
        {
            Sort(_childrenNodes, 0, _childrenNodes.Count - 1);
            _nodesAreSorted = true;
        }
    }

    public override Status Process()
    {
        OrderNodes();
        return base.Process();
    }

    private static int Partition(List<Node> nodes, int low, int high)
    {
        Node pivot = nodes[high];
        int lowIndex = (low - 1);

        for (int j = low; j < high; j++)
        {
            if (nodes[j].Priority >= pivot.Priority)
            {
                lowIndex++;

                Node temp = nodes[lowIndex];
                nodes[lowIndex] = nodes[j];
                nodes[j] = temp;
            }
        }

        Node temp1 = nodes[lowIndex + 1];
        nodes[lowIndex + 1] = nodes[high];
        nodes[high] = temp1;

        return lowIndex + 1;
    }

    private static void Sort(List<Node> nodes, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(nodes, low, high);
            Sort(nodes, low, partitionIndex - 1);
            Sort(nodes, partitionIndex + 1, high);
        }
    }
}
