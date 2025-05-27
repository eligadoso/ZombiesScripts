using Unity.Netcode;
using UnityEngine;

public class WeaponBasicsMultiplayer : NetworkBehaviour, IInteractuable
{
    public void Usar()
    {
        if (IsOwner)
        {
            UsarServerRpc();
        }
    }

    [ServerRpc]
    private void UsarServerRpc()
    {
        Debug.Log($"{gameObject.name} ha sido usado por el jugador con ID {OwnerClientId}");
    }
}
