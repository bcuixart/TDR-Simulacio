using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Ajustaments : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    [SerializeField] Dropdown resolucioDropdown;

    [SerializeField] Slider generalSlider;
    [SerializeField] Slider uiSlider;
    [SerializeField] Slider animalSlider;
    [SerializeField] Slider ambientSlider;

    Resolution[] res;

    void Start()
    {
        res = Screen.resolutions;

        resolucioDropdown.ClearOptions();

        List<string> opcions = new List<string>();

        int currIndex = 0;
        for (int i = 0; i < res.Length; i++)
        {
            string opcio = res[i].width + "x" + res[i].height + " @ " + res[i].refreshRate + " Hz";
            opcions.Add(opcio);

            if(res[i].width == Screen.width && res[i].height == Screen.height)
            {
                currIndex = i;
            }
        }

        resolucioDropdown.AddOptions(opcions);

        resolucioDropdown.value = currIndex;
        resolucioDropdown.RefreshShownValue();

        generalSlider.value = PlayerPrefs.GetFloat("MasterVol", 0);
        uiSlider.value = PlayerPrefs.GetFloat("UIVol", 0);
        animalSlider.value = PlayerPrefs.GetFloat("AnimalVol", 0);
        ambientSlider.value = PlayerPrefs.GetFloat("AmbientVol", 0);
    }

    public void Resolucio(int index)
    {
        Resolution ress = res[index];
        Screen.SetResolution(ress.width, ress.height, Screen.fullScreen);
    }

    public void Qualitat(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void PantallaCompleta(bool pc)
    {
        Screen.fullScreen = pc;
    }

    public void VolumGeneral(float volum)
    {
        mixer.SetFloat("MasterVol", volum);
        PlayerPrefs.SetFloat("MasterVol", volum);
    }

    public void VolumUI(float volum)
    {
        mixer.SetFloat("UIVol", volum);
        PlayerPrefs.SetFloat("UIVol", volum);
    }

    public void VolumAnimals(float volum)
    {
        mixer.SetFloat("AnimalVol", volum);
        PlayerPrefs.SetFloat("AnimalVol", volum);
    }

    public void VolumAmbient(float volum)
    {
        mixer.SetFloat("AmbientVol", volum);
        PlayerPrefs.SetFloat("AmbientVol", volum);
    }
}
