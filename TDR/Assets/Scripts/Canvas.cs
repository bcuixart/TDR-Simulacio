using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    public static Canvas instance;

    [Header("Seleccionar individus")]
    [SerializeField] GameObject seleccionarIndividuFons;
    [SerializeField] GameObject seleccionarIndividuObrir;
    [SerializeField] GameObject seleccionarIndividuSeleccionat;
    [SerializeField] GameObject seleccionarIndividuNoSeleccionat;

    [SerializeField] Text nomEspecie;
    [SerializeField] Text estatText;

    [SerializeField] Text ganaText;
    [SerializeField] Text setText;
    [SerializeField] Text ganesReproduccioText;

    [SerializeField] Text infectatText;
    [SerializeField] Text gestantText;
    
    [SerializeField] Slider ganaSlider;
    [SerializeField] Slider setSlider;
    [SerializeField] Slider ganesReproduccioSlider;

    [SerializeField] Text genereText;

    [SerializeField] Slider salutSlider;
    [SerializeField] Text salutText;

    [Header("Pausa")]
    public GameObject pausaFons;
    [SerializeField] GameObject pausaNova;
    [SerializeField] GameObject pausaRepeticio;
    public GameObject botoDevModeRepeticio;
    public GameObject devModeIndicador;
    public Animator devModeEfectAnim;

    [SerializeField] GameObject pausaAjustamentsPausa;
    [SerializeField] GameObject pausaAjustamentsAjustaments;

    [Header("Temps")]
    [SerializeField] GameObject tempsFons;
    [SerializeField] GameObject tempsObrir;

    [SerializeField] Text tempsText;

    [SerializeField] List<Text> velocitatImages;
    [SerializeField] Text pausaImage;

    [Header("Controls")]
    [SerializeField] GameObject controlsFons;
    [SerializeField] GameObject controlsObrir;

    [Header("Dades")]
    [SerializeField] GameObject dadesFons;
    [SerializeField] GameObject dadesObrir;

    public Transform dadesLlistaEspeciesNormalBase;
    public Transform dadesLlistaEspeciesPersonalitzadesBase;

    public GameObject dadesLlistaEspeciesPrefab;

    [SerializeField] GameObject dadesLlistaNormal;
    [SerializeField] GameObject dadesLlistaPersonalitzades;

    [SerializeField] Text especieNomText;
    [SerializeField] Text especieNombreText;
    [SerializeField] Text especieNombreInfectatsText;
    [SerializeField] GameObject dadesLlista;
    [SerializeField] GameObject dadesEspecie;

    [SerializeField] Grafic poblacioGrafic;
    [SerializeField] Grafic infectatsGrafic;
    [SerializeField] Grafic percentatgeGrafic;
    [SerializeField] Grafic salutGrafic;

    [Header("Notificacions")]
    [SerializeField] GameObject notificacionsFons;
    [SerializeField] GameObject notificacionsBase;
    [SerializeField] GameObject notificacionsObrir;
    [SerializeField] GameObject notificacionsBasePrefab;
    [SerializeField] GameObject notificacionsTransform;
    [SerializeField] GameObject notificacionsPrefab;

    [Header("Sorolls")]
    [SerializeField] AudioSource audioS;
    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip selectSoundSubnormal;
    [SerializeField] AudioClip tornarSound;
    [SerializeField] AudioClip tornarSoundSubnormal;

    int dadesEspecieSeleccionadaID;
    bool dadesEspecieSeleccionadaPersonalitzada;

    public static bool obertIndividuSeleccionat;
    public static bool obertTemps;
    public static bool obertDades;
    public static bool obertNotificacions;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (GameManager.instance.info.finalitzada)
        {
            pausaNova.SetActive(false);
            pausaRepeticio.SetActive(true);
        }

        if(PlayerPrefs.GetInt("ObrirTutorial", 0) == 0)
        {
            PlayerPrefs.SetInt("ObrirTutorial", 1);

            ObrirControls();
        }

        InvokeRepeating("FormatejarTextTemps", 0, 1);

        if (Menu.devMode)
        {
            botoDevModeRepeticio.SetActive(true);
            devModeIndicador.SetActive(true);
        }

        if (obertNotificacions)
        {
            ObrirNotificacions();
        }

        if (obertIndividuSeleccionat)
        {
            ObrirIndividuSeleccionar();
        }

        if (obertDades)
        {
            ObrirDades();
        }

        if (obertTemps)
        {
            ObrirTemps();
        }
    }

    void Update()
    {
        
    }

    void FormatejarTextTemps()
    {
        float temps = GameManager.instance.elapsed;

        int hores = Mathf.FloorToInt(temps / 3600f);
        int minuts = Mathf.FloorToInt((temps -3600 * hores) / 60f);
        int segons = (int)temps - 3600 * hores - 60 * minuts;

        tempsText.text = (hores > 0) ? hores.ToString("00") + ":" + minuts.ToString("00") + ":" + segons.ToString("00") : minuts.ToString("00") + ":" + segons.ToString("00");
    }

    public void SeleccionarIndividu(Individu individu)
    {
        Soroll_Seleccionar();

        if(individu == null)
        {
            seleccionarIndividuNoSeleccionat.SetActive(true);
            seleccionarIndividuSeleccionat.SetActive(false);
            seleccionarIndividuFons.SetActive(true);
            seleccionarIndividuObrir.SetActive(false);

            return;
        }

        seleccionarIndividuNoSeleccionat.SetActive(false);
        seleccionarIndividuSeleccionat.SetActive(true);
        seleccionarIndividuFons.SetActive(true);
        seleccionarIndividuObrir.SetActive(false);

        nomEspecie.text = individu.especie.nomSingular;

        ganaText.text = individu.gana.ToString() + "%";
        setText.text = individu.set.ToString() + "%";
        ganesReproduccioText.text = individu.ganesDeReproduirse.ToString() + "%";

        ganaSlider.value = individu.gana;
        setSlider.value = individu.set;
        ganesReproduccioSlider.value = individu.ganesDeReproduirse;

        infectatText.text = individu.infectat ? ("Sí (" + individu.procesMalaltia + "% de malaltia)") : "No";
        gestantText.text = individu.embarassada ? ("Sí (" + 100f* (individu.procesEmbaras/individu.especie.tempsGestacio) + "%)") : "No";

        salutSlider.value = individu.genoma.gens[0].gen;
        salutText.text = individu.genoma.gens[0].gen.ToString();

        if (Menu.subnormal)
        {
            genereText.text = (individu.genoma.genere == Genere.Masculí) ? "MACHO IBÉRICO" : "MACHA IBÉRICA";

            switch (individu.estat)
            {
                case EstatIndividu.Normal:
                    estatText.text = "Livin' la vida.";
                    break;
                case EstatIndividu.Dormint:
                    estatText.text = "Mumint.";
                    break;
                case EstatIndividu.BuscantMenjar:
                    estatText.text = "Hora de dinar!";
                    break;
                case EstatIndividu.Menjant:
                    estatText.text = "Fent nyam-nyam.";
                    break;
                case EstatIndividu.Caçant:
                    estatText.text = "Buscant presa.";
                    break;
                case EstatIndividu.BuscantAigua:
                    estatText.text = "Buscant aigua.";
                    break;
                case EstatIndividu.Bevent:
                    estatText.text = "Fent glub-glub.";
                    break;
                case EstatIndividu.BuscantParella:
                    estatText.text = "Buscant novio/a.";
                    break;
                case EstatIndividu.Copulant:
                    estatText.text = "Fent $*!?";
                    break;
                case EstatIndividu.DonantALlum:
                    estatText.text = "Parint.";
                    break;
                case EstatIndividu.Mort:
                    estatText.text = "Morit :)";
                    break;
                case EstatIndividu.SentMenjat:
                    estatText.text = "Sent brutalment devorat";
                    break;
                default:
                    break;
            }
        }
        else
        {
            genereText.text = (individu.genoma.genere == Genere.Masculí) ? "Mascle" : "Femella";

            switch (individu.estat)
            {
                case EstatIndividu.Normal:
                    estatText.text = "Normal.";
                    break;
                case EstatIndividu.Dormint:
                    estatText.text = "Dormint.";
                    break;
                case EstatIndividu.BuscantMenjar:
                    estatText.text = "Buscant menjar.";
                    break;
                case EstatIndividu.Menjant:
                    estatText.text = "Menjant.";
                    break;
                case EstatIndividu.Caçant:
                    estatText.text = "Buscant presa.";
                    break;
                case EstatIndividu.BuscantAigua:
                    estatText.text = "Buscant aigua.";
                    break;
                case EstatIndividu.Bevent:
                    estatText.text = "Bevent.";
                    break;
                case EstatIndividu.BuscantParella:
                    estatText.text = "Buscant parella.";
                    break;
                case EstatIndividu.Copulant:
                    estatText.text = "Copulant.";
                    break;
                case EstatIndividu.DonantALlum:
                    estatText.text = "Donant a llum.";
                    break;
                case EstatIndividu.Mort:
                    estatText.text = "Mort.";
                    break;
                case EstatIndividu.SentMenjat:
                    estatText.text = "Sent caçat.";
                    break;
                default:
                    break;
            }
        }
    }

    public void RefrescarIndividuSeleccionat()
    {
        SeleccionarIndividu(GameManager.instance.individuSeleccionat);
    }

    public void ObrirIndividuSeleccionar()
    {
        obertIndividuSeleccionat = true;

        seleccionarIndividuFons.SetActive(true);
        seleccionarIndividuObrir.SetActive(false);
    }

    public void TancarIndividuSeleccionat()
    {
        Soroll_Tornar();

        obertIndividuSeleccionat = false;

        seleccionarIndividuFons.SetActive(false);
        seleccionarIndividuObrir.SetActive(true);
    }

    public void ObrirTemps()
    {
        Soroll_Seleccionar();

        obertTemps = true;

        tempsFons.SetActive(true);
        tempsObrir.SetActive(false);
    }

    public void TancarTemps()
    {
        Soroll_Tornar();

        obertTemps = false;

        tempsFons.SetActive(false);
        tempsObrir.SetActive(true);
    }

    public void ObrirControls()
    {
        controlsFons.SetActive(true);
        controlsObrir.SetActive(false);
    }

    public void TancarControls()
    {
        controlsFons.SetActive(false);
        controlsObrir.SetActive(true);
    }

    public void ObrirDades()
    {
        Soroll_Seleccionar();

        obertDades = true;

        dadesFons.SetActive(true);
        dadesObrir.SetActive(false);
    }

    public void TancarDades()
    {
        Soroll_Tornar();

        obertDades = false;

        dadesFons.SetActive(false);
        dadesObrir.SetActive(true);
    }

    public void ObrirNotificacions()
    {
        Soroll_Seleccionar();

        obertNotificacions = true;

        notificacionsFons.SetActive(true);
        notificacionsObrir.SetActive(false);
        notificacionsBase.SetActive(false);
    }

    public void TancarNotificacions()
    {
        Soroll_Tornar();

        obertNotificacions = false;

        notificacionsFons.SetActive(false);
        notificacionsObrir.SetActive(true);
        notificacionsBase.SetActive(true);
    }

    public void NovaNotificacio(Notificacio not)
    {
        GameObject notificacioBase = Instantiate(notificacionsBasePrefab, notificacionsBase.transform);
        Destroy(notificacioBase, 6);

        notificacioBase.GetComponent<Text>().text = Menu.subnormal ? not.textSubnormal.Split('-')[1] : not.text.Split('-')[1];

        GameObject notificacioLlista = Instantiate(notificacionsPrefab, notificacionsTransform.transform);
        notificacioLlista.GetComponent<Text>().text = Menu.subnormal ? not.textSubnormal : not.text;
    }

    public void TriarLlistaDades(int llista)
    {
        Soroll_Seleccionar();
        switch (llista)
        {
            case 0:
                dadesLlistaNormal.SetActive(true);
                dadesLlistaPersonalitzades.SetActive(false);
                break;
            case 1:
                dadesLlistaNormal.SetActive(false);
                dadesLlistaPersonalitzades.SetActive(true);
                break;
        }
    }

    public void ObrirDadesIndividu(int id, bool personalitzat)
    {
        Soroll_Seleccionar();

        dadesEspecieSeleccionadaID = id;
        dadesEspecieSeleccionadaPersonalitzada = personalitzat;

        dadesLlista.SetActive(false);
        dadesEspecie.SetActive(true);

        if (personalitzat)
        {
            especieNombreText.text = "Poblacio actual: " + DadesManager.instance.nombreIndividusPersonalitzats[id];
            float percentatge = 100f * ((float)DadesManager.instance.nombreIndividusPersonalitzatsInfectats[id] / (float)DadesManager.instance.nombreIndividusPersonalitzats[id]);
            especieNombreInfectatsText.text = "Nombre actual infectats: " + DadesManager.instance.nombreIndividusPersonalitzatsInfectats[id] + " (" + percentatge + "%)";

            especieNomText.text = GameManager.instance.especiesPersonalitzades[id].nomPlural;

            List<float> punts=new List<float>();
            List<float> puntsInf=new List<float>();

            for (int i = 0; i < DadesManager.instance.dades.especiesPersonalitzades[id].nombreIndividus.Count; i++)
            {
                punts.Add(DadesManager.instance.dades.especiesPersonalitzades[id].nombreIndividus[i]);
                puntsInf.Add(DadesManager.instance.dades.especiesPersonalitzades[id].nombreIndividusInfectats[i]);
            }

            poblacioGrafic.punts = punts;
            infectatsGrafic.punts = puntsInf;

            percentatgeGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].percentatgeInfectats;
            salutGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].salutMitjana;
        }
        else
        {
            especieNombreText.text = "Poblacio actual: " + DadesManager.instance.nombreIndividusNormals[id];
            float percentatge = 100f * ((float)DadesManager.instance.nombreIndividusNormalsInfectats[id] / (float)DadesManager.instance.nombreIndividusNormals[id]);
            especieNombreInfectatsText.text = "Nombre actual infectats: " + DadesManager.instance.nombreIndividusNormalsInfectats[id] + " (" + percentatge + "%)";

            especieNomText.text = GameManager.instance.especiesNormals[id].nomPlural;

            List<float> punts = new List<float>();
            List<float> puntsInf = new List<float>();

            for (int i = 0; i < DadesManager.instance.dades.especiesNormals[id].nombreIndividus.Count; i++)
            {
                punts.Add(DadesManager.instance.dades.especiesNormals[id].nombreIndividus[i]);
                puntsInf.Add(DadesManager.instance.dades.especiesNormals[id].nombreIndividusInfectats[i]);
            }

            poblacioGrafic.punts = punts;
            infectatsGrafic.punts = puntsInf;

            percentatgeGrafic.punts = DadesManager.instance.dades.especiesNormals[id].percentatgeInfectats;
            salutGrafic.punts = DadesManager.instance.dades.especiesNormals[id].salutMitjana;
        }

        poblacioGrafic.CrearGrafic();
        infectatsGrafic.CrearGrafic();
        percentatgeGrafic.CrearGrafic();
        salutGrafic.CrearGrafic();
    }

    public void RefrescarGrafic()
    {
        Soroll_Seleccionar();

        ObrirDadesIndividu(dadesEspecieSeleccionadaID, dadesEspecieSeleccionadaPersonalitzada);
    }

    public void DeseleccionarDadesEspecie()
    {
        Soroll_Tornar();

        dadesLlista.SetActive(true);
        dadesEspecie.SetActive(false);
    }

    public void DeseleccionarIndividu()
    {
        Soroll_Tornar();

        foreach (Renderer _renderer in GameManager.instance.individuSeleccionat._renderers)
        {
            _renderer.material.SetFloat("_EmissionAmount", 0);
        }

        GameManager.instance.individuSeleccionat = null;

        SeleccionarIndividu(null);
    }

    public void PosarVelocitat()
    {
        Soroll_Seleccionar();

        for (int i = 0; i < velocitatImages.Count; i++)
        {
            if (i == GameManager.instance.velocitat - 1)
            {
                velocitatImages[i].color = new Color32(50, 50, 50, 255);
                continue;
            }

            velocitatImages[i].color = new Color32(120, 120, 120, 255);
        }
    }

    public void UI_Sortir()
    {
        Soroll_Tornar();

        GameManager.instance.AfegirNotificacio(new Notificacio(TipusNotificacio.FinalSimulacioArbitrari, GameManager.instance.elapsed, "", 0));
        StartCoroutine(GameManager.instance.EventMeteorit());
    }

    public void UI_Pausar()
    {
        Soroll_Seleccionar();

        GameManager.instance.Pausa(!GameManager.instance.pausat);
    }

    public void UI_Repetir()
    {
        Soroll_Seleccionar();

        Time.timeScale = 1;
        GameManager.instance.info.notificacions.Clear();
        SceneManager.LoadScene("Simulacio");
    }

    public void PosarPausa()
    {
        pausaAjustamentsAjustaments.SetActive(false);
        pausaAjustamentsPausa.SetActive(true);

        pausaImage.color = (GameManager.instance.pausat) ? new Color32(50, 50, 50, 255) : new Color32(120, 120, 120, 255);

        if (!GameManager.instance.pausat)
        {
            PosarVelocitat();

            return;
        }

        for (int i = 0; i < velocitatImages.Count; i++)
        {
            velocitatImages[i].color = new Color32(120, 120, 120, 255);
        }
    }

    public void Ajustaments()
    {
        Soroll_Seleccionar();
        pausaAjustamentsAjustaments.SetActive(true);
        pausaAjustamentsPausa.SetActive(false);
    }

    public void Ajustaments_Tancar()
    {
        Soroll_Tornar();
        pausaAjustamentsAjustaments.SetActive(false);
        pausaAjustamentsPausa.SetActive(true);
    }

    public void SeleccionarVelocitat(int velocitat)
    {
        GameManager.instance.PosarVelocitat(velocitat);
    }

    public void Soroll_Seleccionar()
    {
        audioS.PlayOneShot(Menu.subnormal ? selectSoundSubnormal : selectSound);
    }

    public void Soroll_Tornar()
    {
        audioS.PlayOneShot(Menu.subnormal ? tornarSoundSubnormal : tornarSound);
    }
}
