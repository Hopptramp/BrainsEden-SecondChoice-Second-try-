using UnityEngine;
using System.Collections;
#if UNITY_5_3_OR_NEWER
	using UnityEngine.SceneManagement;
#endif
public class GoalEnter : MonoBehaviour
{

    public float delay = 1.0f;
    private float timer = 0;
    private bool triggered = false;
    public bool disableSceneLoad;
    public int sceneNumber = 1;


    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
			timer += Time.deltaTime;
			#if UNITY_5_3_OR_NEWER
				if (timer >= delay && !disableSceneLoad)
					SceneManager.LoadScene(sceneNumber);
			#endif
        }

    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            triggered = true;
            col.GetComponent<Player_Movement>().DisableInput();
        }
    }
}
