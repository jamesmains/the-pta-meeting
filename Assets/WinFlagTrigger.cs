using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinFlagTrigger : MonoBehaviour
{
    [SerializeField] private WinZone wZone;
    [SerializeField] private int targetFrog;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Frog"))
        {
            var frog = other.transform.GetComponent<PlayerInput>();
            if (targetFrog != (int) frog.frogColor) return;
            wZone.ToggleCheck(targetFrog,true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Frog"))
        {
            var frog = other.transform.GetComponent<PlayerInput>();
            if (targetFrog != (int) frog.frogColor) return;
            wZone.ToggleCheck(targetFrog,false);
        }
    }
}
