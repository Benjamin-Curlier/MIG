using UnityEngine;
using System;
using System.Linq;

namespace MIG.Scripts.UI
{
    public class ModalManager : MonoBehaviour
    {
        #region Singleton
        private static ModalManager instance = null;

        private static ModalManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ModalManager>();
                    if (!instance)
                        Debug.Log("DialogManager have to be in the scene to display modal");
                }
                return instance;
            }
        }
        #endregion

        public GameObject PopUpWithInputField;
        public GameObject PopUpTripleButton;

        private Canvas CurrentDynamicCanvas
        {
            get
            {
                var currentUiDynamic = FindObjectsOfType<Canvas>().FirstOrDefault(x => x.name == "UI dynamic");

                if (!currentUiDynamic)
                {
                    Debug.LogError("UI Dynamic should exist in every scene.");
                    return null;
                }

                return currentUiDynamic;
            }
        }

        public void MessageBox(string msg, string inputPlaceholder, string btnText, Action<string> action)
        {
            var newPopup = Instantiate(PopUpWithInputField, CurrentDynamicCanvas.transform);

            var dlg = newPopup.GetComponent<Modal>();

            dlg.Init(msg, inputPlaceholder, btnText, action);
        }

        public void MessageBox(string msg, string btnText, Action firstAction, string secondBtnText, Action secondAction)
        {
            var newPopup = Instantiate(PopUpTripleButton, CurrentDynamicCanvas.transform);

            var dlg = newPopup.GetComponent<Modal>();

            dlg.Init(msg, btnText, firstAction, secondBtnText, secondAction);
        }

        public static void Display(string msg, string inputPlaceholder, string btnText, Action<string> action) 
            => Instance.MessageBox(msg, inputPlaceholder, btnText, action);

        public static void Display(string message, string buttonText, Action firstAction, string secondButtonText, Action secondAction) 
            => Instance.MessageBox(message, buttonText,firstAction, secondButtonText, secondAction);
    }
}