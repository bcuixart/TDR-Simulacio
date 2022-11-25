using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe que gestiona coses relacionades amb l'ADN, com ara la reproducció
public class ADN
{
    public ADN(int randomSeed)
    {
        Random.InitState(randomSeed);

        return;
    }

    #region Mitosi
    //Fer la mitosi: torna el mateix genoma que el progenitor però amb possibilitat de mutació.
    //La veritat és que no sé ni si la utilitzaré, però bueno, ja la he feta.
    public static Genoma Mitosi(Genoma progenitor, float probabilitatMutacio)
    {
        //Creem una llista de gens pel descendent, la qual s'anirà omplint.
        List<Gen> gensDescendent = new List<Gen>();

        //Per cada gen del progenitor, el repliquem. Hi ha una probabilitat que muti.
        foreach (Gen gen in progenitor.gens)
        {
            Gen genDescendent = ReplicarGenMitosi(gen, probabilitatMutacio);

            gensDescendent.Add(genDescendent);
        }

        //Creem el genoma amb la llista de gens i amb el "gènere" del progenitor
        return new Genoma(progenitor.genere, gensDescendent);
    }

    static Gen ReplicarGenMitosi(Gen genProgenitor, float probabilitatMutacio)
    {
        //Partim del gen del progenitor
        float gen = genProgenitor.gen;

        //Hi ha una probabilitat de mutació
        float probabilitat = Random.value;
        if(probabilitat <= (probabilitatMutacio / 100f))
        {
            gen += Random.Range(-1f, 1f);
        }

        //Ens assegurem que el gen no sigui major de 1 ni menor de -1
        gen = Mathf.Clamp(gen, -1, 1);

        //tornem el gen modificat
        return new Gen(genProgenitor.nomGen, gen, genProgenitor.exclusivitatGen);
    }
    #endregion

    #region Meiosi
    //Fer la meiosi: barrejar els gens de dos progenitors
    public Genoma Meiosi(Genoma progenitor1, Genoma progenitor2, float probabilitatMutacio)
    {
        //Creem una llista de gens pel descendent, la qual s'anirà omplint.
        List<Gen> gensDescendent = new List<Gen>();

        //Per cada gen dels progenitors, el repliquem.
        for (int i = 0; i < progenitor1.gens.Count; i++)
        {
            Gen genDescendent = ReplicarGenMeiosi(progenitor1.gens[i], progenitor2.gens[i], probabilitatMutacio);

            gensDescendent.Add(genDescendent);
        }

        //Determinem el gènere de forma atzarosa
        float genereAtzar = Random.value;
        Genere genere = (genereAtzar <= 0.5f) ? Genere.Femení : Genere.Masculí;

        //Creem el genoma amb la llista de gens i amb el "gènere" del progenitor
        return new Genoma(genere, gensDescendent);
    }

    static Gen ReplicarGenMeiosi(Gen genProgenitorPare, Gen genProgenitorMare, float probabilitatMutacio)
    {
        //Partim dels gens del progenitors
        float gen1 = genProgenitorPare.gen;
        float gen2 = genProgenitorMare.gen;

        float nouGen = 0;

        //Mirem l'exclusivitat del gen
        switch (genProgenitorPare.exclusivitatGen)
        {
            case ExclusivitatGen.Femení:

                //Si el gen és exclusiu femení, tornem el gen de la mare.
                nouGen = gen2;

                break;

            case ExclusivitatGen.Masculí:

                //Si el gen és exclusiu masculí, tornem el gen del pare.
                nouGen = gen1;

                break;

            case ExclusivitatGen.Ninguna:

                //Si el gen no és ni masculí ni femení, determinem el mètode de barrejació de gens
                float metodeBarrejació = Random.value;

                if (metodeBarrejació <= 0.45f)
                {
                    //45% possibilitat que sigui el gen del pare
                    nouGen = gen1;
                }
                else if (metodeBarrejació <= 0.9f)
                {
                    //45% de possibilitat que sigui el gen de la mare
                    nouGen = gen2;
                }
                else
                {
                    //10% de possibilitat que sigui la mitjana dels dos gens
                    nouGen = (gen1 + gen2) / 2f;
                }

                break;
        }
        

        //A part d'això, hi ha una probabilitat de mutació
        float probabilitat = Random.value;
        if (probabilitat <= (probabilitatMutacio / 100f))
        {
            nouGen += Random.Range(-1f, 1f);
        }

        //Ens assegurem que el gen no sigui major de 1 ni menor de -1
        nouGen = Mathf.Clamp(nouGen, -1, 1);

        //Tornem el gen modificat
        return new Gen(genProgenitorPare.nomGen, nouGen, genProgenitorMare.exclusivitatGen);
    }
    #endregion
}
