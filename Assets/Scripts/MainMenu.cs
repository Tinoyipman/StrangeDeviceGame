using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string firstGame;
    public string secondGame;
    public string thirdGame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameOne()
    {
        SceneManager.LoadScene(firstGame);
    }

    public void StartGameTwo()
    {
        SceneManager.LoadScene(secondGame);
    }

    public void StartGameThree()
    {
        SceneManager.LoadScene(thirdGame);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quiting");
    }
}
