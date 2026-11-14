using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoController : MonoBehaviour
{
    [Header("Main Menu Scene Name")]
    [SerializeField] private string mainSceneName = "MainMenu"; // သင့်ရဲ့ Main Menu Scene အမည်

    void Update()
    {
        // ကစားသမားက Escape ခလုတ် (သို့မဟုတ် Screen ကို နှိပ်လျှင်) Main Menu သို့ ပြန်သွားရန်
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMainMenu();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}