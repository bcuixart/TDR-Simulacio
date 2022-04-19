using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Tots els estats (Coses que poden fer) els individus
public enum EstatIndividu { Normal, Dormint, BuscantMenjar, Menjant, Caçant, BuscantAigua, Bevent, BuscantParella, Copulant, DonantALlum, Mort}

//La classe que gestiona el comportament dels individus
public class Individu : MonoBehaviour
{
    #region Variables
    [Header("Especie")]
    public int especieID;
    public ParametresEspecie especie;
    public bool personalitzat;

    [Header("Genoma i ADN")]
    public Genoma genoma;

    [Header("Estat")]
    public EstatIndividu estat;

    [Header("Vida i paràmetres")]
    public float gana;
    public float set;
    public float ganesDeReproduirse;

    [Header("Embaràs i fills")]
    public bool embarassada;
    [SerializeField] float procesEmbaras;

    [SerializeField] GameObject fillPrefab;

    [Header("Components")]
    [SerializeField] UllsIndividu ulls;
    [SerializeField] NavMeshAgent agent;
    public Renderer _renderer;
    GameManager gameMana;

    [Header("Altres")]
    float tempsPelProximPuntAtzaros;
    float tempsPelProximPuntAtzarosBuscarMenjar;

    float elapsed;

    GameObject menjarActual;
    [HideInInspector] public Individu parellaActual;
    [HideInInspector] public bool preparatPerCopular;
    [HideInInspector] public bool posicioAtzarSpawn;

    #endregion

    void Start()
    {
        gameMana = GameManager.instance;

        especie = (personalitzat) ? gameMana.especiesPersonalitzades[especieID] : gameMana.especiesNormals[especieID];
        gameMana.RegistrarIndividu(this);

        if (posicioAtzarSpawn)
        {
            agent.Warp(TrobarPuntAtzar(Vector3.zero, 100, -1));
        }

        InvokeRepeating("ActualitzarEstat", 0, 0.5f);

        agent.speed = especie.velocitatCaminarBase + especie.velocitatCaminarVariacio * genoma.gens[0].gen;
    }

    void Update()
    {
        elapsed = gameMana.elapsed;
    }

    void ActualitzarEstat()
    {
        if (!gameMana.comencat)
        {
            return;
        }

        if(estat == EstatIndividu.Mort)
        {
            gameObject.layer = LayerMask.NameToLayer("Cadaver");

            if(gameMana.individuSeleccionat == this)
            {
                gameMana.individuSeleccionat = null;
                Canvas.instance.SeleccionarIndividu(null);
            }

            gameMana.DesregistrarIndividu(this);

            Destroy(gameObject);

            return;
        }

        gana += (estat != EstatIndividu.Menjant) ? especie.tempsAfamacioBase + especie.tempsAfamacioVariacio * genoma.gens[0].gen : 0;
        gana = Mathf.Clamp(gana, 0, 100);

        //set += (estat != EstatIndividu.Bevent) ? especie.tempsAssedegamentBase + especie.tempsAssedegamentVariacio * genoma.gens[0].gen : 0;
        set = Mathf.Clamp(set, 0, 100);

        ganesDeReproduirse += (estat != EstatIndividu.Copulant && !embarassada) ? especie.tempsGanesReproduccioBase + especie.tempsGanesReproduccioVariacio * genoma.gens[2].gen : 0;
        ganesDeReproduirse = Mathf.Clamp(ganesDeReproduirse, 0, 100);

        if(gana >= 100 || set >= 100)
        {
            agent.ResetPath();
            estat = EstatIndividu.Mort;

            return;
        }

        if (estat != EstatIndividu.Menjant && estat != EstatIndividu.Bevent && estat != EstatIndividu.Copulant)
        {
            float prioritat = Mathf.Max(gana, set, ganesDeReproduirse);
            if (prioritat >= 50)
            {
                if (prioritat == gana)
                {
                    estat = EstatIndividu.BuscantMenjar;
                }
                else if (prioritat == set)
                {
                    estat = EstatIndividu.BuscantAigua;
                }
                else if (prioritat == ganesDeReproduirse)
                {
                    estat = EstatIndividu.BuscantParella;
                }
            }
            else
            {
                estat = EstatIndividu.Normal;
            }
        }

        procesEmbaras += (embarassada) ? 1 : 0;
        if (procesEmbaras >= especie.tempsGestacioBase + especie.tempsGestacioVariacio * genoma.gens[4].gen)
        {
            estat = EstatIndividu.DonantALlum;
        }

        switch (estat)
        {
            case EstatIndividu.Normal:
                Actuar_Normal();
                break;
            case EstatIndividu.Dormint:
                break;
            case EstatIndividu.BuscantMenjar:
                Actuar_BuscantMenjar();
                break;
            case EstatIndividu.Menjant:
                Actuar_Menjant();
                break;
            case EstatIndividu.Caçant:
                break;
            case EstatIndividu.BuscantAigua:
                break;
            case EstatIndividu.Bevent:
                break;
            case EstatIndividu.BuscantParella:
                Actuar_BuscantReproduirse();
                break;
            case EstatIndividu.Copulant:
                Actuar_Copulant();
                break;
            case EstatIndividu.DonantALlum:
                Actuar_DonantALlum();
                break;
            case EstatIndividu.Mort:
                break;
        }
    }

