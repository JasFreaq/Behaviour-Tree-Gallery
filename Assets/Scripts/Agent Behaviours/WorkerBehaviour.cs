using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehaviour : BTAgent
{
    [Header("Worker")]
    [SerializeField] private Transform _office;

    private PatronBehaviour _currentServingPatron;

    void Start()
    {
        Sequence provideTicketsSequence = new Sequence("Provide Tickets");
        provideTicketsSequence.AddChild(new Leaf("Is Gallery Open", Blackboard.Instance.IsGalleryOpen));
        provideTicketsSequence.AddChild(new Leaf("Are Patrons Present", PatronsPresent));
        provideTicketsSequence.AddChild(new Leaf("Give Ticket", GoToPatron));

        DepSequence returnOfficeSequence = new DepSequence("Return To Office",
            new Leaf("Are Patrons Present", PatronsPresent, true), _navAgent);
        returnOfficeSequence.AddChild(new Leaf("Return to Office", MoveToOffice));

        Selector workSelector = new Selector("Do Work");
        workSelector.AddChild(provideTicketsSequence);
        workSelector.AddChild(returnOfficeSequence);

        _tree.AddChild(workSelector);
        base.Start();
    }

    public Node.Status PatronsPresent()
    {
        if (Blackboard.Instance.PatronsPresent)
            return Node.Status.Success;

        return Node.Status.Failure;
    }

    public Node.Status GoToPatron()
    {
        if (_currentServingPatron == null)
        {
            if (!Blackboard.Instance.PatronsPresent)
                return Node.Status.Failure;
            
            _currentServingPatron = Blackboard.Instance.DeregisterPatron();
        }

        Node.Status status = MoveToLocation(_currentServingPatron.transform.position);
        if (status == Node.Status.Success)
        {
            _currentServingPatron.GiveTicket();
            _currentServingPatron = null;
        }

        return status;
    }

    public Node.Status MoveToOffice()
    {
        return MoveToLocation(_office.position);
    }
}
