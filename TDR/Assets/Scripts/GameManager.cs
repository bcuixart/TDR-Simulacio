using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Informacio")]
    public InformacioSimulacio info;

    [Header("Llista d'individus")]
    public List<Individus> individusNormals;
    public List<Individus> individusPersonalitzats;
    public List<Planta> plantes = new List<Planta>();

    [Header("Especies")]
    public List<ParametresEspecie> especiesNormals = new List<ParametresEspecie>();
    public List<ParametresEspecie> especiesPersonalitzades = new List<ParametresEspecie>();

    [Header("Temps")]
    public bool pausat;
    public int velocitat;
    [SerializeField] float velocitatTemps;
    public float elapsed;

    [Header("Prefabs individus")]
    public List<GameObject> individusPrefabs;
    public GameObject individuPersonalitzatPrefab;
    
    [HideInInspector] public Individu individuSeleccionat;
    public bool comencat = false;

    DadesManager dades;

    void Awake()
    {
        instance = this;
        comencat = false;

        velocitatTemps = 0;

        if(InformacioSimulacio.instance != null)
        {
            info = InformacioSimulacio.instance;
        }
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
            if (file.Contains(".meta"))
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
            if (file.Contains("meta"))
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
                GameObject GO = Instantiate(individusPrefabs[i], Vector3.zero, Quaternion.identity);
                Individu ind = GO.GetComponent<Individu>();
                ind.especieID = i;
                ind.posicioAtzarSpawn = true;
                GO.name = "Especie_" + i.ToString();

                ind.genoma.genere = (j > ((info.individusPerFerApareixerNormal[i] - 1) / 2)) ? Genere.Femení : Genere.Masculí;
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
        velocitatTemps = 1;
        Time.timeScale = 1;
    }

    void Update()
    {
        if (!comencat)
        {
            return;
        }

        elapsed += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Pausa(!pausat);
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
        }
        else
        {
            individusNormals[individu.especieID].individus.Add(individu);

            dades.nombreIndividusNormals[individu.especieID]++;
        }
    }

    public void DesregistrarIndividu(Individu individu)
    {
        if (individu.personalitzat)
        {
            individusPersonalitzats[individu.especieID].individus.Remove(individu);

            dades.nombreIndividusPersonalitzats[individu.especieID]--;
        }
        else
        {
            individusNormals[individu.especieID].individus.Remove(individu);

            dades.nombreIndividusNormals[individu.especieID]--;
        }
    }

    public void Pausa(bool _p)
    {
        pausat = _p;

        Time.timeScale = (pausat) ? 0 : velocitatTemps;

        Canvas.instance.PosarPausa();
    }
}

[System.Serializable]
public class Individus
{
    public List<Individu> individus;
}