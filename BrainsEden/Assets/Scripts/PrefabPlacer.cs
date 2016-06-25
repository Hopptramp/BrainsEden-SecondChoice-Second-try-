using UnityEngine;
using System.Collections;

public class PrefabPlacer : MonoBehaviour
{
    float addPosX = 0;
    float addPosY = 0;
    float addPosZ = 0;

    [SerializeField] GameObject[] prefabsToPlace;

	// Use this for initialization
	void Start ()
    {
	
	}
	
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
            Instantiate(prefabsToPlace[0], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Instantiate(prefabsToPlace[1], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Instantiate(prefabsToPlace[2], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Instantiate(prefabsToPlace[3], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Instantiate(prefabsToPlace[4], transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            Instantiate(prefabsToPlace[5], transform.position, Quaternion.identity);
        }
    }
}
