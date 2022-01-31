using System.Collections;
using System.Collections.Generic;
using JimJam;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] float waitTime;

    [SerializeField] private string currentLevel;
    [SerializeField] private string currentDetails;
    
    [SerializeField] private TextMeshProUGUI detailsDisplay;

    [SerializeField] private UnityEvent onTransitionStart;
    [SerializeField] private UnityEvent onTransitionLoaded;
    [SerializeField] private UnityEvent onTransitionEnd;
    
    public static LevelTransition instance;

    private bool _ready = true;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else { instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void ResetLevel()
    {
        GotoLevel(currentLevel, currentDetails);
    }
    
    public void GotoLevel(string target, string targetDetails)
    {
        if (!_ready) return;
        
        if ((SceneUtility.GetBuildIndexByScenePath(target) >= 0))
        { 
            currentLevel = target;
            currentDetails = targetDetails;
            _ready = false;
            StartCoroutine(Goto(currentLevel));
            detailsDisplay.text = targetDetails;
        }
        else { Debug.LogError("No scene by that name exists... check the build settings."); }
    }

    IEnumerator Goto(string t)
    {
        onTransitionStart.Invoke();
        yield return new WaitForSeconds(waitTime);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(t);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        onTransitionLoaded.Invoke();
        
        yield return new WaitForSeconds(waitTime);
        onTransitionEnd.Invoke();
        _ready = true;
    }
}
