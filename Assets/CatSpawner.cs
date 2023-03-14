using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatSpawner : MonoBehaviour
{

    [SerializeField] GameObject cat;
    [SerializeField] ParticleSystem ps;
    [SerializeField] AudioSource audioSource;

    [SerializeField] private int numberOfCats = 100;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnCats()
    {
        for (int i = 0; i < numberOfCats; i++)
        {
            Instantiate(cat, transform.position, Random.rotation);
        }
        ps.Play();
        audioSource.Play();
    }

}
