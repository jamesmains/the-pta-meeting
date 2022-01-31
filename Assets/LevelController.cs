using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    
    
    [SerializeField] private string targetSceneName;
    [SerializeField] private string details;

    [SerializeField] private float inputSpeed;
    
    [SerializeField] private Image resetDisplay;
    [SerializeField] private Image quitDisplay;

    [HideInInspector] public UnityEvent quitLevel, resetLevel;

    private void Awake()
    {
        quitLevel.AddListener(QuitLevel);
        resetLevel.AddListener(ResetLevel);
    }

    public void NextLevel()
    {
        LevelTransition.instance.GotoLevel(targetSceneName,details);
    }

    private void ResetLevel()
    {
        LevelTransition.instance.ResetLevel();
    }

    private void QuitLevel()
    {
        LevelTransition.instance.GotoLevel("MainMenu","");
    }

    private void Update()
    {
        if (resetDisplay == null || quitDisplay == null) return;
        UpdateInputs(KeyCode.R,resetDisplay,resetLevel);
        UpdateInputs(KeyCode.Escape,quitDisplay,quitLevel);
    }

    private void UpdateInputs(KeyCode key, Image display, UnityEvent action)
    {
        if (Input.GetKey(key))
        {
            display.fillAmount += inputSpeed * Time.deltaTime;
            if(display.fillAmount >= 1)
                action.Invoke();
        }
        else
        {
            if (display.fillAmount > 0)
                display.fillAmount -= inputSpeed * Time.deltaTime;
        }
    }
}
