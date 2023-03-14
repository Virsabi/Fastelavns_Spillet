using UnityEngine;
using MyBox;
using UnityEngine.UI;
using VirsabiRepoDevelopment.Packages.com.virsabi.Runtime._IN_REVIEW.Unsorted;

/// <summary>
/// I am still in beta - may not be usable without extensions - used in Mærsk.
/// </summary>
namespace Virsabi.DebugTools
{
    [RequireComponent(typeof(CollisionEventTrigger))]
    public class GODebugElement : MonoBehaviour
    {


        [Foldout("Debug Values", true)]

        [ReadOnly]
        public GameObject go;

        [ReadOnly, SerializeField]
        private TMPro.TextMeshProUGUI text;

        [ReadOnly, SerializeField]
        private Image _buttonBackground;

        [ReadOnly, SerializeField]
        private GO_Debugger gO_Debugger;

        /// <summary>
        /// Loads the gameobjects info into UI element
        /// </summary>
        /// <param name="_go"></param>
        public void LoadElement(GameObject _go)
        {
            go = _go;
            _buttonBackground = GetComponentInChildren<Image>();
            gO_Debugger = GetComponentInParent<GO_Debugger>();
            text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

            text.SetText(go.name);

            if (go.activeSelf)
                _buttonBackground.color = gO_Debugger.EnabledColor;
            else
                _buttonBackground.color = gO_Debugger.DisabledColor;
        }


        /// <summary>
        /// Enable/Disables the gameobject and color of button
        /// </summary>
        public void ToggleGameObject()
        {
            go.SetActive(!go.activeSelf);

            if (go.activeSelf)
                _buttonBackground.color = gO_Debugger.EnabledColor;
            else
                _buttonBackground.color = gO_Debugger.DisabledColor;


        }
    }

}