using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSessionManager : MonoBehaviour
{
    public void LeaveGame()
    {
        // Si eres host, detén el host (esto también detiene cliente y servidor)
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        // Si eres cliente
        else if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Limpia cualquier dato de conexión si usas una clase estática
        ConnectionData.isHost = false;
        ConnectionData.joinCode = "";

        // Regresar al menú (debe estar en la build settings)
        SceneManager.LoadScene("Menu");
    }
}
