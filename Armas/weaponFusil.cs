using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class weaponFusil : MonoBehaviour, IInteractuable
{
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float CD;

    private float tiempoUltimoDisparo = -Mathf.Infinity;
    public void Usar()
    {
        //-Mathf.Infinity es como decir: "hace infinito tiempo que no hago Usar()"
        //Eso permite que el primer disparo se haga sin espera
        //Luego, el juego guarda la hora real del disparo y empieza a contar normalmente
        // Si el tiempo actual es menor que el tiempo permitido, salimos
        if (Time.time < tiempoUltimoDisparo + CD)
            return;
        
        if (proyectilPrefab && puntoDisparo)
        {
            Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
            tiempoUltimoDisparo = Time.time;
            Debug.Log("Disparo realizado");
        }
    }
}
