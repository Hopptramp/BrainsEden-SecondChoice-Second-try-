using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class PrefabPlacer : MonoBehaviour
{
    float addPosX = 0;
    float addPosY = 0;
    float addPosZ = 0;

    [SerializeField] GameObject[] prefabsToPlace;
    GameObject prefab;
    // Use this for initialization
    void Start ()
    {
	
	}

#if UNITY_EDITOR
    // Update is called once per frame
    void Update ()
    {
        addPosX = 0;
        addPosY = 0;
        addPosZ = 0;

        // movement
        if (Input.GetKeyDown(KeyCode.W))
        {
            ++addPosZ;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            --addPosZ;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            --addPosX;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ++addPosX;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ++addPosY;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            --addPosY;
        }


        
        transform.position += new Vector3(addPosX, addPosY, addPosZ);

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToPlace[0]);
            prefab.transform.position = transform.position;
            //Instantiate(prefabsToPlace[0], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToPlace[1]);
            prefab.transform.position = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToPlace[2]);
            prefab.transform.position = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToPlace[3]);
            prefab.transform.position = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToPlace[4]);
            prefab.transform.position = transform.position;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            prefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToPlace[5]);
            prefab.transform.position = transform.position;
        }
    }
#endif
}
