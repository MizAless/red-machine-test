using UnityEngine;
using Player.ActionHandlers;
using Camera;
using Player;
using Utils.Singleton;
using Events;
using static Events.EventModels.Game;

public class CameraMover : DontDestroyMonoBehaviourSingleton<CameraMover>
{
    [SerializeField] private float _speed = 100f;
    [SerializeField, Range(0.8f, 1.0f)] private float _inertiaFactor = 0.95f;
    [SerializeField] private float _minVelocityThreshold = 0.01f;

    [SerializeField] private Vector2 _LeftBotoomPointOfBounds;
    [SerializeField] private Vector2 _RightTopPointOfBounds;

    private DragObserver _dragObserver;

    private UnityEngine.Camera _camera;

    private ClickHandler _clickHandler;

    private Vector3 _velocity;
    private Vector3 _previousDragPosition;

    private bool _isDragging = false;

    private Vector3 _defaultPosition = new Vector3(0, 0, -10);

    private void OnDestroy()
    {
        EventsController.Unsubscribe<EventModels.Game.TargetColorNodesFilled>(OnTargetColorNodesFilled);

        _dragObserver.CameraDragStarted -= OnDragStart;
        _clickHandler.DragEndEvent -= OnDragEnd;
    }

    private void LateUpdate()
    {
        if (_camera == null) 
            return;

        Move();
    }

    public void Init(DragObserver dragObserver)
    {
        _dragObserver = dragObserver;

        _clickHandler = ClickHandler.Instance;
        _camera = CameraHolder.Instance.MainCamera;

        EventsController.Subscribe<EventModels.Game.TargetColorNodesFilled>(this, OnTargetColorNodesFilled);

        _dragObserver.CameraDragStarted += OnDragStart;
        _clickHandler.DragEndEvent += OnDragEnd;
    }

    private void Move()
    {
        if (!_isDragging)
        {
            _velocity *= _inertiaFactor;

            Vector3 newPosition = _camera.transform.position + _velocity * Time.smoothDeltaTime;

            newPosition.x = Mathf.Clamp(newPosition.x, _LeftBotoomPointOfBounds.x, _RightTopPointOfBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, _LeftBotoomPointOfBounds.y, _RightTopPointOfBounds.y);
            newPosition.z = _camera.transform.position.z;

            _camera.transform.position = newPosition;

            if (_velocity.magnitude < _minVelocityThreshold)
            {
                _velocity = Vector3.zero;
            }

            return;
        }

        Vector3 dragDirection = _previousDragPosition - _camera.ScreenToWorldPoint(Input.mousePosition);
        dragDirection.z = 0;

        _velocity = dragDirection * _speed;

        Vector3 dragPosition = _camera.transform.position + _velocity * Time.smoothDeltaTime;

        dragPosition.x = Mathf.Clamp(dragPosition.x, _LeftBotoomPointOfBounds.x, _RightTopPointOfBounds.x);
        dragPosition.y = Mathf.Clamp(dragPosition.y, _LeftBotoomPointOfBounds.y, _RightTopPointOfBounds.y);

        _camera.transform.position = dragPosition;

        _previousDragPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Reset()
    {
        _camera.transform.position = _defaultPosition;

        _isDragging = false;
        _velocity = Vector3.zero;
    }

    private void OnDragStart(Vector3 position)
    {
        if (PlayerController.PlayerState != PlayerState.Scrolling)
            return;

        _previousDragPosition = position;
        _isDragging = true;
    }

    private void OnDragEnd(Vector3 position)
    {
        if (PlayerController.PlayerState != PlayerState.Scrolling)
            return;

        _previousDragPosition = Vector3.zero;
        _isDragging = false;
    }

    private void OnTargetColorNodesFilled(EventModels.Game.TargetColorNodesFilled e)
    {
        Reset();
    }
}
