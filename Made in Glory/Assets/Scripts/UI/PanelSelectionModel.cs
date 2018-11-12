using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MIG.Scripts.Character;
using MIG.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using MIG.Scripts.Common.Extensions;

public class PanelSelectionModel : MonoBehaviour
{
    public Button NextCharacter;
    public Button PreviousCharacter;
    public Button Confirm;
    public Button Cancel;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;

    private GameObject _display;
    private GameObject[] _availableModels;
    private Player _character;
    private CharacterPage _characterPage;
    private InformationsComponent _informationsComponent;
    private int _indexCharacter;

    public void Start()
    {
        NextCharacter.onClick.AddListener(HandleClickNextCharacter);
        PreviousCharacter.onClick.AddListener(HandleClickPreviousCharacter);
        Confirm.onClick.AddListener(HandleClickConfirm);
        Cancel.onClick.AddListener(HandleClickCancel);
    }

    private void HandleClickCancel()
    {
        gameObject.SetActive(false);
    }

    private void HandleClickConfirm()
    {
        _character.PrefabName = _availableModels.ElementAt(_indexCharacter).name;
        _characterPage.SaveTeams();

        _informationsComponent.UpdateIconPlayer();

        gameObject.SetActive(false);
    }

    private void HandleClickPreviousCharacter()
    {
        _indexCharacter--;
        if (_indexCharacter < 0)
            _indexCharacter = _availableModels.Count() - 1;

        DisplayCharacter();
    }

    private void HandleClickNextCharacter()
    {
        _indexCharacter++;
        if (_indexCharacter >= _availableModels.Count())
            _indexCharacter = 0;

        DisplayCharacter();
    }

    private void DisplayCharacter()
    {
        DestroyImmediate(_display);
        _display = Instantiate(_availableModels[_indexCharacter], transform);
        _display.transform.localPosition = Position;
        _display.transform.rotation = Quaternion.Euler(Rotation);
        _display.transform.localScale = Scale;
        _display.transform.ChangeLayersRecursively("UI");
    }

    public void DisplayPanel()
    {
        gameObject.SetActive(true);
        DisplayCharacter();
    }

    internal void Init(Player character, CharacterPage characterPage, InformationsComponent informationsComponent)
    {
        _character = character;
        _characterPage = characterPage;
        _informationsComponent = informationsComponent;
        _availableModels = Resources.LoadAll<GameObject>("Characters/Prefabs");

        _indexCharacter = Array.IndexOf(_availableModels, _availableModels.FirstOrDefault(x => x.name == _character.PrefabName));

        if (_indexCharacter == -1)
            _indexCharacter = 0;
    }
}
