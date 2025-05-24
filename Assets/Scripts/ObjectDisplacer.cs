using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectDisplacer : MonoBehaviour
{
    public GameObject[] objectsToMove;
    public Vector3 displacement;

    [ContextMenu("Displace Now")]
    void DisplaceNow()
    {
        foreach (GameObject obj in objectsToMove)
        {
            if (obj != null)
                obj.transform.position += displacement;
        }
    }
}
