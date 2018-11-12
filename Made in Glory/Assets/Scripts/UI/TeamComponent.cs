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
    public class TeamComponent : MonoBehaviour
    {
        public Button DeleteButton;
        public Button ModifyButton;
        public Text TeamName;
        public Text TeamCost;
        public Text TeamCharactersCount;

        private Team _item;
        private TeamScrollList _teamScrollList;
        private int _index;

        private void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(DeleteButton, "DeleteButton need to be set");
            Assert.IsNotNull(ModifyButton, "ModifyButton need to be set");
            Assert.IsNotNull(TeamName, "TeamName need to be set");
            Assert.IsNotNull(TeamCost, "TeamCost need to be set");
            Assert.IsNotNull(TeamCharactersCount, "TeamCharactersCount need to be set");

            DeleteButton.onClick.AddListener(HandleClickDelete);
            ModifyButton.onClick.AddListener(HandleClickModify);
        }

        private void HandleClickDelete()
        {
            _teamScrollList.DeleteTeam(_index);
        }

        private void HandleClickModify()
        {
            _teamScrollList.DisplayTeamCharacters(_index);
        }
       
        public void Setup(Team currentItem, int currentIndex, TeamScrollList currentTeamScrollList)
        {
            TeamName.text = currentItem.Name;
            TeamCost.text = currentItem.Gold.ToString();
            TeamCharactersCount.text = currentItem.Characters.Count.ToString();

            _item = currentItem;
            _teamScrollList = currentTeamScrollList;
            _index = currentIndex;
        }
    }
}
