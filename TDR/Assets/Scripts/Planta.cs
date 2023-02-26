using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Planta : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform prefabParent;

    [Space]
    public float tempsMinimReproduccio;
    public float tempsMaximReproduccio;

    [Space]
    public int fillsMinim;
    public int fillsMaxim;

    [Space]
    [SerializeField] LayerMask terraMask;

    public bool posicioAtzarSpawn;

    GameManager gameMana;

    IEnumerator Start()
    {
        gameMana = GameManager.instance;

        if (gameMana.meteorit)
        {
            Destroy(gameObject);
        }

        prefabParent = transform.parent;

        Random.InitState(gameMana.info.randomSeed);

        gameMana.plantes.Add(this);

        yield return new WaitUntil(() => gameMana.comencat);

        if (posicioAtzarSpawn)
        {
            Vector3 pos = TrobarPuntAtzar(Vector3.zero, 20, 0);
            transform.position = pos;

            EnganxarseATerra();

            fillsMinim = 2;
            fillsMaxim = 10;

            Reproduirse();

            yield break;
        }

        EnganxarseATerra();

        float temps = Random.Range(tempsMinimReproduccio, tempsMaximReproduccio);
        Invoke("Reproduirse", temps);
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        gameMana.plantes.Remove(this);
    }

    void Reproduirse()
    {
        if(gameMana.plantes.Count >= gameMana.maximPlantes || gameMana.arribatTempsMaxim)
        {
            return;
        }

        int fills = Random.Range(fillsMinim, fillsMaxim + 1);

        for (int i = 0; i < fills; i++)
        {
            if (gameMana.plantes.Count >= gameMana.maximPlantes)
            {
                return;
            }

            Vector3 pos = TrobarPuntAtzar(transform.position, 20, 0);

            GameObject GO = Instantiate(prefab, pos, Quaternion.identity, prefabParent);
            GO.name = transform.name;

            if (posicioAtzarSpawn)
            {
                GO.GetComponent<Planta>().posicioAtzarSpawn = false;
            }
        }
    }

    public static Vector3 TrobarPuntAtzar(Vector3 origen, float radi, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radi;

        randomDirection += origen;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, radi * 2, NavMesh.GetAreaFromName("Sorra"));

        return navHit.position;
    }

    void EnganxarseATerra()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position + new Vector3(0, 20, 0), -transform.up, out hit, 200, terraMask))
        {
            transform.position = hit.point;
        }
    }
}
