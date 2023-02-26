using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InformacioSimulacio
{
    public static InformacioSimulacio instance;

    public string nomSimulacio;
    public string ubicacioSimulacio;
    public string dataCreacio;

    public float probabilitatMutacio = 10f;
    public float infeccioHumans = 1f;

    public float tempsMaxim = -1;
    public float tempsTotal;

    public int randomSeed;

    public List<int> individusPerFerApareixerNormal = new List<int>();
    public List<int> individusPerFerApareixerPersonalitzat = new List<int>();

    public List<Notificacio> notificacions;

    public bool finalitzada;

    public DadesSimulacio dades;

    public InformacioSimulacio(float mut, int indvsNrm, int indvsPrs)
    {
        probabilitatMutacio = mut;

        for (int i = 0; i < indvsNrm; i++)
        {
            individusPerFerApareixerNormal.Add(0);
        }

        for (int i = 0; i < indvsPrs; i++)
        {
            individusPerFerApareixerPersonalitzat.Add(0);
        }

        notificacions = new List<Notificacio>();

        return;
    }
}
