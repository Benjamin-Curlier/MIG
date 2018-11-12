using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using MIG.Scripts.Common.Input;
using MIG.Scripts.Common.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MIG
{

    public class Options : MonoBehaviour
    {
        public ScrollRect ScrollView;
        public GameObject ButtonElement;

        private Transform content;

        private void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(ScrollView, "Scrollview ref is not set");
            Assert.IsNotNull(ButtonElement, "ButtonElement is not set");

            content = ScrollView.content;
            SetUp_MappingInput();
        }

        public void GoBackScene()
        {
            SceneManager.LoadScene("Lobby");
        }

        private void SetUp_MappingInput()
        {
            foreach (var input in  InputManager.Controls)
            {
                var element = Instantiate(ButtonElement, content.transform);

                element.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);

                var name = element.transform.Find("InputName");
                var keyTransform = element.transform.Find("InputKeyCode");
                var gamedPadTransform = element.transform.Find("InputGamepad");
                var textKey = element.transform.Find("InputKeyCode/Text");
                var textPad = element.transform.Find("InputGamepad/Text");

                var keyButton = keyTransform.gameObject.GetComponent<Button>();
                keyButton.interactable = input.Value.flags.HasFlag(InputType.KEYBOARD);
                var padButton = gamedPadTransform.gameObject.GetComponent<Button>();
                padButton.interactable = input.Value.flags.HasFlag(InputType.GAMEPAD);

                name.GetComponent<Text>().text = input.Key;
                textKey.GetComponent<Text>().text = input.Value.key.ToString();
                textPad.GetComponent<Text>().text = input.Value.button;
                var eventSystem = EventSystem.current.gameObject;

                int i = 0;
                foreach (var button in element.GetComponentsInChildren<Button>())
                {
                    button.onClick.AddListener(() =>
                    {
                        eventSystem.SetActive(false);
                        StartCoroutine(InputManager.WaitInput(button.gameObject, input.Key, eventSystem, i != 0 ? InputType.KEYBOARD : InputType.GAMEPAD));
                    });
                    i++;
                }
            }
        }
    }
}
    