using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySelector : Node
{
    #region Helper Data Structure(s)

    struct PriorityAdjustment
    {
        public bool adjusted;
        public Status status;
    }

    #endregion

    private bool _nodesAreSorted;
    private bool _adjustPriorityDynamically;

    private Dictionary<Node, PriorityAdjustment> _priorityAdjustments;

    public PrioritySelector(string name, bool adjustPriorityDynamically, float priority = 0)
        : base(name, priority)
    {
        _adjustPriorityDynamically = adjustPriorityDynamically;
        if (_adjustPriorityDynamically)
            _priorityAdjustments = new Dictionary<Node, PriorityAdjustment>();
    }

    public PrioritySelector(string name, bool adjustPriorityDynamically, bool invert, float priority = 0)
        : base(name, invert, priority)
    {
        _adjustPriorityDynamically = adjustPriorityDynamically;
        if (_adjustPriorityDynamically)
            _priorityAdjustments = new Dictionary<Node, PriorityAdjustment>();
    }

    public override void AddChild(Node child)
    {
        if (_adjustPriorityDynamically) 
        {
            foreach (Node childNode in _childrenNodes)
            {
                PriorityAdjustment adjustment = _priorityAdjustments[childNode];
                adjustment.adjusted = false;
                _priorityAdjustments[childNode] = adjustment;
            }

            _priorityAdjustments.Add(child, new PriorityAdjustment());
        }
        
        base.AddChild(child);

        _nodesAreSorted = false;
    }

    private void OrderNodes()
    {
        Sort(_childrenNodes, 0, _childrenNodes.Count - 1);
        _nodesAreSorted = true;
    }

    private bool AdjustPriority(Node childNode, Status checkStatus)
    {
        bool changed = false;

        if (_adjustPriorityDynamically && checkStatus != Status.Running) 
        {
            PriorityAdjustment adjustment = _priorityAdjustments[childNode];
            if (!adjustment.adjusted || adjustment.status != checkStatus)
            {
                adjustment.adjusted = true;
                adjustment.status = checkStatus;

                if (checkStatus == Status.Success)
                    childNode.ChangePriority(childNode.Priority * 10);
                else if (checkStatus == Status.Failure)
                    childNode.ChangePriority(childNode.Priority / 10);

                changed = true;
            }

            _priorityAdjustments[childNode] = adjustment;
        }

        return changed;
    }

    public override Status Process()
    {
        string debug = "";
        foreach (Node childrenNode in _childrenNodes)
        {
            debug += $"{childrenNode.Name} - {childrenNode.Priority}\n";
        }
        Debug.Log(debug);

        if (!_nodesAreSorted)
            OrderNodes();

        Status status = Status.Running;
        Status childStatus = base.Process();
        bool priorityChanged = false;

        if (childStatus == Status.Success)
        {
            priorityChanged = AdjustPriority(_childrenNodes[_currentChildIndex], Status.Success);

            _currentChildIndex = 0;
            status = Status.Success;
        }
        else if (childStatus == Status.Failure)
        {
            priorityChanged = AdjustPriority(_childrenNodes[_currentChildIndex], Status.Failure);

            _currentChildIndex++;
            if (_currentChildIndex >= _childrenNodes.Count)
            {
                _currentChildIndex = 0;
                status = Status.Failure;
            }
        }

        if (priorityChanged) 
            OrderNodes();

        if (_invert)
            return InvertStatus(status);

        return status;
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
