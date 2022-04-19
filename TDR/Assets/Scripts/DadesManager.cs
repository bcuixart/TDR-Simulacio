using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadesManager : MonoBehaviour
{
    public List<int> nombreIndividusNormals;
    public List<int> nombreIndividusPersonalitzats;

    public DadesSimulacio dades;

    public static DadesManager instance;

    GameManager gameMana;
    int copsComptats;

    void Awake()
    {
        instance = this;
        copsComptats = 0;

        InvokeRepeating("ComptarIndividus", 10, 30);
    }

    void ComptarIndividus()
    {
        if(gameMana == null)
        {
            gameMana = GameManager.instance;
        }

        for (int i = 0; i < nombreIndividusNormals.Count; i++)
        {
            // if (nombreIndividusNormals[i] > 0)
            //{
            DadesEspecie dads = dades.especiesNormals[i];
            List<int> nombs = dads.nombreIndividus;
            nombs.Add(nombreIndividusNormals[i]);

            if (nombreIndividusNormals[i] > 0)
            {
                float vel = 0;
                float det = 0;
                float aR = 0;
                float clr = 0;
                float atr = 0;
                float gest = 0;

                foreach (Individu ind in gameMana.individusNormals[0].individus)
                {
                    vel += ind.genoma.gens[0].gen;
                    det += ind.genoma.gens[1].gen;
                    aR += ind.genoma.gens[2].gen;
                    clr += ind.genoma.gens[3].gen;
                    atr += ind.genoma.gens[4].gen;
                    gest += ind.genoma.gens[5].gen;
                }

                vel /= gameMana.individusNormals[i].individus.Count;
                det /= gameMana.individusNormals[i].individus.Count;
                aR /= gameMana.individusNormals[i].individus.Count;
                clr /= gameMana.individusNormals[i].individus.Count;
                atr /= gameMana.individusNormals[i].individus.Count;
                gest /= gameMana.individusNormals[i].individus.Count;

                dades.especiesNormals[i].velocitatMitjana.Add(vel);
                dades.especiesNormals[i].deteccioMitjana.Add(det);
                dades.especiesNormals[i].ansiaReproductivaMitjana.Add(aR);
                dades.especiesNormals[i].colorMitjana.Add(clr);
                dades.especiesNormals[i].atractiuMitjana.Add(atr);
                dades.especiesNormals[i].gestacioMitjana.Add(gest);
            }
            else
            {
                dades.especiesNormals[i].velocitatMitjana.Add(0);
                dades.especiesNormals[i].deteccioMitjana.Add(0);
                dades.especiesNormals[i].ansiaReproductivaMitjana.Add(0);
                dades.especiesNormals[i].colorMitjana.Add(0);
                dades.especiesNormals[i].atractiuMitjana.Add(0);
                dades.especiesNormals[i].gestacioMitjana.Add(0);
            }
        }

        for (int i = 0; i < nombreIndividusPersonalitzats.Count; i++)
        {
            DadesEspecie dads = dades.especiesPersonalitzades[i];
            List<int> nombs = dads.nombreIndividus;
            nombs.Add(nombreIndividusPersonalitzats[i]);

            if (nombreIndividusPersonalitzats[i] > 0)
            {
                float vel = 0;
                float det = 0;
                float aR = 0;
                float clr = 0;
                float atr = 0;
                float gest = 0;

                foreach (Individu ind in gameMana.individusPersonalitzats[0].individus)
                {
                    vel += ind.genoma.gens[0].gen;
                    det += ind.genoma.gens[1].gen;
                    aR += ind.genoma.gens[2].gen;
                    clr += ind.genoma.gens[3].gen;
                    atr += ind.genoma.gens[4].gen;
                    gest += ind.genoma.gens[5].gen;
                }

                vel /= gameMana.individusPersonalitzats[i].individus.Count;
                det /= gameMana.individusPersonalitzats[i].individus.Count;
                aR /= gameMana.individusPersonalitzats[i].individus.Count;
                clr /= gameMana.individusPersonalitzats[i].individus.Count;
                atr /= gameMana.individusPersonalitzats[i].individus.Count;
                gest /= gameMana.individusPersonalitzats[i].individus.Count;

                dades.especiesPersonalitzades[i].velocitatMitjana.Add(vel);
                dades.especiesPersonalitzades[i].deteccioMitjana.Add(det);
                dades.especiesPersonalitzades[i].ansiaReproductivaMitjana.Add(aR);
                dades.especiesPersonalitzades[i].colorMitjana.Add(clr);
                dades.especiesPersonalitzades[i].atractiuMitjana.Add(atr);
                dades.especiesPersonalitzades[i].gestacioMitjana.Add(gest);
            }
            else
            {
                dades.especiesPersonalitzades[i].velocitatMitjana.Add(0);
                dades.especiesPersonalitzades[i].deteccioMitjana.Add(0);
                dades.especiesPersonalitzades[i].ansiaReproductivaMitjana.Add(0);
                dades.especiesPersonalitzades[i].colorMitjana.Add(0);
                dades.especiesPersonalitzades[i].atractiuMitjana.Add(0);
                dades.especiesPersonalitzades[i].gestacioMitjana.Add(0);
            }
        }

        copsComptats++;
    }
}
