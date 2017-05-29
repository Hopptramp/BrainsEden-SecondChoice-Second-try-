using UnityEngine;
using System.Collections;

public class GameActors : MonoBehaviour {

    protected RotationData rotationData;

	// Use this for initialization
	void Start () {
    }

    protected void InitDelegates()
    {
        GameManager.instance.onPlayStart += OnPlayStart;
        GameManager.instance.onPlayPause += OnPlayPause;
        GameManager.instance.preRotation += PreRotationLogic;
        GameManager.instance.postRotation += PostRotationLogic;
    }

    protected void RemoveDelegates()
    {
        GameManager.instance.onPlayStart -= OnPlayStart;
        GameManager.instance.onPlayPause -= OnPlayPause;
        GameManager.instance.preRotation -= PreRotationLogic;
        GameManager.instance.postRotation -= PostRotationLogic;
    }

    /// <summary>
    /// Delegate that triggers play start logic in other scripts
    /// </summary>
    protected virtual void OnPlayStart(RotationData _rotationData)
    {

    }

    /// <summary>
    /// Delegate that triggers any pauses in logic in other scripts
    /// </summary>
    protected virtual void OnPlayPause(RotationData _rotationData)
    {

    }

    /// <summary>
    /// Delegate that triggers any pre rotation logic in other scripts
    /// </summary>
    protected virtual void PreRotationLogic(RotationData _rotationData)
    {
        rotationData = _rotationData;
    }

    /// <summary>
    /// Delegate that triggers any post rotation logic in other scripts
    /// </summary>
    protected virtual void PostRotationLogic(RotationData _rotationData, bool _isInit)
    {
        rotationData = _rotationData;
    }

}
