using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelHitResolver : MonoBehaviour
{
    public bool canHit;
    public int theLastHit;
    // Start is called before the first frame update
    void Start()
    {
        canHit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (theLastHit == 1)
        {
            print("end game");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Barrel" && canHit)
        {
            canHit = false;
            StartCoroutine(DelayBarrelHitDetection(0.3f));
            print("Colliding");

        }
    }

    private IEnumerator DelayBarrelHitDetection(float waitTime)
    {
        theLastHit = Random.Range(1, 20);
        yield return new WaitForSeconds(waitTime);
        canHit = true;
        print("Last hit was" + theLastHit);
    }
}
