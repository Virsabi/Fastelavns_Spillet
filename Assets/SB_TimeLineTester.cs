using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class SB_TimeLineTester : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 currentDirection;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        this.transform.Rotate(currentDirection);
    }

    [Button]
    public void Spin(int state)
    {
        if (state == 1)
        {
            currentDirection = Vector3.forward*2; //z
        }
        else if (state == 2)
        {
            currentDirection = Vector3.up*2; // y
        }
        else if(state == 3)
        {
            currentDirection = Vector3.right*2; // x
        }
        //currentDirection = directionV;
    }
}
