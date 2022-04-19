using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UllsIndividu : MonoBehaviour
{
    [SerializeField] Individu individu;
    //public Transform cap;

    public float distanciaDeVista;
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

        distanciaDeVista = individu.especie.deteccioBase + individu.especie.deteccioVariacio * individu.genoma.gens[1].gen;
    }

    public void VeureMenjar()
    {
        menjarVist.Clear();

        Collider[] menjarEnElRadiDeVista = Physics.OverlapSphere(transform.position, distanciaDeVista, menjarMask);
        for (int i = 0; i < menjarEnElRadiDeVista.Length; i++)
        {
            Transform menjar = menjarEnElRadiDeVista[i].transform;

            if (menjar.gameObject.layer == LayerMask.NameToLayer("Animal"))
            {
                string nomMenjar = menjar.name;
                nomMenjar = nomMenjar.Replace("Especie_", "");
                nomMenjar = nomMenjar.Replace("EspeciePersonalitzada_", "");
                int menjarID = int.Parse(nomMenjar);

                if (menjar == transform || !individu.especie.dietaPrimaria.Contains(menjarID))
                {
                    continue;
                }
            }

            Vector3 posicioMenjar = new Vector3(menjar.position.x, transform.position.y, menjar.position.z);

            Vector3 direccioAlMenjar = (posicioMenjar - transform.position).normalized;

            if (Vector3.Angle(transform.forward, direccioAlMenjar) < angleDeVista / 2)
            {
                float dst = Vector3.Distance(transform.position, posicioMenjar);

                if (!Physics.Raycast(transform.position, direccioAlMenjar, dst, obstacleMask))
                {
                    menjarVist.Add(menjar);
                }
            }
        }
    }

    public void VeureIndividusPerReproduirse()
    {
        individusPerReproduirseVistos.Clear();

        Collider[] individusEnElRadiDeVista = Physics.OverlapSphere(transform.position, distanciaDeVista, individusPerReproduirseMask);
        for (int i = 0; i < individusEnElRadiDeVista.Length; i++)
        {
            Transform _individu = individusEnElRadiDeVista[i].transform;

            Individu j = _individu.GetComponent<Individu>();

            if(j == null || j.especieID != individu.especieID || j.estat != EstatIndividu.BuscantParella || j.genoma.genere == individu.genoma.genere || individu.embarassada || j.embarassada)
            {
                //Debug.Log("He trobat algu que no me toca");
                continue;
            }

            Vector3 posicioIndividu = new Vector3(_individu.position.x, transform.position.y, _individu.position.z);

            Vector3 direccioAlIndividu = (posicioIndividu - transform.position).normalized;

            if (Vector3.Angle(transform.forward, direccioAlIndividu) < angleDeVista / 2)
            {
                float dst = Vector3.Distance(transform.position, posicioIndividu);

                if (!Physics.Raycast(transform.position, direccioAlIndividu, dst, obstacleMask))
                {
                    individusPerReproduirseVistos.Add(_individu);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angle, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
