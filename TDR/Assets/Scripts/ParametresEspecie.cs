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
    public List<int> dietaPrimaria;
    public List<int> dietaSecundaria;
    public List<int> dietaTerciaria;
    public List<int> depredadors;

    [Space]
    public float velocitatCaminarBase;
    public float velocitatCaminarVariacio;
    public float velocitatCorrerBase;
    public float velocitatCorrerVariacio;
    public float tempsAfamacioBase;
    public float tempsAfamacioVariacio;
    public float tempsAssedegamentBase;
    public float tempsAssedegamentVariacio;

    [Space]
    public float tempsGanesReproduccioBase;
    public float tempsGanesReproduccioVariacio;
    public float tempsGestacioBase;
    public float tempsGestacioVariacio;
    public int fillsMinims;
    public int fillsMaxims;

    [Space]
    public float tempsDesenvolupamentCriesBase;
    public float tempsDesenvolupamentCriesVariacio;
    public float desenvolupamentCriesVariacio;

    [Space]
    public float deteccioBase;
    public float deteccioVariacio;

    [Space]
    public float temperaturaBase;
    public float temperaturaVariacio;

    [Space]
    public bool grups;
    public int grupsMaxim;

    public ParametresEspecie(int _id, string _nomS, string _nomP)
    {
        id = _id;
        nomSingular = _nomS;
        nomPlural = _nomP;
        return;
    }
}
