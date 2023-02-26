using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividuAnimEvents : MonoBehaviour
{
    [SerializeField] AudioClip[] sorollsNormal;
    [SerializeField] AudioClip[] sorollsNormalSubnormal;

    [SerializeField] AudioClip[] sorollsCaminar;
    [SerializeField] AudioClip[] sorollsCaminarSubnormal;

    [SerializeField] AudioClip[] sorollsMenjar;
    [SerializeField] AudioClip[] sorollsMenjarSubnormal;

    [SerializeField] AudioClip[] sorollsCopular;
    [SerializeField] AudioClip[] sorollsCopularSubnormal;

    [SerializeField] AudioClip sorollInfectat;
    [SerializeField] AudioClip sorollInfectatSubnormal;

    [SerializeField] AudioSource audioS;

    bool subnormal;

    void Start()
    {
        subnormal = Menu.subnormal;

        Random.InitState(GameManager.instance.info.randomSeed);
    }

    public void SorollNormal()
    {
        if (subnormal)
        {
            int r = Random.Range(0, sorollsNormalSubnormal.Length);
            audioS.PlayOneShot(sorollsNormalSubnormal[r]);
        }
        else
        {
            int r = Random.Range(0, sorollsNormal.Length);
            audioS.PlayOneShot(sorollsNormal[r]);
        }
    }

    public void SorollCaminar()
    {
        if (subnormal)
        {
            int r = Random.Range(0, sorollsCaminarSubnormal.Length);
            audioS.PlayOneShot(sorollsCaminarSubnormal[r]);
        }
        else
        {
            int r = Random.Range(0, sorollsCaminar.Length);
            audioS.PlayOneShot(sorollsCaminar[r]);
        }
    }

    public void SorollMenjar()
    {
        if (subnormal)
        {
            int r = Random.Range(0, sorollsMenjarSubnormal.Length);
            audioS.PlayOneShot(sorollsMenjarSubnormal[r]);
        }
        else
        {
            int r = Random.Range(0, sorollsMenjar.Length);
            audioS.PlayOneShot(sorollsMenjar[r]);
        }
    }

    public void SorollCopular()
    {
        if (subnormal)
        {
            int r = Random.Range(0, sorollsCopularSubnormal.Length);
            audioS.PlayOneShot(sorollsCopularSubnormal[r]);
        }
        else
        {
            int r = Random.Range(0, sorollsCopular.Length);
            audioS.PlayOneShot(sorollsCopular[r]);
        }
    }

    public void SorollInfectat()
    {
        audioS.PlayOneShot(subnormal ? sorollInfectatSubnormal : sorollInfectat);
    }
}
