using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Planta : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform prefabParent;

    [Space]
    [SerializeField] float tempsMinimReproduccio;
    [SerializeField] float tempsMaximReproduccio;

    [Space]
    [SerializeField] int fillsMinim;
    [SerializeField] int fillsMaxim;

    void Start()
    {
        float temps = Random.Range(tempsMinimReproduccio, tempsMaximReproduccio);
        InvokeRepeating("Reproduirse", temps, temps);

        GameManager.instance.plantes.Add(this);
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        GameManager.instance.plantes.Remove(this);
    }

    void Reproduirse()
    {
        if(GameManager.instance.plantes.Count >= 100)
        {
            return;
        }

        int fills = Random.Range(fillsMinim, fillsMaxim + 1);

        for (int i = 0; i < fills; i++)
        {
            Vector3 pos = TrobarPuntAtzar(transform.position, 20, -1);

            GameObject GO = Instantiate(prefab, pos, Quaternion.identity, prefabParent);
            GO.name = transform.name;

            GO.transform.position = new Vector3(GO.transform.position.x, transform.position.y, GO.transform.position.z);
        }
    }

    public static Vector3 TrobarPuntAtzar(Vector3 origen, float radi, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radi;

        randomDirection += origen;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, radi, layermask);

        return navHit.position;
    }
}
