using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] public GameObject broma1;
    [SerializeField] public GameObject broma2;
    [SerializeField] public GameObject broma3;
    [SerializeField] public GameObject broma4;
    [SerializeField] public GameObject broma5;

    bool bromaEstupida;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bromaEstupida = true;
        }
    }

    public void Comencar()
    {
        if (bromaEstupida)
        {
            broma1.SetActive(false);
            broma2.SetActive(true);
            return;
        }
        SceneManager.LoadScene("JSONEspecie");
    }

    public void Comencar2()
    {
        broma2.SetActive(false);
        broma3.SetActive(true);
    }

    public void Comencar3()
    {
        broma3.SetActive(false);
        broma4.SetActive(true);
    }

    public void Comencar4()
    {
        broma4.SetActive(false);
        broma5.SetActive(true);
    }

    public void ComencarBe()
    {
        SceneManager.LoadScene("JSONEspecie");
    }

    public void Sortir()
    {
        Application.Quit();
    }
}
