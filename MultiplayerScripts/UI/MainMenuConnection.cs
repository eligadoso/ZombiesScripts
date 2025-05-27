using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuConnection : MonoBehaviour
{
    public TMP_InputField inputJoinCode;

    public void OnCreateHost()
    {
        ConnectionData.isHost = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnJoinClient()
    {
        ConnectionData.isHost = false;
        ConnectionData.joinCode = inputJoinCode.text.Trim().ToUpper();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
