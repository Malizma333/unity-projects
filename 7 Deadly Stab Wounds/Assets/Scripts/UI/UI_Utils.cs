using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Utils : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject LobbyMenu;

    public static GameObject MainMenuInstance;
    public static GameObject LobbyMenuInstance;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuInstance = MainMenu;
        LobbyMenuInstance = LobbyMenu;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
