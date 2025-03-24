using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOneself : MonoBehaviour
{
    public void DestroyOneselves()
    {
        Destroy(transform.parent.gameObject);
    }
}
