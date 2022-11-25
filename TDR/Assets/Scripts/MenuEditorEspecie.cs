using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuEditorEspecie : MonoBehaviour
{
    public Menu menu;

    [SerializeField] ParametresEspecie especieSeleccionada;
    [SerializeField] string especieSeleccionadaPath;

    [Header("Nom i aspecte")]
    [SerializeField] InputField nomSingular;
    [SerializeField] InputField nomPlural;

    [SerializeField] Toggle articleMasculi;
    [SerializeField] Toggle articleFemeni;

    [Header("Comportament")]
    [SerializeField] Slider velocitatSlider;
    [SerializeField] Text velocitatText;

    [SerializeField] Slider afamacioSlider;
    [SerializeField] Text afamacioText;

    [SerializeField] Slider gRSlider;
    [SerializeField] Text gRText;

    [SerializeField] Slider gestacioSlider;
    [SerializeField] Text gestacioText;

    [SerializeField] Slider fillsMinimsSlider;
    [SerializeField] Slider fillsMaximsSlider;
    [SerializeField] Text fillsText;

    [SerializeField] Slider probabilitatInfeccioSlider;
    [SerializeField] Text probabilitatInfeccioText;

    [Header("Dieta")]
    [SerializeField] Text NOOO;

    [Header("Esborrar")]
    [SerializeField] GameObject EsborrarSegurSi;
    [SerializeField] GameObject EsborrarSegurNo;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ObrirDadesEspecie(int id, bool normal)
    {
        using (StreamReader stream = new StreamReader((normal) ? menu.filesNormal[id] : menu.filesPersonalitzat[id]))
        {
            string fileData = stream.ReadToEnd();

            ParametresEspecie parametres = JsonUtility.FromJson<ParametresEspecie>(fileData);

            ParametresEspecie pm = parametres;
            int _id = parametres.id;

            especieSeleccionada = pm;
            especieSeleccionadaPath = (normal) ? menu.filesNormal[id] : menu.filesPersonalitzat[id];

            stream.Close();
        }

        nomSingular.text = especieSeleccionada.nomSingular;
        nomPlural.text = especieSeleccionada.nomPlural;

        articleMasculi.isOn = (especieSeleccionada.articleGenere == ArticleGenere.Masculí) ? true : false;
        articleFemeni.isOn = (especieSeleccionada.articleGenere == ArticleGenere.Femení) ? true : false;

        velocitatSlider.value = especieSeleccionada.velocitat;

        afamacioSlider.value = especieSeleccionada.tempsAfamacio;
        gRSlider.value = especieSeleccionada.tempsGanesReproduccio;

        gestacioSlider.value = especieSeleccionada.tempsGestacio;

        fillsMinimsSlider.value = especieSeleccionada.fillsMinims;
        fillsMaximsSlider.value = especieSeleccionada.fillsMaxims;

        probabilitatInfeccioSlider.value = especieSeleccionada.probabilitatInfeccio;
    }

    public void GuardarEspecie()
    {
        using (StreamWriter stream = new StreamWriter(especieSeleccionadaPath))
        {
            string s = JsonUtility.ToJson(especieSeleccionada, true);

            stream.Write(s);

            stream.Close();
        }
    }

    #region OH_DIOS_MIO
    public void PosarNomSingular(string nom)
    {
        especieSeleccionada.nomSingular = nom;
    }    
    
    public void PosarNomPlural(string nom)
    {
        especieSeleccionada.nomPlural = nom;
    }

    public void PosarGenereArticleMasculi(bool on)
    {
        if (on)
        {
            especieSeleccionada.articleGenere = ArticleGenere.Masculí;
        }
    }

    public void PosarGenereArticleFemeni(bool on)
    {
        if (on)
        {
            especieSeleccionada.articleGenere = ArticleGenere.Femení;
        }
    }

    public void PosarVelocitat(float val)
    {
        velocitatText.text = val.ToString("0.##") + "%";
        especieSeleccionada.velocitat = val;
    }

    public void PosarTempsAfamacio(float val)
    {
        Debug.Log(val);
        afamacioText.text = val.ToString("0.##") + "% - Gana completa en " + (0.5f * (100f/val)).ToString("0.##") + " segons.";
        especieSeleccionada.tempsAfamacio = val;
    }

    public void PosarTempsGanesReproduirse(float val)
    {
        gRText.text = val.ToString("0.##") + "% - Ànsia reproductiva completa en " + (0.5f * (100f / val)).ToString("0.##") + " segons.";
        especieSeleccionada.tempsGanesReproduccio = val;
    }

    public void PosarTempsGestacio(float val)
    {
        gestacioText.text = val.ToString("0.##") + " segons";
        especieSeleccionada.tempsGestacio = val;
    }

    public void PosarFillsMinims(float val)
    {
        especieSeleccionada.fillsMinims = (int) val;

        if (val == fillsMaximsSlider.value)
        {
            fillsText.text = (val == 1) ? val.ToString() + " fill sempre." :val.ToString() + " fills sempre.";
        }
        else
        {
            fillsText.text = "Entre " + val.ToString() + " i " + fillsMaximsSlider.value + " fills.";
        }
    }

    public void PosarFillsMaxims(float val)
    {
        fillsMinimsSlider.maxValue = val;
        especieSeleccionada.fillsMaxims = (int)val;

        if(val == fillsMinimsSlider.value)
        {
            fillsText.text = (val == 1) ? val.ToString() + " fill sempre." : val.ToString() + " fills sempre.";
        }
        else
        {
            fillsText.text = "Entre " + fillsMinimsSlider.value + " i " + val.ToString() + " fills.";
        }
    }

    public void PosarProbabilitatInfeccio(float val)
    {
        especieSeleccionada.probabilitatInfeccio = val;
        probabilitatInfeccioText.text = (100f*val).ToString("0.##") + "%";
    }

    public void EsborrarEspecie()
    {
        File.Delete(especieSeleccionadaPath);

        menu.CanviarIDEspecies();
    }

    public void EsborrarSegur()
    {
        EsborrarSegurSi.SetActive(true);
        EsborrarSegurNo.SetActive(false);
    }

    public void EsborrarSegur_No()
    {
        EsborrarSegurSi.SetActive(false);
        EsborrarSegurNo.SetActive(true);
    }

    #endregion
}
