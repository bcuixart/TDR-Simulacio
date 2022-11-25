using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//El gènere que pot tenir un individu.
public enum Genere { Masculí, Femení, Otro, PrefieroNoDecirlo}

//Aquesta classe conté tots els gens que tindrà cada idividu
[System.Serializable] public class Genoma
{
    //El gènere de l'individu
    public Genere genere;

    //La llista de gens.
    [Space]
    public List<Gen> gens = new List<Gen>
    { 
        new Gen("Salut", 0, ExclusivitatGen.Ninguna)
    };

    //Un constructor de Genoma. Anirà molt bé per la reproducció
    public Genoma(Genere _genere, List<Gen> _gens)
    {
        genere = _genere;
        gens = _gens;

        return;
    }
}
