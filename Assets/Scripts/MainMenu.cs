using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject chooseLevelMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChooseLevelStart()
    {
        chooseLevelMenu.SetActive(true);
        startMenu.SetActive(false);
    }

    public void LoadEasyScene()
    {
        SceneManager.LoadScene("HountedHouseLvl1");
    }
}
