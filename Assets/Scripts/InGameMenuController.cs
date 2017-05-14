using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InGameMenuController : GameActors, IPointerDownHandler
{
    private Rotation cameraRotate;

    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private GameObject panel;
    private BoxCollider collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();

    }

    private void Start()
    {
        cameraRotate = GameManager.instance.rotation;
        InitDelegates();

        ActivateMenu(false);
        collider.enabled = false;
    }



    protected override void OnPlayPause(RotationData _rotationData)
    {
        ActivateMenu(true);
        collider.enabled = true;
    }

    protected override void OnPlayStart(RotationData _rotationData)
    {
        ActivateMenu(false);
        collider.enabled = false;
    }

    protected override void PreRotationLogic(RotationData _rotationData)
    {
        //if leaving pause 
        if (_rotationData.currentState == CameraState.Below)
            ActivateMenu(false);
    }

    void ActivateMenu(bool _isActive)
    {
        panel.SetActive(_isActive);
        canvas.alpha = _isActive ? 1 : 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("The cursor entered the selectable UI element.");
        if (GameManager.gameState == GameState.Pause)
        {
            cameraRotate.TriggerRotation(Direction.Up);
        }
        else if (GameManager.gameState == GameState.CompleteLevel)
        {
            cameraRotate.TriggerRotation(Direction.Up);
            GameManager.instance.BeginLevel();
        }
    }
}
