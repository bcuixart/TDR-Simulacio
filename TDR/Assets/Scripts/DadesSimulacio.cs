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

    public List<float> velocitatMitjana;
    public List<float> deteccioMitjana;
    public List<float> ansiaReproductivaMitjana;
    public List<float> colorMitjana;
    public List<float> atractiuMitjana;
    public List<float> gestacioMitjana;

    public DadesEspecie(int _id, string _nom)
    {
        id = _id;
        nom = _nom;

        nombreIndividus = new List<int>();

        velocitatMitjana = new List<float>();
        deteccioMitjana = new List<float>();
        ansiaReproductivaMitjana = new List<float>();
        colorMitjana = new List<float>();
        atractiuMitjana = new List<float>();
        gestacioMitjana = new List<float>();

        return;
    }
}
