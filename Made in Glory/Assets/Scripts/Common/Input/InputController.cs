using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MIG.Scripts.Common.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;

namespace MIG.Scripts.Common.Input
{
    public class InputController : MonoBehaviour//Singleton<InputController>
    {
        public static event EventHandler<InfoEventArgs<Vector2>> MoveEvent;
        public static event EventHandler<InfoEventArgs<int>> ShortcutEvent;
        public static event EventHandler<InfoEventArgs<object>> EscapeEvent;
        public static event EventHandler<InfoEventArgs<GameObject>> ClickEvent;
        public static event EventHandler<InfoEventArgs<float>> ScrollEvent;

        private Camera _camera;
        private Camera _uiCamera;
        private Button[] _buttons;

        private void Start()
        {
            _camera = Camera.main;

            if (SceneManager.GetActiveScene().name.Contains("Game"))
            {
                _buttons = FindObjectsOfType<Button>();

                foreach (var button in _buttons)
                {
                    button.onClick.AddListener(() =>
                    {
                        ClickEvent?.Invoke(this, new InfoEventArgs<GameObject>(button.gameObject));
                    });
                }
            }
        }

        private void Update()
        {
            if (ShortcutEvent != null)
            {
                for (int i = 1; i <= 8; i++)
                {
                    var shortcut = string.Format("Shortcut {0}", i);

                    if (InputManager.IsDown(shortcut))
                    {
                        ShortcutEvent(this, new InfoEventArgs<int>(i));
                        break;
                    }
                }
            }

            if (UnityInput.GetKeyUp(KeyCode.Escape) && EscapeEvent != null)
                EscapeEvent(this, new InfoEventArgs<object>(null));

            if ((UnityInput.GetAxis("Horizontal") != 0 || UnityInput.GetAxis("Vertical") != 0) && MoveEvent != null)
            {
                var vectorMovement = new Vector2(UnityInput.GetAxis("Horizontal"), UnityInput.GetAxis("Vertical"));
                MoveEvent(this, new InfoEventArgs<Vector2>(vectorMovement));
            }

            if (UnityInput.GetAxis("Mouse ScrollWheel") != 0f && ScrollEvent != null)
                ScrollEvent(this, new InfoEventArgs<float>(UnityInput.GetAxis("Mouse ScrollWheel")));

            if (InputManager.IsDown("Click") && ClickEvent != null)
            {
                RaycastHit raycastHitInfo;
                var ray = _camera.ScreenPointToRay(UnityInput.mousePosition);

                PointerEventData pointerData = new PointerEventData(EventSystem.current);

                pointerData.position = UnityInput.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                if (results.Count > 0 && results[0].gameObject.layer == LayerMask.NameToLayer("UI"))
                    results.Clear();
                else
                {
                    if (Physics.Raycast(ray, out raycastHitInfo, Mathf.Infinity, LayerMask.GetMask("GameplayElement")))
                        ClickEvent(this, new InfoEventArgs<GameObject>(raycastHitInfo.collider.gameObject));
                }
            }
        }

        private void Awake()
        {
            InputManager.Load();
        }
    }
}
