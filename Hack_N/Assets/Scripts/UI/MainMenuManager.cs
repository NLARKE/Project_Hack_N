using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Stage1";
    public void StartHack()
    {
        SceneManager.LoadScene(gameplaySceneName); // Load Stage Phase
    }

    public void OpenStageSelect()
    {
        Debug.Log("[SYSTEM]: Opening Stage Selection..."); // Stage Select Phase

        // ဒီကြောင်းကို ထည့်သွင်းပေးပါ (StageSelect Scene သို့ ပွင့်သွားစေရန်)
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("StageSelect");
    }

    public void TerminateGame()
    {
        Debug.Log("[SYSTEM]: Terminating Application..."); // Terminate Game Phase
        Application.Quit(); 
    }
}
