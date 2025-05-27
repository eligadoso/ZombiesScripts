using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    public RelayManager relayManager;
    public TextMeshProUGUI joinCodeText; // Opcional, se puede dejar vacío
    public bool autoStartAsHost = false; // Activa el modo "AutoStartHost"
                                         //Este modo era solo para probar recomendable dejar desactivado.

    async void Start()
    {
        Debug.Log("Iniciando NetworkBootstrap...");
        await UGSAuthenticator.EnsureInitializedAsync();

        if (autoStartAsHost || ConnectionData.isHost)
        {
            Debug.Log("Modo HOST");
            string code = await relayManager.CreateRelay();
            joinCodeText.text = code;
        }
        else if (!ConnectionData.isHost)
        {
            Debug.Log("Modo CLIENTE");
            string code = ConnectionData.joinCode;
            await relayManager.JoinRelay(code);
            joinCodeText.text = code;
        }
        else
        {
            Debug.LogError("ConnectionData.Instance es NULL.");
        }
    }

}
