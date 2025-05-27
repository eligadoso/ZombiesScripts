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
            Debug.Log("Jugador conectado. Total jugadores: " + NetworkManager.Singleton.ConnectedClientsList.Count);
        }
    }

    private System.Collections.IEnumerator AssignSpawnAfterDelay(ulong clientId)
    {
        yield return new WaitForSeconds(0.1f); // espera por seguridad

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            GameObject playerObj = client.PlayerObject.gameObject;

            // Asigna posición solo en el servidor
            Vector3 spawnPos = GetValidSpawnPoint();
            playerObj.transform.position = spawnPos;

            Debug.Log($"Asignado spawn a jugador {clientId} en {spawnPos}");
        }

    }
    Vector3 GetValidSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No hay puntos de spawn definidos.");
            return Vector3.zero; // fallback
        }

        lastUsedIndex = (lastUsedIndex + 1) % spawnPoints.Count;
        return spawnPoints[lastUsedIndex].position;
    }
}
