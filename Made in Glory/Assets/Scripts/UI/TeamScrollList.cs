using MIG.Scripts.Character;
using MIG.Scripts.Common;
using MIG.Scripts.Common.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class TeamScrollList : MonoBehaviour
    {
        public ScrollRect ScrollViewTeam;
        public ObjectPool ButtonObjectPoolTeam;

        private List<Team> _teams;

        private void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(ScrollViewTeam, "Should be set");
            RefreshDisplay();
        }

        private void Awake()
        {
            var json = string.Empty;

            string filePath = Path.Combine(Application.streamingAssetsPath + "/PlayerDatas/", "Teams.json");

            if (File.Exists(filePath))
            {
                json = File.ReadAllText(filePath);

                try
                {
                    _teams = JsonHelper.GetJsonArray<Team>(json).ToList();
                }
                catch
                {
                    _teams = new List<Team>();
                }
            }
            else
                _teams = new List<Team>();
        }
        
        public void ReturnLobby()
        {
            SceneManager.LoadScene("Lobby");
        }

        internal void DisplayTeamCharacters(int teamIndex)
        {
            SceneData.SetData("TeamIndex", teamIndex);

            SceneManager.LoadScene("TeamManagement");
        }

        #region Teams
        private void RemoveTeams()
        {
            while (ScrollViewTeam.content.childCount > 0)
            {
                Debug.Log("Remove team");
                GameObject toRemove = ScrollViewTeam.content.GetChild(0).gameObject;
                ButtonObjectPoolTeam.ReturnObject(toRemove);
            }
        }

        private void AddTeams()
        {
            for (int i = 0; i < _teams.Count; i++)
            {
                Team item = _teams[i];
                GameObject newButton = ButtonObjectPoolTeam.GetObject();
                newButton.transform.SetParent(ScrollViewTeam.content, false);

                TeamComponent sampleButton = newButton.GetComponent<TeamComponent>();
                sampleButton.Setup(item, i, this);
            }
        }

        public void DeleteTeam(int index)
        {
            _teams.RemoveAt(index);
            RefreshDisplay();
            SaveTeams();
        }

        public void AddTeam(string name)
        {
            _teams.Add(new Team() { Name = name});
            SaveTeams();
            RefreshDisplay();
        }
        #endregion

        public void SaveTeams()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/PlayerDatas/", "Teams.json");

            string json = JsonHelper.SetJsonArray(_teams.ToArray());

            File.WriteAllText(filePath, json);
        }

        public void RefreshDisplay()
        {
            RemoveTeams();
            AddTeams();
        }
    }
}
