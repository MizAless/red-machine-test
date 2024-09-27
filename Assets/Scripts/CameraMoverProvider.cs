using UnityEngine;

public class CameraMoverProvider : MonoBehaviour
{
    [SerializeField] DragObserver _dragObserver;

    private void Awake()
    {
        CameraMover.Instance.Init(_dragObserver);
    }
}
