using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using Virsabi;


/// <summary>
/// I am still in beta - may not be usable without extensions - used in Mærsk.
/// </summary>
namespace Virsabi.DebugTools
{
    public class GO_Debugger : MonoBehaviour
    {
        [Tooltip("List of Gameobjects to toggle")]
        public List<GameObject> gameObjects = new List<GameObject>();

        [Foldout("Setup", true), Help("Reference to the root layout where to spawn the buttons")]
        public Transform gridlayout;

        [MustBeAssigned]
        [Help("The prefab to be spawned for each gameobject in the list.")]
        public GameObject go_ref_element_prefab;



        public Color EnabledColor;
        public Color DisabledColor;


        /// <summary>
        /// Loads the gameobjects from the list and creates a UI button for toggling enabled status
        /// </summary>
        [ButtonMethod]
        public void LoadObjectsIntoCanvas()
        {
            foreach (GODebugElement element in gridlayout.GetComponentsInChildren<GODebugElement>())
            {
                DestroyImmediate(element.gameObject);
            }


            foreach (GameObject go in gameObjects)
            {
                GameObject GOButton = Instantiate(go_ref_element_prefab, gridlayout);

                GOButton.GetComponent<GODebugElement>().LoadElement(go);
            }
        }
    }

}