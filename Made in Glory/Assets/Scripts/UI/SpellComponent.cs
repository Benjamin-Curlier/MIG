using MIG.Scripts.UI;
using MIG.Scripts.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class SpellComponent : MonoBehaviour
    {
        public Text SpellName;
        public CharacteristicComponent Damage;
        public CharacteristicComponent Range;
        public CharacteristicComponent Radius;
        public CharacteristicComponent Cooldown;
        public CharacteristicComponent ManaCost;
        public Button DeleteButton;
        public ToggleHealDamage toggle;
        public Button SpellIcon;

        private PanelSelectionIconSpell _panelSelectionIconSpell;

        private Spell _item;
        private CharacterPage _characterPage;
        private int _index;

        private void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(SpellName, "Text need to be set");

            DeleteButton.onClick.AddListener(HandleDeleteClick);
            SpellIcon.onClick.AddListener(HandleClickIcon);

            _panelSelectionIconSpell = Resources.FindObjectsOfTypeAll<PanelSelectionIconSpell>()[0];
        }

        private void HandleClickIcon()
        {
            _panelSelectionIconSpell.gameObject.SetActive(true);
            _panelSelectionIconSpell.spellComponent = this;
        }

        internal void ChangeIconSpell(string iconName)
        {
            var sprite = Resources.Load<Sprite>($"Sprites/SpellIcons/{iconName}");

            _item.Icon = iconName;

            SpellIcon.GetComponent<Image>().sprite = sprite;
            _characterPage.SaveTeams();
        }

        private void HandleDeleteClick() => _characterPage.DeleteSpell(_index);

        public void Setup(Spell currentItem, int currentIndex, CharacterPage currentCharacterPage)
        {
            _item = currentItem;
            _characterPage = currentCharacterPage;
            _index = currentIndex;

            SpellName.text = currentItem.Name;

            Damage.Init(currentItem.Attributes.Damage, currentCharacterPage, 10);
            Range.Init(currentItem.Attributes.Range, currentCharacterPage, 10);
            Radius.Init(currentItem.Attributes.Radius, currentCharacterPage, 10);
            Cooldown.Init(currentItem.Attributes.Cooldown, currentCharacterPage, -10);
            ManaCost.Init(currentItem.Attributes.ManaCost, currentCharacterPage, -10);

            var sprite = Resources.Load<Sprite>($"Sprites/SpellIcons/{currentItem.Icon}");

            if (sprite != null)
                SpellIcon.GetComponent<Image>().sprite = sprite;

            toggle.Setup(currentItem, currentCharacterPage);
        }
    }
}
