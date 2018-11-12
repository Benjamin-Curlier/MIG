using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class Modal : MonoBehaviour
    {
        public InputField InputField;
        public Text Message;
        public Button Button;
        public Button CancelButton;
        public Text ButtonText;
        public Text PlaceHolderText;
        public Button SecondButton;
        public Text SecondButtonText;

        public void Init(string msg, string placeholder, string btntext, Action<string> action)
        {
            Message.text = msg;
            PlaceHolderText.text = placeholder;
            ButtonText.text = btntext;

            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() =>
            {
                action(InputField.text);
                Destroy(this.gameObject);
            });
            CancelButton.onClick.RemoveAllListeners();
            CancelButton.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
            });
        }

        internal void Init(string msg, string btnText, Action firstAction, string secondBtnText, Action secondAction)
        {
            Message.text = msg;
            ButtonText.text = btnText;
            SecondButtonText.text = secondBtnText;

            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() =>
            {
                firstAction();
                Destroy(this.gameObject);
            });

            SecondButton.onClick.RemoveAllListeners();
            SecondButton.onClick.AddListener(() =>
            {
                secondAction();
                Destroy(this.gameObject);
            });

            CancelButton.onClick.RemoveAllListeners();
            CancelButton.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
            });
        }
    }
}
