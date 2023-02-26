using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class MenuSimulacioSeleccionada : MonoBehaviour
{
    [SerializeField] Menu menu;

    [SerializeField] InformacioSimulacio info;

    string path;
    string pathCaptura;

    [Space]
    [Header("Dades generals")]
    [SerializeField] GameObject dadesGenerals;

    [SerializeField] Text nomSimulacioText;
    [SerializeField] Text dataCreacioText;
    [SerializeField] Text tempsMaximText;
    [SerializeField] Text tempsTotalText;
    [SerializeField] Text variabilitatText;
    [SerializeField] Slider variabilitatSlider;    
    [SerializeField] Text infeccioHumansText;
    [SerializeField] Slider infeccioHumansSlider;

    [Header("Notificacions")]
    [SerializeField] GameObject notificacions;

    [SerializeField] GameObject notificacionsPrefab;
    [SerializeField] Transform notificacionsBase;

    [Header("Grafics")]
    [SerializeField] GameObject grafics;

    [SerializeField] Grafic graficPoblacio;
    [SerializeField] Grafic graficInfectats;
    [SerializeField] Grafic graficPercentatge;
    [SerializeField] Grafic graficSalut;

    [SerializeField] GameObject graficPoblacioObject;
    [SerializeField] GameObject graficInfectatsObject;
    [SerializeField] GameObject graficPercentatgeObject;
    [SerializeField] GameObject graficSalutObject;
    [SerializeField] GameObject graficYMinMax;

    [SerializeField] GameObject graficsSeleccionarEspecie;
    [SerializeField] GameObject graficsGrafics;

    [SerializeField] Text graficCapturesText;

    [Header("Captures")]
    [SerializeField] Camera cam;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void CarregarSimulacio(string p)
    {
        path = p;
        pathCaptura = p.Replace(".json", "");
        using (StreamReader stream = new StreamReader(path))
        {
            string fileData = stream.ReadToEnd();

            info = JsonUtility.FromJson<InformacioSimulacio>(fileData);

            stream.Close();
        }

        nomSimulacioText.text = info.nomSimulacio;

        dataCreacioText.text = info.dataCreacio;

        switch (info.tempsMaxim)
        {
            case -1:
                tempsMaximText.text = "Sense temps màxim";
                break;
            case 600:
                tempsMaximText.text = " minuts";
                break;
            case 1200:
                tempsMaximText.text = "20 minuts";
                break;
            case 1800:
                tempsMaximText.text = "30 minuts";
                break;
            case 3600:
                tempsMaximText.text = "1 hora";
                break;
            case 7200:
                tempsMaximText.text = "2 hores";
                break;
            case 10800:
                tempsMaximText.text = "3 hores";
                break;
            case 18000:
                tempsMaximText.text = "5 hores";
                break;
            case 36000:
                tempsMaximText.text = "10 hores";
                break;
        }

        int hores = Mathf.FloorToInt(info.tempsTotal / 3600f);
        int minuts = Mathf.FloorToInt((info.tempsTotal - 3600 * hores) / 60f);
        int segons = (int)info.tempsTotal - 3600 * hores - 60 * minuts;

        tempsTotalText.text = (hores > 0) ? hores.ToString("00") + ":" + minuts.ToString("00") + ":" + segons.ToString("00") : minuts.ToString("00") + ":" + segons.ToString("00");

        variabilitatSlider.value = info.probabilitatMutacio;
        variabilitatText.text = info.probabilitatMutacio.ToString("0.00") + "%";

        infeccioHumansSlider.value = info.infeccioHumans;
        infeccioHumansText.text = info.infeccioHumans.ToString("0.00") + "%";

        foreach (Transform child in notificacionsBase)
        {
            Destroy(child.gameObject);
        }

        foreach (Notificacio not in info.notificacions)
        {
            GameObject GOn = Instantiate(notificacionsPrefab, notificacionsBase);
            GOn.SetActive(true);

            if(!Menu.subnormal || string.IsNullOrEmpty(not.textSubnormal) || string.IsNullOrWhiteSpace(not.textSubnormal))
            {
                GOn.GetComponentInChildren<Text>().text = not.text;
            }
            else
            {
                GOn.GetComponentInChildren<Text>().text = not.textSubnormal;
            }
            
        }
    }

    public void OmplirGrafic(int id)
    {
        graficPoblacio.punts.Clear();
        graficInfectats.punts.Clear();
        graficSalut.punts.Clear();
        graficPercentatge.punts.Clear();

        for (int i = 0; i < info.dades.especiesNormals[id].nombreIndividus.Count; i++)
        {
            graficPoblacio.punts.Add(info.dades.especiesNormals[id].nombreIndividus[i]);
            graficInfectats.punts.Add(info.dades.especiesNormals[id].nombreIndividusInfectats[i]);
            graficSalut.punts.Add((info.dades.especiesNormals[id].salutMitjana.Count <= i) ? 0 : info.dades.especiesNormals[id].salutMitjana[i]);
            graficPercentatge.punts.Add(info.dades.especiesNormals[id].percentatgeInfectats[i]);
        }

        graficPoblacio.CrearGrafic();
        graficInfectats.CrearGrafic();
        graficPercentatge.CrearGrafic();
        graficSalut.CrearGrafic();

        graficsSeleccionarEspecie.SetActive(false);
        graficsGrafics.SetActive(true);
    }

    public void TornarGraficsGrafics()
    {
        graficsSeleccionarEspecie.SetActive(true);
        graficsGrafics.SetActive(false);
    }

    public void ObrirDadesGenerals()
    {
        dadesGenerals.SetActive(true);
        notificacions.SetActive(false);
        grafics.SetActive(false);
    }

    public void ObrirNotificacions()
    {
        dadesGenerals.SetActive(false);
        notificacions.SetActive(true);
        grafics.SetActive(false);
    }

    public void ObrirGrafics()
    {
        dadesGenerals.SetActive(false);
        notificacions.SetActive(false);
        grafics.SetActive(true);
    }

    public void MostrarGraficPoblacio(bool m)
    {
        graficPoblacioObject.SetActive(m);

        if (m)
        {
            graficYMinMax.SetActive(true);
            menu.Soroll_Seleccionar();
        }
        else
        {
            menu.Soroll_Tornar();
            if (!graficInfectatsObject.activeSelf)
            {
                graficYMinMax.SetActive(false);
            }
        }
    }

    public void MostrarGraficInfectats(bool m)
    {
        graficInfectatsObject.SetActive(m);

        if (m)
        {
            graficYMinMax.SetActive(true);
            menu.Soroll_Seleccionar();
        }
        else
        {
            menu.Soroll_Tornar();
            if (!graficPoblacioObject.activeSelf)
            {
                graficYMinMax.SetActive(false);
            }
        }
    }

    public void MostrarGraficPercentatge(bool m)
    {
        graficPercentatgeObject.SetActive(m);

        if (m)
        {
            menu.Soroll_Seleccionar();
        }
        else
        {
            menu.Soroll_Tornar();
        }
    }

    public void MostrarGraficSalut(bool m)
    {
        graficSalutObject.SetActive(m);

        if (m)
        {
            menu.Soroll_Seleccionar();
        }
        else
        {
            menu.Soroll_Tornar();
        }
    }

    public void ComencarRepeticio()
    {
        InformacioSimulacio.instance = info;

        SceneManager.LoadScene("Simulacio");
    }

    public void FerReplica()
    {
        InformacioSimulacio.instance = new InformacioSimulacio(10f, 10, 30);

        InformacioSimulacio.instance.finalitzada = false;

        InformacioSimulacio.instance.tempsMaxim = info.tempsMaxim;
        InformacioSimulacio.instance.probabilitatMutacio = info.probabilitatMutacio;

        InformacioSimulacio.instance.individusPerFerApareixerNormal = info.individusPerFerApareixerNormal;
        InformacioSimulacio.instance.individusPerFerApareixerPersonalitzat = info.individusPerFerApareixerPersonalitzat;

        InformacioSimulacio.instance.notificacions = new List<Notificacio>();
    }

    public void ExportarCSV()
    {
        string path = info.ubicacioSimulacio + ".csv";

        TextWriter tw = new StreamWriter(path, false);
        tw.WriteLine("Temps, Conills, Conills infectats, Salut conills");

        tw.Close();

        tw = new StreamWriter(path, true);
        for (int i = 0; i < info.dades.especiesNormals[0].nombreIndividus.Count; i++)
        {
            string line = (30 * i).ToString() + "," + info.dades.especiesNormals[3].nombreIndividus[i] + "," + info.dades.especiesNormals[3].nombreIndividusInfectats[i] + "," + info.dades.especiesNormals[3].salutMitjana[i];
            tw.WriteLine(line);
        }

        tw.Close();
    }

    public void Captura()
    {
        StartCoroutine(ProcesCaptures());
    }

    IEnumerator ProcesCaptures()
    {
        string t = graficCapturesText.text;

        //Grafic població+infectats conills
        OmplirGrafic(3);
        MostrarGraficInfectats(true);
        MostrarGraficPoblacio(true);
        MostrarGraficPercentatge(false);
        MostrarGraficSalut(false);
        graficCapturesText.text = "Població i infectats de conills-temps (minuts)";
        yield return null;
        FerCaptura(666, 666, "Conills_PoblacioInfectats.png");

        yield return new WaitForSecondsRealtime(0.05f);

        //Grafic percentatge conills
        MostrarGraficInfectats(false);
        MostrarGraficPoblacio(false);
        MostrarGraficPercentatge(true);
        MostrarGraficSalut(false);
        graficCapturesText.text = "Percentatge de conills infectats-temps (minuts)";
        yield return null;
        FerCaptura(666, 666, "Conills_Percentatge.png");

        yield return new WaitForSecondsRealtime(0.05f);

        //Grafic salut conills
        MostrarGraficInfectats(false);
        MostrarGraficPoblacio(false);
        MostrarGraficPercentatge(false);
        MostrarGraficSalut(true);
        graficCapturesText.text = "Mitjana gen de salut en conills-temps (minuts)";
        yield return null;
        FerCaptura(666, 666, "Conills_Salut.png");

        yield return new WaitForSecondsRealtime(0.05f);

        //Grafic infectats humans
        OmplirGrafic(0);
        MostrarGraficInfectats(true);
        MostrarGraficPoblacio(false);
        MostrarGraficPercentatge(false);
        MostrarGraficSalut(false);
        graficCapturesText.text = "Humans infectats-temps (minuts)";
        yield return null;
        FerCaptura(666, 666, "Humans_Infectats.png");
    }

    public void FerCaptura(int w, int h, string captureName)
    {
        cam.targetTexture = RenderTexture.GetTemporary(w, h, 16);

        RenderTexture renderTexture = cam.targetTexture;

        Texture2D result = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(666, 60, renderTexture.width, renderTexture.height);
        result.ReadPixels(rect, 0, 0);

        if (!Directory.Exists(pathCaptura))
        {
            Directory.CreateDirectory(pathCaptura);
        }

        byte[] byteArraw = result.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(pathCaptura, captureName), byteArraw);

        RenderTexture.ReleaseTemporary(renderTexture);
        cam.targetTexture = null;
    }
}
