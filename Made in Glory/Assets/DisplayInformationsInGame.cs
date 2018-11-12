using MIG;
using MIG.Scripts.Character;
using MIG.Scripts.Dtos;
using MIG.Scripts.Game.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayInformationsInGame : MonoBehaviour, INewCharacterTurn
{
    public Text CharacterName;
    public Image HealthDisplay;
    public Image ManaDisplay;
    public Image IconCharacter;

    public ConnectorPhoton connector;

    private Player _player;

    public void NewCharacterTurn(NextPlayerDto character)
    {
        _player = connector.Game.CurrentPlayer;
        UpdateDisplay();
    }
    
    public void UpdateDisplay()
    {
        var actualPlayer = connector.Game.GetActualPlayer();

        if (_player != null)
            CharacterName.color = Color.blue;
        else
            CharacterName.color = Color.red;

        CharacterName.text = actualPlayer.Name;
        HealthDisplay.fillAmount = actualPlayer.Attributes.Health.FinalValue / actualPlayer.Attributes.Health.BaseValue;
        ManaDisplay.fillAmount = actualPlayer.Attributes.ManaPoints.FinalValue / actualPlayer.Attributes.ManaPoints.BaseValue;
        IconCharacter.sprite = Resources.Load<Sprite>($"Characters/Avatars/{actualPlayer.PrefabName}");
    }

    // Use this for initialization
    void Start()
    {
        connector.Game.ListenersNewCharacterTurn.Add(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
