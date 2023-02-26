using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

//Tots els estats (Coses que poden fer) els individus
public enum EstatIndividu { Normal, Dormint, BuscantMenjar, Menjant, Caçant, BuscantAigua, Bevent, BuscantParella, Copulant, DonantALlum, Mort, SentMenjat}

//La classe que gestiona el comportament dels individus
public class Individu : MonoBehaviour
{
    #region Variables
    [Header("Especie")]
    public int especieID;
    public ParametresEspecie especie;
    public bool personalitzat;
    [SerializeField] bool diferenciaMascleFemella;

    [Header("Genoma i ADN")]
    public Genoma genoma;

    [Header("Estat")]
    public EstatIndividu estat;

    [Header("Vida i paràmetres")]
    public float gana;
    public float set;
    public float ganesDeReproduirse;

    float probabilitatPercentatgeActual;

    [Header("Desenvolupament")]
    public bool adult;
    public float percentatgeDesenvolupament;
    Vector3 tamanyAdult;

    [Header("Virus")]
    public bool infectat;
    public GameObject particulesVirus;

    [SerializeField] float multiplicadorDistanciaVirus;
    [SerializeField] float probabilitatInfeccioVirus;

    public float procesMalaltia;

    [Header("Embaràs i fills")]
    public bool embarassada;
    public float procesEmbaras;

    [SerializeField] GameObject fillPrefab;

    [Header("Components")]
    [SerializeField] UllsIndividu ulls;
    public Animator anim;
    [SerializeField] Transform model;
    public NavMeshAgent agent;
    public Renderer[] _renderers;
    [SerializeField] Texture[] textures;
    [SerializeField] Texture textureDefecte;
    [SerializeField] ParticleSystem particulesCopular;
    [SerializeField] GameObject particulesMort;
    GameManager gameMana;

    [Header("Mascle-femella")]
    [SerializeField] Animator mascleAnim;
    [SerializeField] Animator femellaAnim;
    [SerializeField] Transform mascleModel;
    [SerializeField] Transform femellaModel;
    [SerializeField] Renderer[] mascleRenderers;
    [SerializeField] Renderer[] femellaRenderers;

    [Header("Altres")]
    float tempsPelProximPuntAtzaros;
    float tempsPelProximPuntAtzarosBuscarMenjar;

    float elapsed;

    GameObject menjarActual;
    [HideInInspector] public Individu parellaActual;
    [HideInInspector] public bool preparatPerCopular;
    [HideInInspector] public bool posicioAtzarSpawn;

    #endregion

    IEnumerator Start()
    {
        gameMana = GameManager.instance;

        Random.InitState(gameMana.info.randomSeed);

        especie = (personalitzat) ? gameMana.especiesPersonalitzades[especieID] : gameMana.especiesNormals[especieID];
        gameMana.RegistrarIndividu(this);

        if (diferenciaMascleFemella)
        {
            bool mascle = genoma.genere == Genere.Masculí;
            anim = (mascle) ? mascleAnim : femellaAnim;
            model = (mascle) ? mascleModel : femellaModel;
            _renderers = (mascle) ? mascleRenderers : femellaRenderers;

            model.gameObject.SetActive(true);
        }

        foreach (Renderer _renderer in _renderers)
        {
            _renderer.material.SetFloat("_EmissionAmount", 0);
        }

        tamanyAdult = model.localScale;

        model.localScale = 0.05f * tamanyAdult;

        foreach (Renderer _renderer in _renderers)
        {
            CanviarColor(genoma.gens[0].gen, _renderer);
        }

        multiplicadorDistanciaVirus = -0.5f * genoma.gens[0].gen + 0.5f;
        probabilitatInfeccioVirus = diferenciaMascleFemella ? 0.01f * gameMana.info.infeccioHumans : especie.probabilitatInfeccio;

        agent.speed = especie.velocitat;

        yield return null;

        if (posicioAtzarSpawn)
        {
            agent.Warp(TrobarPuntAtzar(Vector3.zero, 100, NavMesh.GetAreaFromName("Sorra")));
        }

        float r = Random.Range(0f, 0.5f);
        InvokeRepeating("ActualitzarEstat", r, 0.5f);
    }

    void Update()
    {
        elapsed = gameMana.elapsed;

        anim.SetFloat("Velocitat", agent.velocity.sqrMagnitude);
    }

