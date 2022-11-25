using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Informacio")]
    public InformacioSimulacio info;

    [Header("ADN")]
    public ADN adn;

    [Header("Llista d'individus")]
    public List<Individus> individusNormals;
    public List<Individus> individusPersonalitzats;
    public List<Planta> plantes = new List<Planta>();
    public int maximPlantes = 100;

    Individu primerIndividuPerInfectar;

    [Header("Especies")]
    public List<ParametresEspecie> especiesNormals = new List<ParametresEspecie>();
    public List<ParametresEspecie> especiesPersonalitzades = new List<ParametresEspecie>();

    [Header("Virus")]
    [SerializeField] GameObject virusPosicioFixa;

    [Header("Temps")]
    public bool pausat;
    public bool arribatTempsMaxim;
    public int velocitat;
    [SerializeField] float velocitatTemps;
    public float elapsed;
    [SerializeField] Animator solAnim;
    [SerializeField] GameObject eventMeteorit;
    [SerializeField] Material celNormal;
    [SerializeField] Material celMeteorit;

    [Header("Prefabs individus")]
    public List<GameObject> individusPrefabs;
    public GameObject individuPersonalitzatPrefab;
    [SerializeField] GameObject plantaPrefab;
    [SerializeField] Transform plantaPrefabBase;
    
    [HideInInspector] public Individu individuSeleccionat;
    public bool comencat = false;
    public bool meteorit;
    [SerializeField] bool optimitzacioDesesperada;

    DadesManager dades;

    public static float probabilitatSalt;

    void Awake()
    {
        virusPosicioFixa.SetActive(false);

        if (InformacioSimulacio.instance != null)
        {
            info = InformacioSimulacio.instance;
        }

        if (!info.finalitzada)
        {
            int s = Random.Range(0, 101);
            info.randomSeed = s;

            info.dataCreacio = System.DateTime.Now.ToString();
        }
        else
        {
            info.tempsMaxim = info.tempsTotal;
        }

        Random.InitState(info.randomSeed);

        adn = new ADN(info.randomSeed);

        instance = this;
        comencat = false;

        velocitatTemps = 0;
    }

    void Start()
    {
        StartCoroutine(Comencacio());
    }

    public IEnumerator Comencacio()
    {
        dades = DadesManager.instance;

        string[] files = Directory.GetFiles(Application.dataPath + "/Especies");
        foreach (string file in files)
        {
            if (!file.EndsWith(".json"))
            {
                continue;
            }

            using (StreamReader stream = new StreamReader(file))
            {
                string fileData = stream.ReadToEnd();

                ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

                ParametresEspecie pm = parametres;
                int id = parametres.id;

                especiesNormals.Add(pm);

                dades.dades.especiesNormals.Add(new DadesEspecie(id, parametres.nomSingular));
                dades.nombreIndividusNormals.Add(0);
                dades.nombreIndividusNormalsInfectats.Add(0);

                stream.Close();

                GameObject canGO = Instantiate(Canvas.instance.dadesLlistaEspeciesPrefab, Canvas.instance.dadesLlistaEspeciesNormalBase);
                canGO.GetComponent<Button>().onClick.AddListener(delegate { Canvas.instance.ObrirDadesIndividu(id, false); });
                canGO.GetComponentInChildren<Text>().text = pm.nomPlural;
            }
        }

        yield return null;

        string[] filesCustom = Directory.GetFiles(Application.dataPath + "/StreamingAssets");
        foreach (string file in filesCustom)
        {
            if (!file.EndsWith(".json"))
            {
                continue;
            }

            using (StreamReader stream = new StreamReader(file))
            {
                string fileData = stream.ReadToEnd();

                ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

                ParametresEspecie pm = parametres;
                int id = parametres.id;

                especiesPersonalitzades.Add(pm);

                dades.dades.especiesPersonalitzades.Add(new DadesEspecie(id, parametres.nomSingular));
                dades.nombreIndividusPersonalitzats.Add(0);
                dades.nombreIndividusPersonalitzatsInfectats.Add(0);

                stream.Close();

                GameObject canGO = Instantiate(Canvas.instance.dadesLlistaEspeciesPrefab, Canvas.instance.dadesLlistaEspeciesPersonalitzadesBase);
                canGO.GetComponent<Button>().onClick.AddListener(delegate { Canvas.instance.ObrirDadesIndividu(id, true); });
                canGO.GetComponentInChildren<Text>().text = pm.nomPlural;
            }
        }

        yield return null;

        for (int i = 0; i < info.individusPerFerApareixerNormal.Count; i++)
        {
            for (int j = 0; j < info.individusPerFerApareixerNormal[i]; j++)
            {
                GameObject GO = Instantiate(individusPrefabs[i]);
                Individu ind = GO.GetComponent<Individu>();
                ind.especieID = i;
                ind.posicioAtzarSpawn = true;
                GO.name = "Especie_" + i.ToString();

                ind.genoma.genere = (j > ((info.individusPerFerApareixerNormal[i] - 1) / 2)) ? Genere.Femení : Genere.Masculí;

                if(primerIndividuPerInfectar == null && i != 0)
                {
                    primerIndividuPerInfectar = ind;
                }
            }

            yield return null;
        }

        yield return null;

        for (int i = 0; i < info.individusPerFerApareixerPersonalitzat.Count; i++)
        {
            for (int j = 0; j < info.individusPerFerApareixerPersonalitzat[i]; j++)
            {
                GameObject GO = Instantiate(individuPersonalitzatPrefab, Vector3.zero, Quaternion.identity);
                Individu ind = GO.GetComponent<Individu>();
                ind.especieID = i;
                ind.personalitzat = true;
                ind.posicioAtzarSpawn = true;
                GO.name = "EspeciePersonalitzada_" + i.ToString();

                ind.genoma.genere = (j > ((info.individusPerFerApareixerPersonalitzat[i] - 1) / 2)) ? Genere.Femení : Genere.Masculí;
            }

            yield return null;
        }

        comencat = true;

        velocitat = 1;
        velocitatTemps = 1;
        Time.timeScale = 1;

        AfegirNotificacio(new Notificacio(TipusNotificacio.IniciSimulacio, 0, "", 0));

        if (info.tempsMaxim != -1)
        {
            Invoke("TempsMaxim", info.tempsMaxim);
        }

        InvokeRepeating("IntentFerApareixerPlanta", 15, 15);

        yield return new WaitForSeconds(30);

        if(primerIndividuPerInfectar != null)
        {
            primerIndividuPerInfectar.Infectar();
        }

        yield return null;

        virusPosicioFixa.SetActive(true);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.P))
        {
            Menu.devMode = true;
            Canvas.instance.botoDevModeRepeticio.SetActive(true);
            Canvas.instance.devModeIndicador.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (!comencat || meteorit)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pausa(!pausat);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Canvas.instance.pausaFons.activeSelf)
            {
                Pausa(false);
            }
            else
            {
                Canvas.instance.pausaFons.SetActive(true);
                Pausa(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PosarVelocitat(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PosarVelocitat(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PosarVelocitat(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PosarVelocitat(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PosarVelocitat(5);
        }

        if (Input.GetKeyDown(KeyCode.I) && Menu.devMode)
        {
            if(individuSeleccionat == null)
            {
                return;
            }

            individuSeleccionat.particulesVirus.SetActive(true);
            individuSeleccionat.infectat = true;

            RegistrarInfectat(individuSeleccionat);
        }

        if (Input.GetKeyDown(KeyCode.O) && Menu.devMode)
        {
            OptimitzacioDesesperada();
        }

        if (arribatTempsMaxim)
        {
            return;
        }

        elapsed += Time.deltaTime;
    }

    public void PosarVelocitat(int vel)
    {
        velocitat = vel;

        switch (vel)
        {
            case 1:
                velocitatTemps = 1;
                break;
            case 2:
                velocitatTemps = 5;
                break;
            case 3:
                velocitatTemps = 10;
                break;
            case 4:
                velocitatTemps = 30;
                break;
            case 5:
                velocitatTemps = 60;
                break;
        }

        Pausa(false);

        Canvas.instance.PosarVelocitat();
    }

    public void RegistrarIndividu(Individu individu)
    {
        if (individu.personalitzat)
        {
            individusPersonalitzats[individu.especieID].individus.Add(individu);

            dades.nombreIndividusPersonalitzats[individu.especieID]++;

            return;
        }

        individusNormals[individu.especieID].individus.Add(individu);

        dades.nombreIndividusNormals[individu.especieID]++;
    }

    public void DesregistrarIndividu(Individu individu)
    {
        if (individu.personalitzat)
        {
            individusPersonalitzats[individu.especieID].individus.Remove(individu);

            dades.nombreIndividusPersonalitzats[individu.especieID]--;

            if(dades.nombreIndividusPersonalitzats[individu.especieID] == 0)
            {
                AfegirNotificacio(new Notificacio(TipusNotificacio.Extincio, elapsed, individu.especie.nomPlural.ToLower(), (int)individu.especie.articleGenere));
            }

            return;
        }

        individusNormals[individu.especieID].individus.Remove(individu);

        dades.nombreIndividusNormals[individu.especieID]--;

        if (dades.nombreIndividusNormals[individu.especieID] == 0)
        {
            AfegirNotificacio(new Notificacio(TipusNotificacio.Extincio, elapsed, individu.especie.nomPlural.ToLower(), (int)individu.especie.articleGenere));
        }
    }

    public void RegistrarInfectat(Individu individu)
    {
        if (individu.personalitzat)
        {
            dades.nombreIndividusPersonalitzatsInfectats[individu.especieID]++;

            if(dades.nombreIndividusPersonalitzatsInfectats[individu.especieID] == 1)
            {
                AfegirNotificacio(new Notificacio(TipusNotificacio.PrimerInfectat, elapsed, individu.especie.nomSingular, (int) individu.especie.articleGenere));
            }

            return;
        }

        dades.nombreIndividusNormalsInfectats[individu.especieID]++;

        if (dades.nombreIndividusNormalsInfectats[individu.especieID] == 1)
        {
            AfegirNotificacio(new Notificacio(TipusNotificacio.PrimerInfectat, elapsed, individu.especie.nomSingular.ToLower(), (int)individu.especie.articleGenere));
        }
    }

    public void DesregistrarInfectat(Individu individu)
    {
        if (individu.personalitzat)
        {
            dades.nombreIndividusPersonalitzatsInfectats[individu.especieID]--;

            return;
        }

        dades.nombreIndividusNormalsInfectats[individu.especieID]--;
    }

    public void IntentFerApareixerPlanta()
    {
        int probabilitat = Random.Range(1, maximPlantes + 1);
        if(probabilitat <= plantes.Count)
        {
            return;
        }

        GameObject GO = Instantiate(plantaPrefab, plantaPrefabBase);
        Planta p = GO.GetComponent<Planta>();

        GO.name = plantaPrefab.name;
        p.posicioAtzarSpawn = true;
    }

    public void TempsMaxim()
    {
        arribatTempsMaxim = true;

        AfegirNotificacio(new Notificacio(TipusNotificacio.FinalSimulacioTempsMaxim, info.tempsMaxim, "", 0));

        solAnim.speed = 0;
    }

    public void Pausa(bool _p)
    {
        pausat = _p;

        Time.timeScale = (pausat) ? 0 : velocitatTemps;

        if (!pausat)
        {
            Canvas.instance.pausaFons.SetActive(false);
        }

        Canvas.instance.PosarPausa();
    }

    public void AfegirNotificacio(Notificacio not)
    {
        info.notificacions.Add(not);

        Canvas.instance.NovaNotificacio(not);
    }

    public void SortirGuardarSimulacio()
    {
        if (string.IsNullOrWhiteSpace(info.ubicacioSimulacio) || info.finalitzada)
        {
            return;
        }

        if (!arribatTempsMaxim)
        {
            AfegirNotificacio(new Notificacio(TipusNotificacio.FinalSimulacioArbitrari, elapsed, "", 0));
        }

        if (!info.finalitzada)
        {
            StartCoroutine(EventMeteorit());
        }

        info.finalitzada = true;
        info.tempsTotal = elapsed;

        info.dades = dades.dades;

        using (StreamWriter stream = new StreamWriter(info.ubicacioSimulacio))
        {
            string s = JsonUtility.ToJson(info, true);

            stream.Write(s);

            stream.Close();
        }

        Time.timeScale = 1;
    }

    void OptimitzacioDesesperada()
    {
        optimitzacioDesesperada = !optimitzacioDesesperada;

        Renderer[] renderers = FindObjectsOfType<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = !optimitzacioDesesperada;
        }
    }

    public IEnumerator EventMeteorit()
    {
        meteorit = true;

        Pausa(false);
        PosarVelocitat(1);

        eventMeteorit.SetActive(true);

        Light l = solAnim.GetComponent<Light>();
        Color c = new Color32(128, 3, 3, 255);
        RenderSettings.skybox = celMeteorit;

        float t = 0;
        while (t < 5)
        {
            t += Time.deltaTime;
            l.color = Color.Lerp(Color.white, c, t/5);
            l.intensity = Mathf.Lerp(0.5f, 1, t/5);

            yield return null;
        }

        for (int i = 0; i < individusNormals.Count; i++)
        {
            for (int j = 0; j < individusNormals[i].individus.Count; j++)
            {
                Destroy(individusNormals[i].individus[j].gameObject);
            }
        }

        Destroy(GameObject.Find("Decoracions"));
        Destroy(GameObject.Find("Plantitas"));

        yield return new WaitForSeconds(10);

        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}

[System.Serializable]
public class Individus
{
    public List<Individu> individus;
}