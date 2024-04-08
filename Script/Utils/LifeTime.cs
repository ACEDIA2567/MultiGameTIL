using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] float DesTime = 2.0f;

    void Start()
    {
        Destroy(gameObject, DesTime);
    }
}
