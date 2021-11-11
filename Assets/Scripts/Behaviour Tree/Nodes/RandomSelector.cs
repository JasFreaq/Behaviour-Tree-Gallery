using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelector : Selector
{
    private static readonly System.Random R = new System.Random();

    public RandomSelector(string name, int priority = 0) : base(name, priority) { }

    public RandomSelector(string name, bool invert, int priority = 0) : base(name, invert, priority)
    { }

    private void RandomizeNodes()
    {
        int n = _childrenNodes.Count;
        while (n > 1)
        {
            n--;
            int k = R.Next(n + 1);
            Node value = _childrenNodes[k];
            _childrenNodes[k] = _childrenNodes[n];
            _childrenNodes[n] = value;
        }
    }

    public override void AddChild(Node child)
    {
        base.AddChild(child);
        RandomizeNodes();
    }
}