    void Actuar_Normal()
    {
        if(elapsed < tempsPelProximPuntAtzaros)
        {
            return;
        }

        tempsPelProximPuntAtzaros = elapsed + 10 * Random.Range(0.5f, 2f);

        Vector3 puntAtzar = TrobarPuntAtzar(transform.position, 30, -1);
        agent.SetDestination(puntAtzar);
    }

    void Actuar_BuscantMenjar()
    {
        ulls.VeureMenjar();

        if(ulls.menjarVist.Count == 0)
        {
            if (elapsed >= tempsPelProximPuntAtzarosBuscarMenjar)
            {
                Vector3 puntAtzar = TrobarPuntAtzar(transform.position, 100, -1);
                agent.SetDestination(puntAtzar);

                tempsPelProximPuntAtzarosBuscarMenjar = elapsed + 10 * Random.Range(0.5f, 1);
            }

            return;
        }

        Transform menjar = TrobarObjecteMesProper(ulls.menjarVist.ToArray());
        agent.SetDestination(menjar.position);

        float distanciaALaDestinacio = (transform.position - agent.destination).sqrMagnitude;
        if (distanciaALaDestinacio <= Mathf.Pow(1.5f * agent.stoppingDistance, 2))
        {
            menjarActual = menjar.gameObject;
            estat = EstatIndividu.Menjant;
        }
    }

    void Actuar_Menjant()
    {
        if (menjarActual != null)
        {
            Consumible consumible = menjarActual.GetComponent<Consumible>();
            if (consumible != null && menjarActual != null)
            {
                gana -= consumible.SerMenjat(1);

                return;
            }

            menjarActual = null;
        }

        estat = EstatIndividu.Normal;
    }

    void Actuar_BuscantReproduirse()
    {
        preparatPerCopular = false;

        ulls.VeureIndividusPerReproduirse();

        if (ulls.individusPerReproduirseVistos.Count == 0)
        {
            parellaActual = null;

            if (elapsed >= tempsPelProximPuntAtzarosBuscarMenjar)
            {
                Vector3 puntAtzar = TrobarPuntAtzar(transform.position, 100, -1);
                agent.SetDestination(puntAtzar);

                tempsPelProximPuntAtzarosBuscarMenjar = elapsed + 10 * Random.Range(0.5f, 1);
            }

            return;
        }

        Transform ind = TrobarObjecteMesProper(ulls.individusPerReproduirseVistos.ToArray());
        agent.SetDestination(ind.position);

        parellaActual = ind.GetComponent<Individu>();

        float distanciaALaDestinacio = (transform.position - agent.destination).sqrMagnitude;
        if (distanciaALaDestinacio <= Mathf.Pow(2f * agent.stoppingDistance, 2))
        {
            preparatPerCopular = true;
            transform.LookAt(ind.position);

            if (!parellaActual.preparatPerCopular)
            {
                parellaActual.EnviarSenyalAparellament(this);

                return;
            }

            if(parellaActual.parellaActual == this)
            {
                estat = EstatIndividu.Copulant;
                parellaActual.estat = EstatIndividu.Copulant;

                transform.LookAt(ind.position);
                parellaActual.transform.LookAt(transform.position);
            }
        }
    }

