using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DadesSimulacio
{
    public List<DadesEspecie> especiesNormals = new List<DadesEspecie>();
    public List<DadesEspecie> especiesPersonalitzades = new List<DadesEspecie>();
}

[System.Serializable]
public class DadesEspecie
{
    public int id;
    public string nom;

    public List<int> nombreIndividus;
    public List<int> nombreIndividusInfectats;
    public List<float> percentatgeInfectats;

    public List<float> salutMitjana;

    public DadesEspecie(int _id, string _nom)
    {
        id = _id;
        nom = _nom;

        nombreIndividus = new List<int>();
        nombreIndividusInfectats = new List<int>();
        percentatgeInfectats = new List<float>();

        salutMitjana = new List<float>();

        return;
    }
}
