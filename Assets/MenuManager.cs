using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Image guide;
    public static MenuManager instance;
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayGame()
    {
        SceneFader.instance.ShowGuideThenLoad(guide, "Level1", 10f);
    }

    public void QuitGame()
    {
        SceneFader.instance.FadeToScene("quit");
        Application.Quit();
    }
    
}
