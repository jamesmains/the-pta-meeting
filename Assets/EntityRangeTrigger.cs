using System;
using System.Collections;
using System.Collections.Generic;
using JimJam.Gameplay;
using UnityEngine;
using UnityEngine.Events;

public class EntityRangeTrigger : MonoBehaviour
{
    [SerializeField] private string targetEntity;
    [SerializeField] private UnityEvent onEnterTrigger;
    [SerializeField] private UnityEvent onExitTrigger;
    
    [Header("Debug")]
    [SerializeField] private bool debugging;

    #region 3D Trigger Cols

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetEntity))
        {
            onEnterTrigger.Invoke();
            if (debugging)
                print($"{other.gameObject.name} collided with {gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetEntity))
        {
            onExitTrigger.Invoke();
            if (debugging)
                print($"{other.gameObject.name} left range of {gameObject.name}");
        }
    }

    #endregion

    #region 2D Trigger Cols

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetEntity))
        {
            onEnterTrigger.Invoke();
            if (debugging)
                print($"{other.gameObject.name} collided with {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetEntity))
        {
            onExitTrigger.Invoke();
            if (debugging)
                print($"{other.gameObject.name} left range of {gameObject.name}");
        }
    }

    #endregion    

    
}
