using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virsabi;

public class TestLoadNextScene : MonoBehaviour
{   
    [SerializeField, ReadOnly]
    private SceneChanger sceneChanger;

    private void OnValidate()
    {
        sceneChanger = GetComponent<SceneChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sceneChanger.GoToNextSceneUsingLoadingScreen();
        }
    }
}
