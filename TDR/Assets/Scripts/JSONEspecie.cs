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

    #region OH_DIOS
    [Header("JSONObert")]
    [SerializeField] Button saveButton;

    [SerializeField] Text nomText;

    [SerializeField] InputField nomSingular;
    [SerializeField] InputField nomPlural;
    [SerializeField] Toggle articleMasculi;
    [SerializeField] Toggle articleFemeni;

    //AIXO ES HORROROS AAAH

    [SerializeField] Slider velocitatCaminacioBaseSlider;
    [SerializeField] Slider velocitatCaminacioVariacioSlider;    
    [SerializeField] Text velocitatCaminacioBaseText;
    [SerializeField] Text velocitatCaminacioVariacioText;

    [SerializeField] Slider velocitatCorrerBaseSlider;
    [SerializeField] Slider velocitatCorrerVariacioSlider;
    [SerializeField] Text velocitatCorrerBaseText;
    [SerializeField] Text velocitatCorrerVariacioText;

    [SerializeField] Slider tempsAfamacioBaseSlider;
    [SerializeField] Slider tempsAfamacioVariacioSlider;
    [SerializeField] Text tempsAfamacioBaseText;
    [SerializeField] Text tempsAfamacioVariacioText;

    [SerializeField] Slider tempsAssedecacioBaseSlider;
    [SerializeField] Slider tempsAssedecacioVariacioSlider;
    [SerializeField] Text tempsAssedecacioBaseText;
    [SerializeField] Text tempsAssedecacioVariacioText;

    [SerializeField] Slider tempsGanesReproduccioBaseSlider;
    [SerializeField] Slider tempsGanesReproduccioVariacioSlider;
    [SerializeField] Text tempsGanesReproduccioBaseText;
    [SerializeField] Text tempsGanesReproduccioVariacioText;

    [SerializeField] Slider tempsGestacioBaseSlider;
    [SerializeField] Slider tempsGestacioVariacioSlider;
    [SerializeField] Text tempsGestacioBaseText;
    [SerializeField] Text tempsGestacioVariacioText;

    [SerializeField] Slider detecioBaseSlider;
    [SerializeField] Slider detecioVariacioSlider;
    [SerializeField] Text detecioBaseText;
    [SerializeField] Text detecioVariacioText;

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
        InformacioSimulacio.instance = new InformacioSimulacio(100);
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

        velocitatCaminacioBaseSlider.value = especieSeleccionada.velocitatCaminarBase;
        velocitatCaminacioVariacioSlider.value = especieSeleccionada.velocitatCaminarVariacio;

        velocitatCorrerBaseSlider.value = especieSeleccionada.velocitatCorrerBase;
        velocitatCorrerVariacioSlider.value = especieSeleccionada.velocitatCorrerVariacio;

        tempsAfamacioBaseSlider.value = especieSeleccionada.tempsAfamacioBase;
        tempsAfamacioVariacioSlider.value = especieSeleccionada.tempsAfamacioVariacio;

        tempsAssedecacioBaseSlider.value = especieSeleccionada.tempsAssedegamentBase;
        tempsAssedecacioVariacioSlider.value = especieSeleccionada.tempsAssedegamentVariacio;

        tempsGanesReproduccioBaseSlider.value = especieSeleccionada.tempsGanesReproduccioBase;
        tempsGanesReproduccioVariacioSlider.value = especieSeleccionada.tempsGanesReproduccioVariacio;

        tempsGestacioBaseSlider.value = especieSeleccionada.tempsGestacioBase;
        tempsGestacioVariacioSlider.value = especieSeleccionada.tempsGestacioVariacio;

        detecioBaseSlider.value = especieSeleccionada.deteccioBase;
        detecioVariacioSlider.value = especieSeleccionada.deteccioVariacio;

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

    public void VelocitatCaminacioBase(float valor)
    {
        velocitatCaminacioBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.velocitatCaminarBase = valor;
    }

    public void VelocitatCaminacioVariacio(float valor)
    {
        velocitatCaminacioVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.velocitatCaminarVariacio = valor;
    }

    public void VelocitatCorrecioBase(float valor)
    {
        velocitatCorrerBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.velocitatCorrerBase = valor;
    }

    public void VelocitatCorrecioVariacio(float valor)
    {
        velocitatCorrerVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.velocitatCorrerVariacio = valor;
    }

    public void TempsAfamacioBase(float valor)
    {
        tempsAfamacioBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.tempsAfamacioBase = valor;
    }

    public void TempsAfamacioVariacio(float valor)
    {
        tempsAfamacioVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.tempsAfamacioVariacio = valor;
    }

    public void TempsAssedegacioBase(float valor)
    {
        tempsAssedecacioBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.tempsAssedegamentBase = valor;
    }

    public void TempsAssedegacioVariacio(float valor)
    {
        tempsAssedecacioVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.tempsAssedegamentVariacio = valor;
    }

    public void TempsGanesDeReproduccioBase(float valor)
    {
        tempsGanesReproduccioBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.tempsGanesReproduccioBase = valor;
    }

    public void TempsGanesDeReproduccioVariacio(float valor)
    {
        tempsGanesReproduccioVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.tempsGanesReproduccioVariacio = valor;
    }

    public void TempsGestacioBase(float valor)
    {
        tempsGestacioBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.tempsGestacioBase = valor;
    }

    public void TempsGestacioVariacio(float valor)
    {
        tempsGestacioVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.tempsGestacioVariacio = valor;
    }

    public void DeteccioBase(float valor)
    {
        detecioBaseText.text = "Base (" + valor.ToString() + ")";
        especieSeleccionada.deteccioBase = valor;
    }

    public void DeteccioVariacio(float valor)
    {
        detecioVariacioText.text = "Variació (" + valor.ToString() + ")";
        especieSeleccionada.deteccioVariacio = valor;
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
