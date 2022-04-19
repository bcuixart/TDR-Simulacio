using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExclusivitatGen { Ninguna, Masculí, Femení }

//Aquesta classe és la informació que té cada gen
[System.Serializable] public class Gen
{
    //El nom del gen: "Velocitat", "Fertilitat"...
    public string nomGen;

    //El valor del gen. Sempre serà entre -1 i 1
    public float gen;

    //Saber si el gen és exclusiu d'un gènere. Per exemple, l'atractiu dels mascles. Si un gen és exlusiu d'un gènere, quan uns progenitors es reprodueixen només es té en compte el gen del progenitor del gènere corresponent.
    //És a dir, el fill d'un cèrvol atractiu serà atractiu. Els gens de la mare no influeixen.
    public ExclusivitatGen exclusivitatGen;


    //Això és un "constructor" de gen. Serà la mar d'útil per fer el genoma base de les espècies
    public Gen(string _nomGen, float _gen, ExclusivitatGen _exclusivitatGen)
    {
        nomGen = _nomGen;
        gen = _gen;
        exclusivitatGen = _exclusivitatGen;

        return;
    }
}
