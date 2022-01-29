using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputTrigger : MonoBehaviour
{
    [SerializeField] private bool isActive;
    [SerializeField] private bool triggerOnAny;
    [SerializeField] private KeyCode triggerKey;

    [SerializeField] private UnityEvent onTrigger;
    
    private void Update()
    {
        if (!isActive) return;
        if(Input.GetKeyDown(triggerKey) || (triggerOnAny && Input.anyKey))
            onTrigger.Invoke();
    }
}
