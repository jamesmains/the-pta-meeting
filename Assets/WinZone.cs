using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinZone : MonoBehaviour
{
    [SerializeField] private bool[] playerChecks;
    [SerializeField] private UnityEvent onWin;
    
    public void ToggleCheck(int flag, bool state)
    {
        playerChecks[flag] = state;
        CheckWin();
    }
    
    private void CheckWin()
    {
        foreach(var b in playerChecks)
            if (!b)
                return;
        
        onWin.Invoke();
    }
}