    void Actuar_Copulant()
    {
        agent.ResetPath();

        if (preparatPerCopular)
        {
            ganesDeReproduirse = 50;
            preparatPerCopular = false;
        }

        ganesDeReproduirse -= 5;

        if(ganesDeReproduirse > 0)
        {
            return;
        }

        if(genoma.genere == Genere.Femení)
        {
            procesEmbaras = 0;
            embarassada = true;
        }

        estat = EstatIndividu.Normal;
    }

    void Actuar_DonantALlum()
    {
        agent.ResetPath();

        if (procesEmbaras > 0)
        {
            procesEmbaras = 0;
            embarassada = false;

            int fills = Random.Range(especie.fillsMinims, especie.fillsMaxims + 1);

            for (int i = 0; i < fills; i++)
            {
                Genoma nouGenoma = ADN.Meiosi(genoma, parellaActual.genoma);
                GameObject fill = Instantiate(fillPrefab, transform.position, Quaternion.identity, transform.parent);
                fill.name = (personalitzat) ? "EspeciePersonalitzada_" + especieID.ToString() : "Especie_" + especieID.ToString();

                Individu nouIndividu = fill.GetComponent<Individu>();
                nouIndividu.personalitzat = personalitzat;
                nouIndividu.especieID = especieID;
                nouIndividu.genoma = nouGenoma;
            }
        }
        else
        {
            estat = EstatIndividu.Normal;
        }
    }

    #region Altres
    public void EnviarSenyalAparellament(Individu pretendent)
    {
        transform.LookAt(pretendent.transform.position);
        Actuar_BuscantReproduirse();
    }

    void OnMouseDown()
    {
        if(gameMana.individuSeleccionat != null)
        {
            gameMana.individuSeleccionat.PosarColorSeleccionat(0);
        }

        gameMana.individuSeleccionat = this;
        Canvas.instance.SeleccionarIndividu(this);

        PosarColorSeleccionat(1);
    }

    //Això és una funció que utilitzo en el Unity i no és important.
    [ContextMenu("Posar genoma base")]
    void PosarGenomaBase()
    {
        genoma.gens = new List<Gen>
        {
        new Gen("Velocitat", 0, ExclusivitatGen.Ninguna),
        new Gen("Detecció", 0, ExclusivitatGen.Ninguna),
        new Gen("Ànsia reproductiva", 0, ExclusivitatGen.Ninguna),
        new Gen("Color", 0, ExclusivitatGen.Ninguna),
        new Gen("Atractiu", 0, ExclusivitatGen.Masculí),
        new Gen("Gestació", 0, ExclusivitatGen.Femení),
        };
    }

    public void PosarColorSeleccionat(float color)
    {
        _renderer.material.SetFloat("_EmissionAmount", color);
    }

    public static Vector3 TrobarPuntAtzar(Vector3 origen, float radi, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radi;

        randomDirection += origen;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, radi, layermask);

        return navHit.position;
    }

    Transform TrobarObjecteMesProper(Transform[] objectes)
    {
        Transform obj = null;

        float distanciaMinima = Mathf.Infinity;
        Vector3 posicio = transform.position;

        foreach (Transform t in objectes)
        {
            float dist = Vector3.Distance(t.transform.position, posicio);
            if (dist < distanciaMinima)
            {
                obj = t;
                distanciaMinima = dist;
            }
        }
        return obj;
    }
    #endregion
}
