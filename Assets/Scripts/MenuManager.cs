using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager MenuManagerInstance;
    public bool GameState;
    public GameObject[] menuElement = new GameObject[2];

    void Start()
    {
        GameState = false;
        MenuManagerInstance = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTheGame()
    {
        GameState = true;
        menuElement[0].SetActive(false);
        GameObject.FindWithTag("particle").GetComponent<ParticleSystem>().Play();
        PlayerPrefs.DeleteAll();
    }

    public void Retry_btn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
