using MIG.Scripts.Character;
using MIG.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class InformationsComponent : MonoBehaviour
    {
        public CharacteristicComponent Health;
        public CharacteristicComponent Armor;
        public CharacteristicComponent ActionPoints;
        public CharacteristicComponent ManaPoints;
        public Text CharacterName;
        public Button ModelSelection;

        public PanelSelectionModel SelectionModel;

        private CharacterPage characterPage;
        private Player _player;

        public void Init(Player character, CharacterPage characterPage)
        {
            _player = character;
            ModelSelection.onClick.AddListener(HandleClickSelectionIcon);

            CharacterName.text = character.Name;

            Health.Init(character.Attributes.Health, characterPage, 2);
            Armor.Init(character.Attributes.Armor, characterPage, 2);
            ManaPoints.Init(character.Attributes.ManaPoints, characterPage, 2);
            ActionPoints.Init(character.Attributes.ActionPoints, characterPage, 10);

            this.characterPage = characterPage;

            SelectionModel.Init(character, characterPage, this);
            UpdateIconPlayer();
        }

        public void UpdateIconPlayer()
        {
            var avatar = Resources.Load<Sprite>(string.Format("Characters/Avatars/{0}", _player.PrefabName));

            var img = ModelSelection.GetComponent<Image>();
            img.sprite = avatar;
        }

        private void HandleClickSelectionIcon()
        {
            SelectionModel.gameObject.SetActive(true);
            SelectionModel.DisplayPanel();
        }
    }
}
