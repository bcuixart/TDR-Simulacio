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
    [SerializeField] GameObject seleccionarIndividuSeleccionat;
    [SerializeField] GameObject seleccionarIndividuNoSeleccionat;

    [SerializeField] Text nomEspecie;
    [SerializeField] Text estatText;

    [SerializeField] Text ganaText;
    [SerializeField] Text setText;
    [SerializeField] Text ganesReproduccioText;    
    
    [SerializeField] Slider ganaSlider;
    [SerializeField] Slider setSlider;
    [SerializeField] Slider ganesReproduccioSlider;

    [SerializeField] Text genereText;

    [SerializeField] Slider velocitatSlider;
    [SerializeField] Slider deteccioSlider;
    [SerializeField] Slider ansiaReproductivaSlider;
    [SerializeField] Slider colorSlider;
    [SerializeField] Slider atractiuSlider;
    [SerializeField] Slider gestacioSlider;

    [Header("Pausa")]
    public GameObject pausaFons;

    [Header("Temps")]
    [SerializeField] GameObject tempsFons;

    [SerializeField] Text tempsText;

    [SerializeField] List<Image> velocitatImages;
    [SerializeField] List<Sprite> velocitatSpritesSeleccionat;
    [SerializeField] List<Sprite> velocitatSpritesNoSeleccionat;

    [SerializeField] Image pausaImage;
    [SerializeField] Sprite pausaSpritePausat;
    [SerializeField] Sprite pausaSpriteNoPausat;

    [Header("Dades")]
    [SerializeField] GameObject dadesFons;

    public Transform dadesLlistaEspeciesNormalBase;
    public Transform dadesLlistaEspeciesPersonalitzadesBase;

    public GameObject dadesLlistaEspeciesPrefab;

    [SerializeField] GameObject dadesLlistaNormal;
    [SerializeField] GameObject dadesLlistaPersonalitzades;

    [SerializeField] Text especieNomText;
    [SerializeField] Text especieNombreText;
    [SerializeField] GameObject dadesLlista;
    [SerializeField] GameObject dadesEspecie;

    [SerializeField] Grafic poblacioGrafic;
    [SerializeField] Grafic velocitatGrafic;
    [SerializeField] Grafic deteccioGrafic;
    [SerializeField] Grafic ansiaReproductivaGrafic;
    [SerializeField] Grafic colorGrafic;
    [SerializeField] Grafic atractiuGrafic;
    [SerializeField] Grafic gestacioGrafic;

    int dadesEspecieSeleccionadaID;
    bool dadesEspecieSeleccionadaPersonalitzada;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InvokeRepeating("FormatejarTextTemps", 0, 1);
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
        if(individu == null)
        {
            seleccionarIndividuNoSeleccionat.SetActive(true);
            seleccionarIndividuSeleccionat.SetActive(false);
            seleccionarIndividuFons.SetActive(true);

            return;
        }

        seleccionarIndividuNoSeleccionat.SetActive(false);
        seleccionarIndividuSeleccionat.SetActive(true);
        seleccionarIndividuFons.SetActive(true);

        nomEspecie.text = individu.especie.nomSingular;

        ganaText.text = "Gana (" + individu.gana.ToString() + "%)";
        setText.text = "Set (" + individu.set.ToString() + "%)";
        ganesReproduccioText.text = "Ganes de reproduir-se (" + individu.ganesDeReproduirse.ToString() + "%)";

        ganaSlider.value = individu.gana;
        setSlider.value = individu.set;
        ganesReproduccioSlider.value = individu.ganesDeReproduirse;

        genereText.text = (individu.genoma.genere == Genere.Masculí) ? "Génere: MACHO IBÉRICO" : "Génere: MACHA IBÉRICA";

        velocitatSlider.value = individu.genoma.gens[0].gen;
        deteccioSlider.value = individu.genoma.gens[1].gen;
        ansiaReproductivaSlider.value = individu.genoma.gens[2].gen;
        colorSlider.value = individu.genoma.gens[3].gen;
        atractiuSlider.value = individu.genoma.gens[4].gen;
        gestacioSlider.value = individu.genoma.gens[5].gen;

        switch (individu.estat)
        {
            case EstatIndividu.Normal:
                estatText.text = "Normal.";
                break;
            case EstatIndividu.Dormint:
                estatText.text = "Mumint.";
                break;
            case EstatIndividu.BuscantMenjar:
                estatText.text = "Buscant menjar.";
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
            default:
                break;
        }
    }

    public void RefrescarIndividuSeleccionat()
    {
        SeleccionarIndividu(GameManager.instance.individuSeleccionat);
    }

    public void ObrirIndividuSeleccionar()
    {
        seleccionarIndividuFons.SetActive(true);
    }

    public void TancarIndividuSeleccionat()
    {
        seleccionarIndividuFons.SetActive(false);
    }

    public void ObrirTemps()
    {
        tempsFons.SetActive(true);
    }

    public void TancarTemps()
    {
        tempsFons.SetActive(false);
    }

    public void ObrirDades()
    {
        dadesFons.SetActive(true);
    }

    public void TancarDades()
    {
        dadesFons.SetActive(false);
    }

    public void TriarLlistaDades(int llista)
    {
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
        dadesEspecieSeleccionadaID = id;
        dadesEspecieSeleccionadaPersonalitzada = personalitzat;

        dadesLlista.SetActive(false);
        dadesEspecie.SetActive(true);

        if (personalitzat)
        {
            especieNombreText.text = "Poblacio actual: " + DadesManager.instance.nombreIndividusPersonalitzats[id];

            especieNomText.text = GameManager.instance.especiesPersonalitzades[id].nomPlural;
            List<float> punts=new List<float>();
            foreach (int item in DadesManager.instance.dades.especiesPersonalitzades[id].nombreIndividus)
            {
                punts.Add(item);
            }
            poblacioGrafic.punts = punts;

            velocitatGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].velocitatMitjana;
            deteccioGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].deteccioMitjana;
            ansiaReproductivaGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].ansiaReproductivaMitjana;
            colorGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].colorMitjana;
            gestacioGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].gestacioMitjana;
            atractiuGrafic.punts = DadesManager.instance.dades.especiesPersonalitzades[id].atractiuMitjana;
        }
        else
        {
            especieNombreText.text = "Poblacio actual: " + DadesManager.instance.nombreIndividusNormals[id];

            especieNomText.text = GameManager.instance.especiesNormals[id].nomPlural;
            List<float> punts = new List<float>();
            foreach (int item in DadesManager.instance.dades.especiesNormals[id].nombreIndividus)
            {
                punts.Add(item);
            }
            poblacioGrafic.punts = punts;
            velocitatGrafic.punts = DadesManager.instance.dades.especiesNormals[id].velocitatMitjana;
            deteccioGrafic.punts = DadesManager.instance.dades.especiesNormals[id].deteccioMitjana;
            ansiaReproductivaGrafic.punts = DadesManager.instance.dades.especiesNormals[id].ansiaReproductivaMitjana;
            colorGrafic.punts = DadesManager.instance.dades.especiesNormals[id].colorMitjana;
            gestacioGrafic.punts = DadesManager.instance.dades.especiesNormals[id].gestacioMitjana;
            atractiuGrafic.punts = DadesManager.instance.dades.especiesNormals[id].atractiuMitjana;
        }

        poblacioGrafic.CrearGrafic();
        velocitatGrafic.CrearGrafic();
        deteccioGrafic.CrearGrafic();
        ansiaReproductivaGrafic.CrearGrafic();
        colorGrafic.CrearGrafic();
        gestacioGrafic.CrearGrafic();
        atractiuGrafic.CrearGrafic();
    }

    public void RefrescarGrafic()
    {
        ObrirDadesIndividu(dadesEspecieSeleccionadaID, dadesEspecieSeleccionadaPersonalitzada);
    }

    public void DeseleccionarDadesEspecie()
    {
        dadesLlista.SetActive(true);
        dadesEspecie.SetActive(false);
    }

    public void DeseleccionarIndividu()
    {
        GameManager.instance.individuSeleccionat._renderer.material.SetFloat("_EmissionAmount", 0);
        GameManager.instance.individuSeleccionat = null;

        SeleccionarIndividu(null);
    }

    public void PosarVelocitat()
    {
        for (int i = 0; i < velocitatImages.Count; i++)
        {
            if (i == GameManager.instance.velocitat - 1)
            {
                velocitatImages[i].sprite = velocitatSpritesSeleccionat[i];
                continue;
            }

            velocitatImages[i].sprite = velocitatSpritesNoSeleccionat[i];
        }
    }

    public void UI_Sortir()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void UI_Pausar()
    {
        GameManager.instance.Pausa(!GameManager.instance.pausat);
    }

    public void PosarPausa()
    {
        pausaImage.sprite = (GameManager.instance.pausat) ? pausaSpritePausat : pausaSpriteNoPausat;

        if (!GameManager.instance.pausat)
        {
            PosarVelocitat();

            return;
        }

        for (int i = 0; i < velocitatImages.Count; i++)
        {
            velocitatImages[i].sprite = velocitatSpritesNoSeleccionat[i];
        }
    }

    public void SeleccionarVelocitat(int velocitat)
    {
        GameManager.instance.PosarVelocitat(velocitat);
    }
}
