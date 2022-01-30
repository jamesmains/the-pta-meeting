using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    [SerializeField] private string details;
    public void NextLevel()
    {
        LevelTransition.instance.GotoLevel(targetSceneName,details);
    }
}
