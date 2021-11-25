using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class Blackboard : MonoBehaviour
{
    [SerializeField] private Text _timeText;

    private int _timeOfDay = 8;
    private Queue<PatronBehaviour> _patrons = new Queue<PatronBehaviour>();

    public bool PatronsPresent
    {
        get { return _patrons.Count > 0; }
    }

    #region Static Members

    private static Blackboard _instance;

    public static Blackboard Instance
    {
        get { return _instance; }
    }

    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(UpdateClockRoutine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Time.timeScale = 5;
    }

    IEnumerator UpdateClockRoutine()
    {
        while (true)
        {
            _timeOfDay++;
            if (_timeOfDay > 23)
                _timeOfDay = 0;
            _timeText.text = $"{_timeOfDay}:00";

            yield return new WaitForSeconds(10);
        }
    }

    public Node.Status IsGalleryOpen()
    {
        if (_timeOfDay >= 9 && _timeOfDay < 17)
            return Node.Status.Success;

        return Node.Status.Failure;
    }

    public void RegisterPatron(PatronBehaviour patron)
    {
        _patrons.Enqueue(patron);
    }
    
    public PatronBehaviour DeregisterPatron()
    {
        return _patrons.Dequeue();
    }
}
