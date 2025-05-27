using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnManager : MonoBehaviour
{
    public List<Transform> spawnPoints;
    private int lastUsedIndex = -1;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("No hay puntos de spawn asignados.");
            return;
        }

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton aún no está listo.");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(AssignSpawnAfterDelay(clientId));
            Debug.Log($"Jugador {clientId} conectado.");
        }
    }

    private IEnumerator AssignSpawnAfterDelay(ulong clientId)
    {
        yield return new WaitForSeconds(0.1f); // Espera para asegurarse de que el objeto esté en escena

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject == null)
            {
                Debug.LogWarning($"El jugador {clientId} aún no tiene PlayerObject.");
                yield break;
            }

            GameObject playerObj = client.PlayerObject.gameObject;

            // Elegir punto de spawn de forma cíclica
            lastUsedIndex = (lastUsedIndex + 1) % spawnPoints.Count;
            Transform spawnPoint = spawnPoints[lastUsedIndex];

            playerObj.transform.position = spawnPoint.position;
            playerObj.transform.rotation = spawnPoint.rotation;

            Debug.Log($"Jugador {clientId} ubicado en punto {lastUsedIndex}");
        }
    }
}
