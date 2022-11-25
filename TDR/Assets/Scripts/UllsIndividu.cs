using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UllsIndividu : MonoBehaviour
{
    [SerializeField] Individu individu;
    //public Transform cap;

    public float angleDeVista;

    [SerializeField] LayerMask menjarMask;
    [SerializeField] LayerMask individusPerReproduirseMask;
    [SerializeField] LayerMask obstacleMask;

    public List<Transform> menjarVist;
    public List<Transform> individusPerReproduirseVistos;

    GameManager gameMana;

    void Start()
    {
        gameMana = GameManager.instance;
    }

    public void VeureMenjar()
    {
        menjarVist.Clear();

        Collider[] menjarEnElRadiDeVista = Physics.OverlapSphere(transform.position, 150, menjarMask);
        for (int i = 0; i < menjarEnElRadiDeVista.Length; i++)
        {
            Transform menjar = menjarEnElRadiDeVista[i].transform;

            if (menjar.gameObject.layer == LayerMask.NameToLayer("Animal")) //Mirar si és un animal.
            {
                string nomMenjar = menjar.name;
                nomMenjar = nomMenjar.Replace("Especie_", "");
                nomMenjar = nomMenjar.Replace("EspeciePersonalitzada_", "");
                int menjarID = int.Parse(nomMenjar);

                if (menjar == transform || !individu.especie.dietaNormal.Contains(menjarID))
                {
                    continue; //Si es determina que aquest animal no forma part de la dieta, no s'afegeix.
                }
            }

            //Si es segueix (plantes automàticament en herbívors o animals vàlids en carnívors)
            menjarVist.Add(menjar);
        }
    }

    public void VeureIndividusPerReproduirse()
    {
        individusPerReproduirseVistos.Clear();

        Collider[] individusEnElRadiDeVista = Physics.OverlapSphere(transform.position, 150, individusPerReproduirseMask);
        for (int i = 0; i < individusEnElRadiDeVista.Length; i++)
        {
            Transform _individu = individusEnElRadiDeVista[i].transform;

            Individu j = _individu.GetComponent<Individu>();

            if(j == null || j.especieID != individu.especieID || j.estat != EstatIndividu.BuscantParella || j.genoma.genere == individu.genoma.genere || individu.embarassada || j.embarassada)
            {
                continue;
            }

            individusPerReproduirseVistos.Add(_individu);
        }
    }

    //No sé què és això. Et tinc clissat, nenet...
    public Vector3 DirFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
