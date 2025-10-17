using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustinTail : MonoBehaviour
{
    List<Transform> movedObjects = new List<Transform>();

    private void OnCollisionExit2D(Collision2D collision)
    {
        movedObjects.Add(collision.transform);
    }

    private void FixedUpdate()
    {
        foreach(Transform tr in movedObjects)
        {
            if(tr.localPosition.y == 0)
            {
                movedObjects.Remove(tr);
            }

            else
            {
                tr.localPosition += new Vector3(0, 0.2f, 0);
                if (tr.localPosition.y > 0)
                {
                    tr.localPosition = new Vector3(tr.localPosition.x, 0, tr.localPosition.z);
                    movedObjects.Remove(tr);
                }
            }
        }
    }
}
