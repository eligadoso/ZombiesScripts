using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static string LastJoinCode;

    public async Task<string> CreateRelay(int maxPlayers = 4)
    {
        await UGSAuthenticator.EnsureInitializedAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("❌ No autenticado. Abortando creación del Relay.");
            return null;
        }


        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            var relayData = new RelayServerData(alloc, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
            NetworkManager.Singleton.StartHost();

            Debug.Log($"🎮 Host iniciado con código: {joinCode}");
            return joinCode;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ Error creando relay: " + ex.Message);
            return null;
        }

    }

    public async Task<bool> JoinRelay(string joinCode)
    {
        await UGSAuthenticator.EnsureInitializedAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.LogError("❌ No autenticado. Abortando unión al Relay.");
            return false;
        }
        try
        {

            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var relayData = new RelayServerData(joinAlloc, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
            NetworkManager.Singleton.StartClient();

            Debug.Log($"[Relay] Cliente conectado con Join Code: {joinCode}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error al unirse al relay: " + ex.Message);
            return false;
        }
    }
}
