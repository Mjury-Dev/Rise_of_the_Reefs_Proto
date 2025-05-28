using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float lifetime = 0.4f;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
