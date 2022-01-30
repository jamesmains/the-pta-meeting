using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPoint : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private void FixedUpdate()
    {
        transform.position = LerpByDistance(pointB.position,pointA.position,Vector3.Distance(pointB.position, pointA.position)/2);
    }
    
    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }
}