    void ActualitzarEstat()
    {
        if (!gameMana.comencat)
        {
            agent.ResetPath();

            return;
        }

        if (gameMana.arribatTempsMaxim)
        {
            agent.ResetPath();
            anim.speed = 0;

            return;
        }

        if(estat == EstatIndividu.SentMenjat)
        {
            anim.speed = 0;

            agent.ResetPath();
            return;
        }

        if (estat == EstatIndividu.Mort)
        {
            gameObject.layer = LayerMask.NameToLayer("Cadaver");

            if(gameMana.individuSeleccionat == this)
            {
                gameMana.individuSeleccionat = null;
                Canvas.instance.SeleccionarIndividu(null);
            }

            gameMana.DesregistrarIndividu(this);

            if (infectat)
            {
                gameMana.DesregistrarInfectat(this);
            }

            particulesMort.transform.SetParent(null);
            particulesMort.SetActive(true);
            Destroy(particulesMort, 3);

            Destroy(gameObject);

            return;
        }

        procesMalaltia = (infectat) ? procesMalaltia + (0.25f*genoma.gens[0].gen*genoma.gens[0].gen - 0.5f*genoma.gens[0].gen + 0.25f) : 0;

        if(procesMalaltia >= 100 && estat != EstatIndividu.Copulant && estat != EstatIndividu.DonantALlum && estat != EstatIndividu.Menjant && estat != EstatIndividu.SentMenjat)
        {
            agent.ResetPath();
            estat = EstatIndividu.Mort;

            return;
        }

        agent.speed = especie.velocitat * percentatgeDesenvolupament;

        gana += (estat != EstatIndividu.Menjant) ? especie.tempsAfamacio : 0;
        gana = Mathf.Clamp(gana, 0, 100);

        //set += (estat != EstatIndividu.Bevent) ? especie.tempsAssedegament : 0;
        //set = Mathf.Clamp(set, 0, 100);

        ganesDeReproduirse += (estat != EstatIndividu.Copulant && !embarassada && adult) ? especie.tempsGanesReproduccio : 0;
        ganesDeReproduirse = Mathf.Clamp(ganesDeReproduirse, 0, 100);

        if(gana >= 100 || set >= 100)
        {
            agent.ResetPath();
            estat = EstatIndividu.Mort;

            return;
        }

        percentatgeDesenvolupament += (adult) ? 0 : especie.tempsDesenvolupamentCries;
        model.localScale = percentatgeDesenvolupament * tamanyAdult;

        if(percentatgeDesenvolupament >= 1)
        {
            adult = true;
            model.localScale = tamanyAdult;
        }

        procesEmbaras += (embarassada) ? 1 : 0;

        if (estat != EstatIndividu.Menjant && estat != EstatIndividu.Bevent && estat != EstatIndividu.Copulant)
        {
            if (procesEmbaras >= especie.tempsGestacio)
            {
                estat = EstatIndividu.DonantALlum;
            }
            else
            {
                float prioritat = Mathf.Max(gana, set);
                if (prioritat >= 50)
                {
                    if (prioritat == gana) { estat = EstatIndividu.BuscantMenjar; }
                    else if (prioritat == set) { estat = EstatIndividu.BuscantAigua; }
                }
                else
                {
                    if (ganesDeReproduirse >= 50) { estat = EstatIndividu.BuscantParella; }
                    else { estat = EstatIndividu.Normal; }
                }
            }
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
        if (distanciaALaDestinacio <= Mathf.Pow(2f * agent.stoppingDistance, 2))
        {
            menjarActual = menjar.gameObject;
            estat = EstatIndividu.Menjant;

            anim.SetBool("Menjant", true);
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

        anim.SetBool("Menjant", false);
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

                particulesCopular.Play();

                transform.LookAt(ind.position);
                parellaActual.transform.LookAt(transform.position);
            }
        }
    }

