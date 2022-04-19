using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InformacioSimulacio
{
    public static InformacioSimulacio instance;

    public List<int> individusPerFerApareixerNormal = new List<int>();
    public List<int> individusPerFerApareixerPersonalitzat = new List<int>();

    public InformacioSimulacio(int indvs)
    {
        for (int i = 0; i < indvs; i++)
        {
            individusPerFerApareixerNormal.Add(0);
            individusPerFerApareixerPersonalitzat.Add(0);
        }

        return;
    }
}
