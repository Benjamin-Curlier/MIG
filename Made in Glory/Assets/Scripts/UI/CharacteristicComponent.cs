using MIG.Scripts.Attributes;
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
    public class CharacteristicComponent : MonoBehaviour
    {
        public Button MinusButton;
        public Button PlusButton;
        public Text ValueText;

        private int _goldCost;
        private Characteristic _item;
        private CharacterPage _characterPage;

        private void Start()
        {
            MinusButton.onClick.AddListener(HandleClickMinus);
            PlusButton.onClick.AddListener(HandleClickPlus);
        }

        private void HandleClickPlus()
        {
            if (_characterPage.GetCurrentTeamGold() - _goldCost > 0)
            {
                var updatedValue = true;

                _item.BaseValue++;

                if (_item.BaseValue > _item.MaxValue)
                {
                    updatedValue = false;
                    _item.BaseValue = _item.MaxValue;
                }

                if (updatedValue)
                    _characterPage.ModifyGold(_goldCost);

                ValueText.text = _item.BaseValue.ToString();
                _characterPage.SaveTeams();
            }
        }

        private void HandleClickMinus()
        {
            if (_characterPage.GetCurrentTeamGold() + _goldCost > 0)
            {
                var updatedValue = true;

                _item.BaseValue--;

                if (_item.BaseValue < _item.MinValue)
                {
                    _item.BaseValue = _item.MinValue;
                    updatedValue = false;
                }

                if (updatedValue)
                    _characterPage.ModifyGold(_goldCost * -1);

                ValueText.text = _item.BaseValue.ToString();
                _characterPage.SaveTeams();
            }
        }

        public void Init(Characteristic characteristic, CharacterPage characterPage, int goldCost)
        {
            this._item = characteristic;
            this._characterPage = characterPage;
            _goldCost = goldCost;

            ValueText.text = characteristic.BaseValue.ToString();
        }
    }
}
