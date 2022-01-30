using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPositions;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frog"))
        {
            var frog = other.transform.GetComponent<PlayerInput>();
            frog.SetCheckPoint(spawnPositions[(int)frog.frogColor].position);
        }
    }
}
