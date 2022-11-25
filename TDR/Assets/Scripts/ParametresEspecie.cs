using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArticleGenere { Masculí, Femení, Neutre }

[System.Serializable]
public class ParametresEspecie
{
    public int id;

    [Space]
    public string nomSingular;
    public string nomPlural;

    public ArticleGenere articleGenere;

    [Space]
    public List<int> dietaNormal;
    public List<int> dietaPersonalitzada;

    [Space]
    public float velocitat;
    public float tempsAfamacio;
    public float tempsAssedegament;

    [Space]
    public float tempsGanesReproduccio;
    public float tempsGestacio;
    public int fillsMinims;
    public int fillsMaxims;

    [Space]
    public float probabilitatInfeccio;

    [Space]
    public float tempsDesenvolupamentCries;

    public ParametresEspecie(int _id, string _nomS, string _nomP)
    {
        id = _id;
        nomSingular = _nomS;
        nomPlural = _nomP;
        return;
    }
}
