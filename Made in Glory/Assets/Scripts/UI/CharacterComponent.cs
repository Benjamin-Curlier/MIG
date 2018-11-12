using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class CharacterComponent : MonoBehaviour
    { 
        public Text CharacterName;
        public Text CoinText;
        public Text SpellsText;

        public Button DeleteButton;

        private Button Button;
        private Character.Player item;
        private CharacterPage characterPage;
        private int index;

        private void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(CharacterName, "ChraacterName need to be set");
            Assert.IsNotNull(CoinText, "CoinText need to be set");
            Assert.IsNotNull(SpellsText, "SpellsText need to be set");

            Button = this.GetComponent<Button>();

            Button.onClick.AddListener(HandleClick);
            DeleteButton.onClick.AddListener(HandleClickDelete);
        }

        private void HandleClick() => characterPage.SetSelectedCharacterIndex(index);

        private void HandleClickDelete() => characterPage.DeleteCharacter(index);

        public void Setup(Character.Player currentItem, int currentIndex, CharacterPage currentCharacterPage, bool isSelected = false)
        {
            CharacterName.text = currentItem.Name;
            CoinText.text = ((new System.Random()).Next(400)).ToString();
            SpellsText.text = currentItem.Spells.Count.ToString();

            characterPage = currentCharacterPage;
            item = currentItem;
            index = currentIndex;
        }
    }
}
