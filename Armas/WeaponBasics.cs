using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBasics : MonoBehaviour, IInteractuable
{
    public void Usar()
    {
        Debug.Log($"{gameObject.name} ha sido usado");
    }
}
