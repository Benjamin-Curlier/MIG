using MIG.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelSelectionIconSpell : MonoBehaviour
{
    public SpellComponent spellComponent;

    // Use this for initialization
    void Start()
    {
        var sprites = Resources.LoadAll<Sprite>("Sprites/SpellIcons");

        var buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            var imgBtn = buttons[i].GetComponent<Image>();

            imgBtn.sprite = sprites[i];

            buttons[i].onClick.AddListener(() =>
            {
                HandleClickIconSpell();
                this.gameObject.SetActive(false);
            });
        }
    }

    private void HandleClickIconSpell()
    {
        var img = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite.name;
        
        spellComponent.ChangeIconSpell(img);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
