using Unity.Netcode;
using UnityEngine;

public class weaponFusilMultiplayer : NetworkBehaviour, IInteractuable
{
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float CD;

    private float tiempoUltimoDisparo = -Mathf.Infinity;

    public void Usar()
    {
        if (!IsOwner) return;

        if (Time.time < tiempoUltimoDisparo + CD)
            return;

        tiempoUltimoDisparo = Time.time;
        DispararServerRpc();
    }

    [ServerRpc]
    private void DispararServerRpc()
    {
        if (proyectilPrefab && puntoDisparo)
        {
            GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
            proyectil.GetComponent<NetworkObject>().Spawn(true);
            Debug.Log($"Disparo realizado por el jugador con ID {OwnerClientId}");
        }
    }
}
