using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadesManager : MonoBehaviour
{
    public List<int> nombreIndividusNormals;
    public List<int> nombreIndividusNormalsInfectats;
    public List<int> nombreIndividusPersonalitzats;
    public List<int> nombreIndividusPersonalitzatsInfectats;

    public DadesSimulacio dades;

    public static DadesManager instance;

    GameManager gameMana;
    int copsComptats;

    void Awake()
    {
        instance = this;
        copsComptats = 0;
    }

    IEnumerator Start()
    {
        gameMana = GameManager.instance;

        yield return new WaitUntil(() => gameMana.comencat);

        InvokeRepeating("ComptarIndividus", 0, 30);
    }

    void ComptarIndividus()
    {
        if (gameMana.arribatTempsMaxim)
        {
            return;
        }

        for (int i = 0; i < nombreIndividusNormals.Count; i++)
        {
            // if (nombreIndividusNormals[i] > 0)
            //{
            DadesEspecie dads = dades.especiesNormals[i];

            List<int> nombs = dads.nombreIndividus;
            nombs.Add(nombreIndividusNormals[i]);

            List<int> nombsInf = dads.nombreIndividusInfectats;
            nombsInf.Add(nombreIndividusNormalsInfectats[i]);

            List<float> nombsPer = dads.percentatgeInfectats;
            float percentatge = (float) nombreIndividusNormalsInfectats[i] / (float) nombreIndividusNormals[i];
            nombsPer.Add(100f * percentatge);

            if (nombreIndividusNormals[i] <= 0)
            {
                continue;
            }

            float sal = 0;

            foreach (Individu ind in gameMana.individusNormals[i].individus)
            {
                sal += ind.genoma.gens[0].gen;
            }

            sal /= gameMana.individusNormals[i].individus.Count;

            dades.especiesNormals[i].salutMitjana.Add(sal);
        }

        for (int i = 0; i < nombreIndividusPersonalitzats.Count; i++)
        {
            DadesEspecie dads = dades.especiesPersonalitzades[i];

            List<int> nombs = dads.nombreIndividus;
            nombs.Add(nombreIndividusPersonalitzats[i]);

            List<int> nombsInf = dads.nombreIndividusInfectats;
            nombsInf.Add(nombreIndividusPersonalitzatsInfectats[i]);

            List<float> nombsPer = dads.percentatgeInfectats;
            float percentatge = (float)nombreIndividusPersonalitzatsInfectats[i] / (float)nombreIndividusPersonalitzats[i];
            nombsPer.Add(100f * percentatge);

            if (nombreIndividusPersonalitzats[i] <= 0)
            {
                continue;
            }

            float sal = 0;

            foreach (Individu ind in gameMana.individusPersonalitzats[0].individus)
            {
                sal += ind.genoma.gens[0].gen;
            }

            sal /= gameMana.individusPersonalitzats[i].individus.Count;

            dades.especiesPersonalitzades[i].salutMitjana.Add(sal);
        }

        copsComptats++;
    }
}
