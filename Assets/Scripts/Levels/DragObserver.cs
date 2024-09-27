using Connection;
using Events;
using Player.ActionHandlers;
using System;
using UnityEngine;

public class DragObserver : MonoBehaviour
{
    [SerializeField] private ColorConnectionManager colorConnectionManager;

    private ClickHandler _clickHandler;

    public event Action<Vector3> CameraDragStarted;

    private void Awake()
    {
        _clickHandler = ClickHandler.Instance;

        _clickHandler.DragStartEvent += OnDragStart;
        //_clickHandler.DragEndEvent += OnDragEnd;
    }

    private void OnDestroy()
    {
        _clickHandler.DragStartEvent -= OnDragStart;
        //_clickHandler.DragEndEvent -= OnDragEnd;
    }

    private void OnDragStart(Vector3 position)
    {
        if (colorConnectionManager.TryGetColorNodeInPosition(position, out _))
            return;

        EventsController.Fire(new EventModels.Game.PlayerFingerSwiped());
        CameraDragStarted?.Invoke(position);
    }

    //private void OnDragEnd(Vector3 position)
    //{
    //    EventsController.Fire(new EventModels.Game.PlayerFingerRemoved());
    //}
}
