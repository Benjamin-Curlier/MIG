using MIG.Scripts.Character;
using MIG.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHealDamage : MonoBehaviour
{
    public Toggle Damage;
    public Toggle Heal;

    private Spell _currentItem;
    private CharacterPage _currentPage;

    void Start()
    {
        Heal.onValueChanged.AddListener(changeHealValue);
        Damage.onValueChanged.AddListener(changeDamageValue);
    }

    private void changeHealValue(bool arg0)
    {
        Damage.isOn = !arg0;

        if (arg0)
            _currentItem.Attributes.Effect = EffectType.Heal;

        _currentPage.SaveTeams();
    }

    private void changeDamageValue(bool arg0)
    {
        Heal.isOn = !arg0;

        if (arg0)
            _currentItem.Attributes.Effect = EffectType.Damage;

        _currentPage.SaveTeams();
    }

    public void Setup(Spell currentItem, CharacterPage currentCharacterPage)
    {
        _currentItem = currentItem;
        _currentPage = currentCharacterPage;

        if (currentItem.Attributes.Effect == EffectType.Heal)
        {
            Heal.isOn = true;
            Damage.isOn = false;
        }
        else
        {
            Damage.isOn = true;
            Heal.isOn = false;
        }
    }
}
