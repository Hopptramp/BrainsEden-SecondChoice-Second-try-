using UnityEngine;
using System.Collections;

public class PrefabPlacer : MonoBehaviour
{
    float addPosX = 0;
    float addPosY = 0;
    float addPosZ = 0;

    [SerializeField] GameObject prefabToPlace;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(prefabToPlace, transform.position, Quaternion.identity);
        }
    }
}
