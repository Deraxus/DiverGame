using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        // Затемнение экрана
        SceneManager.LoadScene("Level1");
    }

    public void GameFinished()
    {
        
    }

    public void MenuReturn()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void MakeSignal()
    {
        // Проиграть звук
        // Осветить локацию
    }
    
}
