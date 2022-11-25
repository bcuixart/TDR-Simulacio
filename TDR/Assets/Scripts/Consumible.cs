using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumible : MonoBehaviour
{
    [SerializeField] bool planta;

    [SerializeField] Individu individu;

    [SerializeField] float copsConsumible;
    [SerializeField] float ganaQueTreu;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public float SerMenjat(float velocitatMenjacio)
    {
        if (!planta)
        {
            individu.estat = EstatIndividu.SentMenjat;
        }

        copsConsumible -= velocitatMenjacio;
        if(copsConsumible <= 0)
        {
            if (planta)
            {
                Destroy(gameObject);
            }
            else
            {
                individu.estat = EstatIndividu.Mort;
            }
        }

        return ganaQueTreu * velocitatMenjacio;
    }
}
