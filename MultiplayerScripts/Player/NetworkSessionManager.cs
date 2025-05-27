using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSessionManager : MonoBehaviour
{
    public void LeaveGame()
    {
        // Si eres host, det�n el host (esto tambi�n detiene cliente y servidor)
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        // Si eres cliente
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Limpia cualquier dato de conexi�n si usas una clase est�tica
        ConnectionData.isHost = false;
        ConnectionData.joinCode = "";

        // Regresar al men� (debe estar en la build settings)
        SceneManager.LoadScene("Menu");
    }
}
