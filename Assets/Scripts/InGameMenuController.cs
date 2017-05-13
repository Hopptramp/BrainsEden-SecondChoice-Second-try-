using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InGameMenuController : GameActors, IPointerEnterHandler
{
    private Rotation cameraRotate;

    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject panel;

    private void Start()
    {
        cameraRotate = GameManager.instance.rotation;
        InitDelegates();

        ActivateMenu(false);
    }



    protected override void OnPlayPause(RotationData _rotationData)
    {
        ActivateMenu(true);
    }

    protected override void OnPlayStart(RotationData _rotationData)
    {
        ActivateMenu(false);
    }

    protected override void PreRotationLogic(RotationData _rotationData)
    {
        if (_rotationData.currentState == CameraState.Below)
            ActivateMenu(false);
    }

    void ActivateMenu(bool _isActive)
    {
        panel.SetActive(_isActive);
        canvas.alpha = _isActive ? 1 : 0;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("The cursor entered the selectable UI element.");
        Direction direction;
        Vector2 dir = Input.GetTouch(0).deltaPosition;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
                direction = Direction.Left;
            else
                direction = Direction.Right;
        }
        else
        {
            if (dir.y > 0)
                direction = Direction.Down;
            else
                direction = Direction.Up;
        }


            cameraRotate.TriggerRotation(direction);
    }
}
