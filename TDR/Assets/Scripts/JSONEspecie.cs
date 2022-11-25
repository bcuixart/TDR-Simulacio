using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class JSONEspecie : MonoBehaviour
{
    [SerializeField] ParametresEspecie especieSeleccionada;
    [SerializeField] string especieSeleccionadaPath;

    [Space]
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemNovaEspeciePrefab;

    [Space]
    [SerializeField] Transform normalItemBase;
    [SerializeField] Transform personalitzatItemBase;

    [Header("UI")]
    [SerializeField] GameObject llistaArxius;
    [SerializeField] GameObject jsonObert;
    [Space]
    [SerializeField] GameObject llistaArxiusNormal;
    [SerializeField] GameObject llistaArxiusPersonalitzats;
    [Space]
    [SerializeField] Text probabilitatMutacioText;

    #region OH_DIOS
    [Header("JSONObert")]
    [SerializeField] Button saveButton;

    [SerializeField] Text nomText;

    [SerializeField] InputField nomSingular;
    [SerializeField] InputField nomPlural;
    [SerializeField] Toggle articleMasculi;
    [SerializeField] Toggle articleFemeni;

    //AIXO ES HORROROS AAAH

    [SerializeField] Slider velocitatCaminacioSlider;
    [SerializeField] Text velocitatCaminacioText;

    [SerializeField] Slider velocitatCorrerSlider;
    [SerializeField] Text velocitatCorrerText;

    [SerializeField] Slider tempsAfamacioSlider;
    [SerializeField] Text tempsAfamacioText;

    [SerializeField] Slider tempsAssedecacioSlider;
    [SerializeField] Text tempsAssedecacioText;

    [SerializeField] Slider tempsGanesReproduccioSlider;
    [SerializeField] Text tempsGanesReproduccioText;

    [SerializeField] Slider tempsGestacioSlider;
    [SerializeField] Text tempsGestacioText;

    [SerializeField] Slider detecioSlider;
    [SerializeField] Text detecioText;

    [SerializeField] Slider fillsBaseSlider;
    [SerializeField] Slider fillsVariacioSlider;
    [SerializeField] Text fillsBaseText;
    [SerializeField] Text fillsVariacioText;
    #endregion

    List<string> filesNormal = new List<string>();
    List<string> filesPersonalitzat = new List<string>();

    bool potGuardar;
    bool devMode;
    int multiplicadorAfegirEspecies;

    void Start()
    {
        //InformacioSimulacio.instance = new InformacioSimulacio(10f ,100);
        CarregarArxius();
    }

    void CarregarArxius()
    {
        filesNormal.Clear();
        filesPersonalitzat.Clear();

        foreach (Transform child in normalItemBase)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in personalitzatItemBase)
        {
            Destroy(child.gameObject);
        }

        string[] files = Directory.GetFiles(Application.dataPath + "/Especies");
        foreach (string file in files)
        {
            if (!file.EndsWith(".json"))
            {
                continue;
            }

            using (StreamReader stream = new StreamReader(file))
            {
                filesNormal.Add(file);
                string fileData = stream.ReadToEnd();

                ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

                ParametresEspecie pm = parametres;
                int id = parametres.id;

                GameObject GO = Instantiate(itemPrefab, normalItemBase);
                GO.name = id.ToString();
                GO.GetComponentInChildren<Text>().text = id.ToString("000") + " - " + pm.nomPlural;

                GO.GetComponent<Button>().onClick.AddListener(delegate { ObrirArxiu(id, true); });

                Transform t1 = GO.transform.GetChild(3);
                Transform t2 = GO.transform.GetChild(4);
                Text txt = GO.transform.GetChild(2).GetComponent<Text>();

                txt.text = InformacioSimulacio.instance.individusPerFerApareixerNormal[id].ToString();

                t1.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, 1, txt, true); });
                t2.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, -1, txt, true); });

                stream.Close();
            }
        }

        string[] filesCustom = Directory.GetFiles(Application.dataPath + "/StreamingAssets");
        foreach (string file in filesCustom)
        {
            if (!file.EndsWith(".json"))
            {
                continue;
            }

            using (StreamReader stream = new StreamReader(file))
            {
                filesPersonalitzat.Add(file);
                string fileData = stream.ReadToEnd();

                ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

                ParametresEspecie pm = parametres;
                int id = parametres.id;

                GameObject GO = Instantiate(itemPrefab, personalitzatItemBase);
                GO.name = id.ToString();
                GO.GetComponentInChildren<Text>().text = id.ToString("000") + " - " + pm.nomPlural;

                GO.GetComponentInChildren<Button>().onClick.AddListener(delegate { ObrirArxiu(id, false); });

                Transform t1 = GO.transform.GetChild(3);
                Transform t2 = GO.transform.GetChild(4);
                Text txt = GO.transform.GetChild(2).GetComponent<Text>();

                txt.text = InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat[id].ToString();

                t1.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, 1, txt, false); });
                t2.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, -1, txt, false); });

                stream.Close();
            }
        }

        GameObject GOH = Instantiate(itemNovaEspeciePrefab, personalitzatItemBase);
        GOH.GetComponent<Button>().onClick.AddListener(delegate { NovaEspecie(); });
    }

    void Update()
    {
        multiplicadorAfegirEspecies = 1;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            multiplicadorAfegirEspecies += 4;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            multiplicadorAfegirEspecies *= 10;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            devMode = true;
            saveButton.interactable = true;
        }
    }

    public void Comencar()
    {
        SceneManager.LoadScene("Simulacio");
    }

    public void TornarMenuPrincipal()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NovaEspecie()
    {
        StartCoroutine(CrearNovaEspecie());
    }

    IEnumerator CrearNovaEspecie()
    {
        int files = filesPersonalitzat.Count;

        string nom = Application.dataPath + "/StreamingAssets/" + "Especie_" + files.ToString("000") + ".json";

        using (StreamWriter stream = new StreamWriter(nom))
        {
            string s = JsonUtility.ToJson(new ParametresEspecie(files, "Nova espècie", "Nova espècie"), true);

            stream.Write(s);

            stream.Close();
        }

        CarregarArxius();

        yield return null;
    }

    public void Guardar()
    {
        using (StreamWriter stream = new StreamWriter(especieSeleccionadaPath))
        {
            string s = JsonUtility.ToJson(especieSeleccionada, true);

            stream.Write(s);

            stream.Close();
        }
    }

    public void ObrirArxiu(int id, bool normal)
    {
        llistaArxius.SetActive(false);
        jsonObert.SetActive(true);

        potGuardar = !normal;
        saveButton.interactable = (devMode) ? true : !normal;

        using (StreamReader stream = new StreamReader((normal) ? filesNormal[id] : filesPersonalitzat[id]))
        {
            filesPersonalitzat.Add(filesNormal[id]);
            string fileData = stream.ReadToEnd();

            ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

            ParametresEspecie pm = parametres;
            int _id = parametres.id;

            especieSeleccionada = pm;
            especieSeleccionadaPath = (normal) ? filesNormal[id] : filesPersonalitzat[id];

            stream.Close();
        }

        nomText.text = especieSeleccionada.nomPlural;

        nomSingular.text = especieSeleccionada.nomSingular;
        nomPlural.text = especieSeleccionada.nomPlural;

        articleMasculi.isOn = (especieSeleccionada.articleGenere == ArticleGenere.Masculí) ? true : false;
        articleFemeni.isOn = (especieSeleccionada.articleGenere == ArticleGenere.Femení) ? true : false;

        velocitatCaminacioSlider.value = especieSeleccionada.velocitat;
        velocitatCorrerSlider.value = especieSeleccionada.velocitat;

        tempsAfamacioSlider.value = especieSeleccionada.tempsAfamacio;
        tempsAssedecacioSlider.value = especieSeleccionada.tempsAssedegament;
        tempsGanesReproduccioSlider.value = especieSeleccionada.tempsGanesReproduccio;

        tempsGestacioSlider.value = especieSeleccionada.tempsGestacio;

        //detecioSlider.value = especieSeleccionada.deteccio;

        fillsBaseSlider.value = especieSeleccionada.fillsMinims;
        fillsVariacioSlider.value = especieSeleccionada.fillsMaxims;
    }

    #region SiUsPlau_No_Miris_Aixo
    public void NomSinglular(string nom)
    {
        especieSeleccionada.nomSingular = nom;
    }

    public void NomPlural(string nom)
    {
        nomText.text = nom;
        especieSeleccionada.nomPlural = nom;
    }

    public void Masculi(bool on)
    {
        if (on)
        {
            especieSeleccionada.articleGenere = ArticleGenere.Masculí;
        }
    }

    public void Femeni(bool on)
    {
        if (on)
        {
            especieSeleccionada.articleGenere = ArticleGenere.Femení;
        }
    }

    public void VelocitatCaminacio(float valor)
    {
        velocitatCaminacioText.text = valor.ToString();
        especieSeleccionada.velocitat = valor;
    }

    public void VelocitatCorrecio(float valor)
    {
        velocitatCorrerText.text = valor.ToString();
        especieSeleccionada.velocitat = valor;
    }

    public void TempsAfamacio(float valor)
    {
        tempsAfamacioText.text = valor.ToString();
        especieSeleccionada.tempsAfamacio = valor;
    }

    public void TempsAssedegacio(float valor)
    {
        tempsAssedecacioText.text = valor.ToString();
        especieSeleccionada.tempsAssedegament = valor;
    }

    public void TempsGanesDeReproduccio(float valor)
    {
        tempsGanesReproduccioText.text = valor.ToString();
        especieSeleccionada.tempsGanesReproduccio = valor;
    }

    public void TempsGestacio(float valor)
    {
        tempsGestacioText.text = valor.ToString();
        especieSeleccionada.tempsGestacio = valor;
    }

    public void Deteccio(float valor)
    {
        detecioText.text = valor.ToString();
        //especieSeleccionada.deteccio = valor;
    }

    public void FillsMaxims(float valor)
    {
        fillsVariacioText.text = "Màxims (" + valor.ToString() + ")";
        especieSeleccionada.fillsMaxims = (int)valor;
    }

    public void FillsMinims(float valor)
    {
        fillsBaseText.text = "Minims (" + valor.ToString() + ")";
        especieSeleccionada.fillsMinims = (int)valor;
    }
    #endregion

    public void PujarNombreIndividus(int id, int quatitat, Text text, bool normal)
    {
        if (normal)
        {
            InformacioSimulacio.instance.individusPerFerApareixerNormal[id] += quatitat * multiplicadorAfegirEspecies;
            InformacioSimulacio.instance.individusPerFerApareixerNormal[id] = Mathf.Clamp(InformacioSimulacio.instance.individusPerFerApareixerNormal[id], 0, 100);
            text.text = InformacioSimulacio.instance.individusPerFerApareixerNormal[id].ToString();

            return;
        }

        InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat[id] += quatitat * multiplicadorAfegirEspecies;
        InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat[id] = Mathf.Clamp(InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat[id], 0, 100);
        text.text = InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat[id].ToString();
    }

    public void CanviarProbabilitatMutacio(float prob)
    {
        InformacioSimulacio.instance.probabilitatMutacio = prob;

        probabilitatMutacioText.text = "Probabilitat mutació (" + prob.ToString() + "%)";
    }

    public void LlistaArxius()
    {
        CarregarArxius();
        llistaArxius.SetActive(true);
        jsonObert.SetActive(false);
    }

    public void LlistaArxius_Normal()
    {
        llistaArxiusNormal.SetActive(true);
        llistaArxiusPersonalitzats.SetActive(false);
    }

    public void LlistaArxius_Personalitzats()
    {
        llistaArxiusNormal.SetActive(false);
        llistaArxiusPersonalitzats.SetActive(true);
    }
}
