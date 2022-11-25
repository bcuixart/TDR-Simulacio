using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Conya Oscar")]
    [SerializeField] GameObject conyaOscarSerio;
    [SerializeField] GameObject conyaOscarConya;

    [Header("Nova simulacio - Ubicacio")]
    [SerializeField] InputField nomSimulacioInput;
    [SerializeField] Text nomSimulacioInputText;

    public string ubicacioSimulacio;

    [SerializeField] string novaSimulacioUbicacioActual;
    string novaSimulacioUbicacioBase;

    [SerializeField] InputField novaCarpetaText;
    [SerializeField] Text ubicacioText;

    [SerializeField] Transform novaUbicacioTransform;
    [SerializeField] GameObject carpetaEnrerePrefab;
    [SerializeField] GameObject carpetaPrefab;
    [SerializeField] GameObject arxiuPrefab;
    [SerializeField] GameObject arxiuNovaSimulacioPrefab;
    GameObject arxiuNovaSimulacioActual;

    [Header("Nova simulacio - Individus")]
    [SerializeField] GameObject especieLlistaPrefab;
    [SerializeField] GameObject especieLlistaPersonalitzatPrefab;
    [SerializeField] Transform especieLlistaPredeterminatBase;
    [SerializeField] Transform especieLlistaPersonalitzatBase;

    [Header("Nova simulacio - Altres")]
    [SerializeField] Text probabilitatMutacioText;
    [SerializeField] Text tempsLimitText;

    public List<string> filesNormal = new List<string>();
    public List<string> filesPersonalitzat = new List<string>();

    [Header("EditorEspecies")]
    public MenuEditorEspecie editorEspecies;

    [Header("Simulacions anteriors - Seleccionar")]
    [SerializeField] Transform simulacionsAnteriorsTransform;
    [SerializeField] string simulacioAnteriorSeleccionadaUbicacio;
    [SerializeField] string simulacioAnteriorSeleccionadaNom;

    [SerializeField] Text simulacioAnteriorSeleccionadaUbicacioText;
    [SerializeField] Text simulacioAnteriorSeleccionadaNomText;

    [SerializeField] Button simulacioAnteriorObrirButton;

    [SerializeField] GameObject botoCSV;
    [SerializeField] GameObject botoReplica;

    Text simulacioAnteriorSeleccionadaArxiuText;

    [Header("Simulacions anteriors - Simulacio seleccionada")]
    [SerializeField] MenuSimulacioSeleccionada menuSimulacioSeleccionada;
    [SerializeField] GameObject boto_Captures;

    [Header("Simulacions anteriors - Llista Especies")]
    [SerializeField] GameObject especieLlistaSimulacionsAnteriorsPredeterminatPrefab;
    [SerializeField] Transform especieLlistaSimulacionsAnteriorsPredeterminatBase;

    [Header("Animacions")]
    [SerializeField] Animator camAnim;
    [SerializeField] Animator canvasAnim;

    [Header("DevMode")]
    [SerializeField] GameObject devModeGameObject;
    [SerializeField] GameObject devModeNovaEspeciePredeterminada;

    public static bool devMode;
    int multiplicadorAfegirEspecies;

    void Start()
    {
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Simulacions")))
        {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Simulacions"));
        }

        novaSimulacioUbicacioActual = Path.Combine(Application.dataPath, "Simulacions");
        novaSimulacioUbicacioBase = novaSimulacioUbicacioActual;

        InformacioSimulacio.instance = new InformacioSimulacio(10f, 10, 30);

        CarregarArxiusNovaUbicacio(novaSimulacioUbicacioActual);
        CarregarArxiusNovaUbicacioSimulacionsAnteriors(novaSimulacioUbicacioActual);

        GameManager.probabilitatSalt = 0.1f;

        if (devMode)
        {
            devModeGameObject.SetActive(true);
            devModeNovaEspeciePredeterminada.SetActive(true);
            OmplirLlistaEspecies();

            botoCSV.SetActive(true);
            botoReplica.SetActive(true);

            boto_Captures.SetActive(true);
        }

        OmplirLlistaEspecies();

        CanviarProbabilitatMutacio(0.1f);
        CanviarTempsMaxim(0);

        Canvas.obertTemps = false;
        Canvas.obertNotificacions = false;
        Canvas.obertIndividuSeleccionat = false;
        Canvas.obertDades = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            conyaOscarConya.SetActive(!conyaOscarConya.activeSelf);
            conyaOscarSerio.SetActive(!conyaOscarSerio.activeSelf);
        }

        multiplicadorAfegirEspecies = 1;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            multiplicadorAfegirEspecies += 4;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            multiplicadorAfegirEspecies *= 10;
        }

        if (Input.GetKeyDown(KeyCode.P) && multiplicadorAfegirEspecies >= 2)
        {
            devMode = true;
            devModeGameObject.SetActive(true);
            devModeNovaEspeciePredeterminada.SetActive(true);
            OmplirLlistaEspecies();

            botoCSV.SetActive(true);
            botoReplica.SetActive(true);

            boto_Captures.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.C) && devMode)
        {
            Time.timeScale = 0;
        }
    }

    #region Nova simulacio - comencar
    public void ComencarSimulacio()
    {
        if (string.IsNullOrWhiteSpace(nomSimulacioInput.text) || File.Exists(InformacioSimulacio.instance.ubicacioSimulacio))
        {
            foreach (Text tx in nomSimulacioInput.GetComponentsInChildren<Text>())
            {
                tx.color = Color.red;
            }

            nomSimulacioInputText.color = Color.red;

            PosarCanvasNovaSimulacioAnim(0);

            return;
        }

        nomSimulacioInputText.color = Color.white;

        foreach (Text tx in nomSimulacioInput.GetComponentsInChildren<Text>())
        {
            tx.color = Color.white;
        }

        SceneManager.LoadScene("Simulacio");
    }
    #endregion

    #region Nova simulacio - nom i ubicacio
    public void OmplirLlistaEspecies()
    {
        filesNormal.Clear();
        filesPersonalitzat.Clear();

        foreach (Transform child in especieLlistaPersonalitzatBase)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in especieLlistaPredeterminatBase)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in especieLlistaSimulacionsAnteriorsPredeterminatBase)
        {
            Destroy(child.gameObject);
        }

        string[] files = Directory.GetFiles(Path.Combine(Application.dataPath, "Especies"));
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

                GameObject GO = Instantiate(devMode ? especieLlistaPersonalitzatPrefab : especieLlistaPrefab, especieLlistaPredeterminatBase);
                GameObject GO2 = Instantiate(especieLlistaSimulacionsAnteriorsPredeterminatPrefab, especieLlistaSimulacionsAnteriorsPredeterminatBase);
                GO.SetActive(true);
                GO2.SetActive(true);
                GO.name = id.ToString();
                GO2.name = id.ToString();
                GO.GetComponentInChildren<Text>().text = id.ToString("000") + " - " + pm.nomPlural;
                GO2.GetComponentInChildren<Text>().text = id.ToString("000") + " - " + pm.nomPlural;

                if (devMode)
                {
                    Transform editarBoto = GO.transform.GetChild(4);
                    editarBoto.GetComponent<Button>().onClick.AddListener(delegate { PosarCanvasAnim(2); });
                    editarBoto.GetComponent<Button>().onClick.AddListener(delegate { PosarCamAnim(2); });
                    editarBoto.GetComponent<Button>().onClick.AddListener(delegate { ObrirDadesEspecie(id, true); });
                }

                Transform t1 = GO.transform.GetChild(2);
                Transform t2 = GO.transform.GetChild(3);
                Text txt = GO.transform.GetChild(1).GetComponent<Text>();

                txt.text = InformacioSimulacio.instance.individusPerFerApareixerNormal[id].ToString();

                t1.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, 1, txt, true); });
                t2.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, -1, txt, true); });

                GO2.GetComponent<Button>().onClick.AddListener(delegate { ObrirSimulacioAnteriorGrafic(id); });

                stream.Close();
            }
        }

        string[] filesCustom = Directory.GetFiles(Path.Combine(Application.dataPath, "StreamingAssets"));
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

                GameObject GO = Instantiate(especieLlistaPersonalitzatPrefab, especieLlistaPersonalitzatBase);
                GO.SetActive(true);
                GO.name = id.ToString();
                GO.GetComponentInChildren<Text>().text = id.ToString("000") + " - " + pm.nomPlural;

                Transform editarBoto = GO.transform.GetChild(4);
                editarBoto.GetComponent<Button>().onClick.AddListener(delegate { PosarCanvasAnim(2); }); 
                editarBoto.GetComponent<Button>().onClick.AddListener(delegate { PosarCamAnim(2); });
                editarBoto.GetComponent<Button>().onClick.AddListener(delegate { ObrirDadesEspecie(id, false); });

                Transform t1 = GO.transform.GetChild(2);
                Transform t2 = GO.transform.GetChild(3);
                Text txt = GO.transform.GetChild(1).GetComponent<Text>();

                txt.text = InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat[id].ToString();

                t1.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, 1, txt, false); });
                t2.GetComponent<Button>().onClick.AddListener(delegate { PujarNombreIndividus(id, -1, txt, false); });

                stream.Close();
            }
        }
    }

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

    public void CrearNovaEspecie(bool personalitzat)
    {
        int files = personalitzat ? filesPersonalitzat.Count : filesNormal.Count;

        string nom = "";
        if (personalitzat)
        {
            if(files == 30)
            {
                return;
            }

            nom = Path.Combine(Application.dataPath, "StreamingAssets", "Especie_" + files.ToString("000") + ".json");
        }
        else
        {
            nom = Path.Combine(Application.dataPath, "Especies", "Especie_" + files.ToString("000") + ".json");
        }

        using (StreamWriter stream = new StreamWriter(nom))
        {
            string s = JsonUtility.ToJson(new ParametresEspecie(files, "Nova espècie", "Nova espècie"), true);

            stream.Write(s);

            stream.Close();
        }

        OmplirLlistaEspecies();
    }

    public void CarregarArxiusNovaUbicacio(string path)
    {
        novaSimulacioUbicacioActual = path;

        ubicacioSimulacio = Path.Combine(novaSimulacioUbicacioActual, InformacioSimulacio.instance.nomSimulacio + ".json");
        ubicacioText.text = ubicacioSimulacio;

        foreach (Transform t in novaUbicacioTransform)
        {
            if (t.gameObject.activeSelf)
            {
                Destroy(t.gameObject);
            }
        }

        if(new DirectoryInfo(path).FullName != new DirectoryInfo(novaSimulacioUbicacioBase).FullName)
        {
            GameObject E = Instantiate(carpetaEnrerePrefab, novaUbicacioTransform);
            E.SetActive(true);

            Button bt = E.GetComponentInChildren<Button>();
            bt.onClick.AddListener(() => CarregarArxiusNovaUbicacio(Directory.GetParent(path).FullName));
        }

        string[] drs = Directory.GetDirectories(path);
        foreach (string dr in drs)
        {
            GameObject C = Instantiate(carpetaPrefab, novaUbicacioTransform);
            C.SetActive(true);
            C.GetComponentInChildren<Text>().text = new DirectoryInfo(dr).Name;

            Button bt = C.GetComponentInChildren<Button>();
            bt.onClick.AddListener(() => CarregarArxiusNovaUbicacio(dr));
        }

        string[] fls = Directory.GetFiles(path);
        foreach (string fl in fls)
        {
            if (!fl.EndsWith(".json"))
            {
                continue;
            }

            GameObject F = Instantiate(arxiuPrefab, novaUbicacioTransform);
            F.SetActive(true);
            F.GetComponentInChildren<Text>().text = new FileInfo(fl).Name;
        }

        Destroy(arxiuNovaSimulacioActual);
        arxiuNovaSimulacioActual = Instantiate(arxiuNovaSimulacioPrefab, novaUbicacioTransform);
        arxiuNovaSimulacioActual.SetActive(true);
        arxiuNovaSimulacioActual.GetComponentInChildren<Text>().text = InformacioSimulacio.instance.nomSimulacio + ".json";

        if (string.IsNullOrWhiteSpace(InformacioSimulacio.instance.nomSimulacio))
        {
            arxiuNovaSimulacioActual.SetActive(false);
        }
    }

    public void CanviarNomNovaSimulacio(string nom)
    {
        ubicacioSimulacio = Path.Combine(novaSimulacioUbicacioActual, nom + ".json");
        ubicacioText.text = ubicacioSimulacio;

        InformacioSimulacio.instance.nomSimulacio = nom;
        InformacioSimulacio.instance.ubicacioSimulacio = ubicacioSimulacio;

        arxiuNovaSimulacioActual.GetComponentInChildren<Text>().text = InformacioSimulacio.instance.nomSimulacio + ".json";
        arxiuNovaSimulacioActual.SetActive(true);

        nomSimulacioInputText.color = Color.white;
        foreach (Text tx in nomSimulacioInput.GetComponentsInChildren<Text>())
        {
            tx.color = Color.white;
        }

        if (string.IsNullOrWhiteSpace(InformacioSimulacio.instance.nomSimulacio))
        {
            arxiuNovaSimulacioActual.SetActive(false);
        }
    }

    public void NovaCarpeta()
    {
        if (string.IsNullOrWhiteSpace(novaCarpetaText.text))
        {
            foreach (Text tx in novaCarpetaText.GetComponentsInChildren<Text>())
            {
                tx.color = Color.red;
            }
            
            return;
        }

        foreach (Text tx in novaCarpetaText.GetComponentsInChildren<Text>())
        {
            tx.color = Color.white;
        }

        Directory.CreateDirectory(Path.Combine(novaSimulacioUbicacioActual, novaCarpetaText.text));

        CarregarArxiusNovaUbicacio(novaSimulacioUbicacioActual);

        novaCarpetaText.text = "";
    }
    #endregion

    #region Nova simulacio - altres
    public void CanviarProbabilitatMutacio(float prob)
    {
        InformacioSimulacio.instance.probabilitatMutacio = prob * 100f;

        probabilitatMutacioText.text = (100 * prob).ToString("0.00") + "%";
    }

    public void CanviarTempsMaxim(float temps)
    {
        float[] tempss = new float[] { -1, 600, 1200, 1800, 3600, 7200, 10800, 18000, 36000 };
        string[] tempsStrings = new string[] { "Sense temps màxim", "10 minuts", "20 minuts", "30 minuts", "1 hora", "2 hores", "3 hores", "5 hores", "10 hores" };

        InformacioSimulacio.instance.tempsMaxim = tempss[(int)temps];
        tempsLimitText.text = tempsStrings[(int)temps];
    }
    #endregion

    #region Editor especies
    public void ObrirDadesEspecie(int _id, bool _normal)
    {
        editorEspecies.ObrirDadesEspecie(_id, _normal);
    }

    public void CanviarIDEspecies()
    {
        filesPersonalitzat.Clear();

        string[] filesCustom = Directory.GetFiles(Path.Combine(Application.dataPath, "StreamingAssets"));
        int currentElement = 0;
        for (int i = 0; i < filesCustom.Length; i++)
        {
            if (!filesCustom[i].EndsWith(".json"))
            {
                continue;
            }

            filesPersonalitzat.Add(filesCustom[i]);

            ParametresEspecie pm = new ParametresEspecie(0, "", "");

            using (StreamReader stream = new StreamReader(filesCustom[i]))
            {
                string fileData = stream.ReadToEnd();

                ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

                pm = parametres;
                pm.id = currentElement;

                string s = JsonUtility.ToJson(pm, true);

                stream.Close();
            }

            File.Delete(filesCustom[i]);

            filesCustom[i] = Path.Combine(Application.dataPath, "StreamingAssets", "Especie_" + currentElement.ToString("000") + ".json");
            using (StreamWriter stream = new StreamWriter(filesCustom[i]))
            {
                string s = JsonUtility.ToJson(pm, true);

                stream.Write(s);

                stream.Close();
            }


            currentElement++;
        }

        OmplirLlistaEspecies();
    }
    #endregion

    #region Simulacions anteriors - seleccionar
    public void DeseleccionarSimulacioAnterior()
    {
        CarregarArxiusNovaUbicacioSimulacionsAnteriors(novaSimulacioUbicacioBase);
    }

    public void CarregarArxiusNovaUbicacioSimulacionsAnteriors(string path)
    {
        simulacioAnteriorSeleccionadaNomText.text = "...";
        simulacioAnteriorSeleccionadaUbicacioText.text = "...";

        if(simulacioAnteriorSeleccionadaArxiuText != null)
        {
            simulacioAnteriorSeleccionadaArxiuText.fontStyle = FontStyle.Italic;
        }

        simulacioAnteriorObrirButton.interactable = false;

        foreach (Transform t in simulacionsAnteriorsTransform)
        {
            if (t.gameObject.activeSelf)
            {
                Destroy(t.gameObject);
            }
        }

        if (new DirectoryInfo(path).FullName != new DirectoryInfo(novaSimulacioUbicacioBase).FullName)
        {
            GameObject E = Instantiate(carpetaEnrerePrefab, simulacionsAnteriorsTransform);
            E.SetActive(true);

            Button bt = E.GetComponentInChildren<Button>();
            bt.onClick.AddListener(() => CarregarArxiusNovaUbicacioSimulacionsAnteriors(Directory.GetParent(path).FullName));
        }

        string[] drs = Directory.GetDirectories(path);
        foreach (string dr in drs)
        {
            GameObject C = Instantiate(carpetaPrefab, simulacionsAnteriorsTransform);
            C.SetActive(true);
            C.GetComponentInChildren<Text>().text = new DirectoryInfo(dr).Name;

            Button bt = C.GetComponentInChildren<Button>();
            bt.onClick.AddListener(() => CarregarArxiusNovaUbicacioSimulacionsAnteriors(dr));
        }

        string[] fls = Directory.GetFiles(path);
        foreach (string fl in fls)
        {
            if (!fl.EndsWith(".json"))
            {
                continue;
            }

            GameObject F = Instantiate(arxiuPrefab, simulacionsAnteriorsTransform);
            F.SetActive(true);
            F.GetComponentInChildren<Text>().text = new FileInfo(fl).Name;

            Button b = F.GetComponentInChildren<Button>();
            b.onClick.AddListener(() => SeleccionarSimulacioAnterior(fl, F.GetComponentInChildren<Text>()));
        }
    }

    public void SeleccionarSimulacioAnterior(string path, Text txt)
    {
        if(simulacioAnteriorSeleccionadaArxiuText != null)
        {
            simulacioAnteriorSeleccionadaArxiuText.fontStyle = FontStyle.Normal;
        }

        bool dobleClicObrir = false;

        if(simulacioAnteriorSeleccionadaArxiuText == txt)
        {
            dobleClicObrir = true;
        }

        simulacioAnteriorSeleccionadaUbicacio = path;
        simulacioAnteriorSeleccionadaUbicacioText.text = simulacioAnteriorSeleccionadaUbicacio;

        simulacioAnteriorSeleccionadaNom = new DirectoryInfo(path).Name;
        simulacioAnteriorSeleccionadaNomText.text = simulacioAnteriorSeleccionadaNom;

        simulacioAnteriorSeleccionadaArxiuText = txt;
        simulacioAnteriorSeleccionadaArxiuText.fontStyle = FontStyle.Italic;

        simulacioAnteriorObrirButton.interactable = true;

        if (dobleClicObrir)
        {
            ObrirSimulacioSeleccionada();
        }
    }

    public void ObrirSimulacioSeleccionada()
    {
        PosarCamAnim(2);
        PosarCanvasSimulacionsAnteriorsAnim(1);

        menuSimulacioSeleccionada.ObrirDadesGenerals();
        menuSimulacioSeleccionada.CarregarSimulacio(simulacioAnteriorSeleccionadaUbicacio);
    }

    public void ObrirSimulacioAnteriorGrafic(int id)
    {
        menuSimulacioSeleccionada.OmplirGrafic(id);
    }
    #endregion

    #region Cam i canvasAnim
    public void PosarCamAnim(float animState)
    {
        camAnim.SetFloat("Girar", animState);
    }

    public void PosarCanvasAnim(float animState)
    {
        canvasAnim.SetFloat("Menu", animState);
    }

    public void PosarCanvasPrincipalAnim(float animState)
    {
        canvasAnim.SetFloat("Menu_Principal", animState);
    }

    public void PosarCanvasNovaSimulacioAnim(float animState)
    {
        canvasAnim.SetFloat("Menu_NovaSimulacio", animState);
    }

    public void PosarCanvasNovaSimulacioIndividusAnim(float animState)
    {
        canvasAnim.SetFloat("Menu_NovaSimulacio_Individus", animState);
    }

    public void PosarCanvasEditorEspecieAnim(float animState)
    {
        canvasAnim.SetFloat("Menu_EditorEspecies", animState);
    }

    public void PosarCanvasSimulacionsAnteriorsAnim(float animState)
    {
        canvasAnim.SetFloat("Menu_SimulacionsAnteriors", animState);
    }
    #endregion

    public void Sortir()
    {
        Application.Quit();
    }
}
