using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour
{
    int tilemapSize;

    int randomDistanceX;
    int randomDistanceZ;
    Transform instantiatePosition;
    Transform oldInstantiatePosition;

    int objectToInstantiate;
    float currentSizeX;
    float currentSizeY;
    float previousSizeX = 0;

    
    public GameObject[] objects;

    List<float> objectsSizeX = new List<float>();
    List<float> objectsSizeY = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        tilemapSize = 100;
        instantiatePosition = gameObject.transform.GetChild(0);
        oldInstantiatePosition = gameObject.transform.GetChild(1);

        foreach(GameObject prop in objects)
        {
            BoxCollider trigger = prop.GetComponent<BoxCollider>();
            BoxCollider collider = prop.transform.GetChild(0).transform.gameObject.GetComponent<BoxCollider>();
            objectsSizeX.Add(trigger.size.x);
            objectsSizeY.Add(collider.size.y);
        }

        float z = 0;
        float x = 0;

        while (true)
        {
            if (z >= tilemapSize)
                break;

            while(true)
            {
                if (x >= tilemapSize)
                    break;

                objectToInstantiate = Random.Range(0, objects.Length);
                currentSizeX = objectsSizeX[objectToInstantiate];
                currentSizeY = objectsSizeY[objectToInstantiate];

                randomDistanceX = Random.Range(1, 40);
                randomDistanceZ = Random.Range(0, 20);
                instantiatePosition.localPosition = new Vector3(oldInstantiatePosition.localPosition.x + randomDistanceX, currentSizeY / 2, z + randomDistanceZ);
                float distance = (instantiatePosition.position - oldInstantiatePosition.position).sqrMagnitude;

                if (distance <= (currentSizeX / 2) + (previousSizeX / 2) + 1)
                {
                    instantiatePosition.localPosition = new Vector3((currentSizeX / 2) + (previousSizeX / 2) + 1, instantiatePosition.localPosition.y, instantiatePosition.localPosition.z);
                }

                GameObject obj = Instantiate(objects[objectToInstantiate], instantiatePosition.localPosition, instantiatePosition.rotation, gameObject.transform);
                x += instantiatePosition.position.x - oldInstantiatePosition.position.x;

                oldInstantiatePosition.localPosition = instantiatePosition.localPosition;
                previousSizeX = currentSizeX;               
                //Debug.Log(x);
            }

            x = 0;
            z += 25;
            oldInstantiatePosition.localPosition = new Vector3(0, currentSizeY, z);
            previousSizeX = 0;
            //Debug.Log(z);
        }
        

    }

}
