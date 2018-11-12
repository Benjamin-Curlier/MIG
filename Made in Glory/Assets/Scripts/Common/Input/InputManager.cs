using MIG.Scripts.Common.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;

namespace MIG.Scripts.Common.Input
{
    public class EnumFlagAttribute : PropertyAttribute
    {
        public EnumFlagAttribute() { }
    }

    [Serializable]
    public struct MultiControl
    {
        [SerializeField]
        public KeyCode key;
        [SerializeField]
        public string button;
        [SerializeField]
        [EnumFlag]
        public InputType flags;
    }

    public class InputManager
    {
        public static Dictionary<string, MultiControl> Controls;

        public static bool IsDown(string touch)
        {
            return UnityInput.GetKeyDown(Controls[touch].key);
        }

        public static IEnumerator WaitInput(GameObject button, string inputName, GameObject eventSystem, InputType inputTypeWanted)
        {
            yield return new WaitUntil(() => UnityInput.anyKeyDown);

            KeyCode pressed = default(KeyCode);

            try
            {
                pressed = (KeyCode)Enum.Parse(typeof(KeyCode), UnityInput.inputString, true);
            }
            catch (ArgumentException)
            {
                foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (UnityInput.GetKeyDown(keycode))
                    {
                        pressed = keycode;
                        break;
                    }
                }
            }

            var inputTypePressed = default(InputType);
            if (pressed >= KeyCode.JoystickButton0)
                inputTypePressed = InputType.GAMEPAD;
            else
                inputTypePressed = InputType.KEYBOARD;

            if (pressed != KeyCode.Escape && inputTypeWanted == inputTypePressed)
            {
                button.GetComponentInChildren<Text>().text = pressed.ToString();
                var currentControl = Controls[inputName];
                Controls[inputName] = new MultiControl { flags = currentControl.flags, button = currentControl.button, key = pressed };
            }

            eventSystem.SetActive(true);
            Debug.Log("Button pressed " + pressed.ToString());
        }

        public static void Save()
        {
            var mappings = Controls.Select(x => new ControlsMapping { Name = x.Key, Control = x.Value }).ToArray();

            var json = JsonHelper.SetJsonArray<ControlsMapping>(mappings);

            PlayerPrefs.SetString("Keys", json);
            PlayerPrefs.Save();
        }

        public static void Load()
        {
            var json = string.Empty;

            if (!PlayerPrefs.HasKey("Keys"))
            {
                string filePath = Path.Combine(Application.streamingAssetsPath + "/Configuration/", "Input.json");

                if (File.Exists(filePath))
                {
                    json = File.ReadAllText(filePath);
                }
                else throw new FileNotFoundException("Input.json should exist");
            }
            else
            {
                json = PlayerPrefs.GetString("Keys");
            }

            var mappings = JsonHelper.GetJsonArray<ControlsMapping>(json);

            Controls = mappings.ToDictionary(x => x.Name, x => x.Control);
        }

        public static void Reset()
        {
            var json = string.Empty;

            string filePath = Path.Combine(Application.streamingAssetsPath + "/Configuration/", "Input.json");

            if (File.Exists(filePath))
            {
                json = File.ReadAllText(filePath);
            }
            else throw new FileNotFoundException("Input.json should exist");

            var mappings = JsonHelper.GetJsonArray<ControlsMapping>(json);

            Controls = mappings.ToDictionary(x => x.Name, x => x.Control);
        }
    }
}
