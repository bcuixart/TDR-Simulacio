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
        new Gen("Velocitat", 0, ExclusivitatGen.Ninguna), //Moure's més ràpid a canvi de consumir més energia.
        new Gen("Detecció", 0, ExclusivitatGen.Ninguna), //Detectar millor entorn a canvi de ser més vulnerable per ser tiquismiquis. Si detecta un aliment més nutritiu més lluny que un a prop, anirà al llunyà, cosa que el pot posar en perill si tarda massa a arribar.
        new Gen("Ànsia reproductiva", 0, ExclusivitatGen.Ninguna), //El temps que triga a voler reproduir-se. Voler reproduir-se pot tenir prioritat respecte menjar, per tant, més ànsia reproductiva pot ser perillosa.
        new Gen("Color", 0, ExclusivitatGen.Ninguna),
        new Gen("Atractiu", 0, ExclusivitatGen.Masculí), //Gen masculí: atractiu. Probabilitat de ser acceptat per una femella per reproduir-se.
        new Gen("Gestació", 0, ExclusivitatGen.Femení), //Gen femení: temps de gestació. Si és més curt, els descendents estaran menys desenvolupats.
    };

    //Un constructor de Genoma. Anirà molt bé per la reproducció
    public Genoma(Genere _genere, List<Gen> _gens)
    {
        genere = _genere;
        gens = _gens;

        return;
    }
}