    void Actuar_Copulant()
    {
        anim.SetBool("Copulant", true);

        if(parellaActual != null)
        {
            transform.LookAt(parellaActual.transform.position);
        }

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

        particulesCopular.Stop();

        anim.SetBool("Copulant", false);
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
                Genoma nouGenoma = gameMana.adn.Meiosi(genoma, parellaActual.genoma, gameMana.info.probabilitatMutacio);
                GameObject fill = Instantiate(fillPrefab, transform.position, Quaternion.identity, transform.parent);
                fill.name = (personalitzat) ? "EspeciePersonalitzada_" + especieID.ToString() : "Especie_" + especieID.ToString();

                Individu nouIndividu = fill.GetComponent<Individu>();
                nouIndividu.posicioAtzarSpawn = false;
                nouIndividu.personalitzat = personalitzat;
                nouIndividu.especieID = especieID;
                nouIndividu.genoma = nouGenoma;

                nouIndividu.gana = 0;
                nouIndividu.set = 0;
                nouIndividu.ganesDeReproduirse = 0;

                nouIndividu.infectat = false;
                nouIndividu.adult = false;
                nouIndividu.particulesVirus.SetActive(false);
                nouIndividu.percentatgeDesenvolupament = especie.tempsDesenvolupamentCries;
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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
        new Gen("Salut", 0, ExclusivitatGen.Ninguna)
        };
    }

    public void PosarColorSeleccionat(float color)
    {
        foreach (Renderer _renderer in _renderers)
        {
            _renderer.material.SetFloat("_EmissionAmount", color);
        }
    }

    public static Vector3 TrobarPuntAtzar(Vector3 origen, float radi, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radi;

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
            float dist = (t.transform.position - posicio).sqrMagnitude;
            if (dist < distanciaMinima)
            {
                obj = t;
                distanciaMinima = dist;
            }
        }
        return obj;
    }

    void CanviarColor(float valor, Renderer _renderer)
    {
        if (valor == 0)
        {
            _renderer.material.SetTexture("_MainTex", textureDefecte);
            _renderer.material.SetTexture("_SecondTex", textureDefecte);

            _renderer.material.SetFloat("_LerpValue", valor);

            return;
        }

        if (valor >= -1 && valor < -0.6f)
        {
            _renderer.material.SetTexture("_MainTex", textures[0]);
            _renderer.material.SetTexture("_SecondTex", textures[1]);

            float val = 2.5f + 2.5f * valor;

            _renderer.material.SetFloat("_LerpValue", val);
        }
        else if (valor >= -0.6f && valor < -0.2f)
        {
            _renderer.material.SetTexture("_MainTex", textures[1]);
            _renderer.material.SetTexture("_SecondTex", textures[2]);

            float val = 1.5f + 2.5f * valor;

            _renderer.material.SetFloat("_LerpValue", val);
        }
        else if (valor >= -0.2f && valor < 0.2f)
        {
            _renderer.material.SetTexture("_MainTex", textures[2]);
            _renderer.material.SetTexture("_SecondTex", textures[3]);

            float val = 0.5f + 1.5f * valor;

            _renderer.material.SetFloat("_LerpValue", val);
        }
        else if (valor >= 0.2f && valor < 0.6f)
        {
            _renderer.material.SetTexture("_MainTex", textures[3]);
            _renderer.material.SetTexture("_SecondTex", textures[4]);

            float val = -0.5f + 2.5f * valor;

            _renderer.material.SetFloat("_LerpValue", val);
        }
        else if (valor >= 0.6f)
        {
            _renderer.material.SetTexture("_MainTex", textures[4]);
            _renderer.material.SetTexture("_SecondTex", textures[5]);

            float val = -1.5f + 2.5f * valor;

            _renderer.material.SetFloat("_LerpValue", val);
        }
    }

    public void Infectar()
    {
        if (gameMana.arribatTempsMaxim)
        {
            return;
        }

        infectat = true;

        particulesVirus.SetActive(true);

        gameMana.RegistrarInfectat(this);

        GetComponentInChildren<IndividuAnimEvents>().SorollInfectat();
    }

    void OnTriggerEnter(Collider col)
    {
        if(infectat || !adult)
        {
            return;
        }

        if (gameMana.arribatTempsMaxim)
        {
            return;
        }

        if (col.CompareTag("VirusPersona"))
        {
            probabilitatPercentatgeActual = 0;
            return;
        }

        if (!col.CompareTag("Virus"))
        {
            return;
        }

        probabilitatPercentatgeActual = Random.value;
    }

    void OnTriggerStay(Collider col)
    {
        if (infectat || !adult)
        {
            return;
        }

        if (gameMana.arribatTempsMaxim)
        {
            return;
        }

        if (!col.CompareTag("Virus"))
        {
            return;
        }

        if(probabilitatPercentatgeActual > probabilitatInfeccioVirus)
        {
            return;
        }

        Vector3 posicioVirus = col.transform.position;
        float distanciaVirus = (transform.position - posicioVirus).sqrMagnitude;

        distanciaVirus = Mathf.Clamp(distanciaVirus, 0, 6.25f);

        if(distanciaVirus > Mathf.Pow(5f * multiplicadorDistanciaVirus, 2))
        {
            return;
        }

        Infectar();
    }

    #endregion
}
