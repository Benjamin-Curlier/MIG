using MIG.Scripts.UI;
using MIG.Scripts.Character;
using MIG.Scripts.Common;
using MIG.Scripts.Common.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class CharacterPage : MonoBehaviour
    {
        public ScrollRect ScrollViewCharacters;
        public ScrollRect ScrollViewSpells;

        public GameObject PopUpAddCharacter;

        public Canvas DynamicCanvas;

        public ObjectPool ObjectPoolCharacters;
        public ObjectPool ObjectPoolSpells;

        public ModalManager dialogManager;

        public InformationsComponent informationsComponent;

        public Text GoldDisplay;

        private int _indexTeam;
        private int _indexCharacter = -1;

        internal void DeleteSpell(int index)
        {
            if (CurrentCharacter != null)
            {
                CurrentCharacter.Spells.RemoveAt(index);
                ModifyGold(-15);
                DisplayInformationCharacter();
                SaveTeams();
            }
        }

        internal void DeleteCharacter(int indexCharacter)
        {
            ModifyGold(-50);
            CurrentTeam.Characters.RemoveAt(indexCharacter);
            DisplayCharacters();
            DisplayInformationCharacter();
            SaveTeams();
        }

        private List<Team> _teams;

        private Team CurrentTeam
        {
            get
            {
                return _teams[_indexTeam];
            }
        }

        private Player CurrentCharacter
        {
            get
            {
                if (_indexCharacter == -1 || CurrentTeam.Characters.ElementAtOrDefault(_indexCharacter) == null)
                    return null;
                return CurrentTeam.Characters.ElementAt(_indexCharacter);
            }
        }

        public void ModifyGold(int gold)
        {
            CurrentTeam.Gold -= gold;

            GoldDisplay.text = GetCurrentTeamGold().ToString();
        }

        public int GetCurrentTeamGold()
        {
            return CurrentTeam.Gold;
        }

        public void Awake()
        {
            _indexTeam = SceneData.GetData("TeamIndex", 0);

            LoadTeamsFromFile();
        }

        public void Start()
        {
            Assert.raiseExceptions = true;
            foreach (var field in this.GetType().GetFields())
                Assert.IsNotNull(field.GetValue(this), $"{field.Name} need to be set");

            informationsComponent.gameObject.SetActive(false);
            DisplayCharacters();
        }

        internal void SetSelectedCharacterIndex(int index)
        {
            _indexCharacter = index;

            if (CurrentCharacter != null)
                DisplayInformationCharacter();
        }

        private void DisplayInformationCharacter()
        {
            if (CurrentCharacter != null)
            {
                informationsComponent.Init(CurrentCharacter, this);
                informationsComponent.gameObject.SetActive(true);
                RemoveSpells();
                AddSpells();
            }
            else
            {
                informationsComponent.gameObject.SetActive(false);
                RemoveSpells();
            }
        }

        private void AddSpells()
        {
            for (int i = 0; i < CurrentCharacter.Spells.Count; i++)
            {
                var spell = CurrentCharacter.Spells[i];

                GameObject newButton = ObjectPoolSpells.GetObject();
                newButton.transform.SetParent(ScrollViewSpells.content, false);

                SpellComponent sampleButton = newButton.GetComponent<SpellComponent>();
                sampleButton.Setup(spell, i, this);
            }
        }

        private void RemoveSpells()
        {
            while (ScrollViewSpells.content.childCount > 0)
            {
                GameObject toRemove = ScrollViewSpells.content.GetChild(0).gameObject;
                ObjectPoolSpells.ReturnObject(toRemove);
            }
        }

        public void AddSpell()
        {
            if (CurrentCharacter != null)
            {
                ModalManager.Display("Enter the name of your new spell", "Enter name...", "Confirm", (spellName) =>
                {
                    CurrentCharacter.Spells.Add(new Spell()
                    {
                        Name = spellName,
                    });
                    ModifyGold(15);
                    DisplayInformationCharacter();
                    SaveTeams();
                });
            }
        }

        public void ReturnTeamsManagement()
        {
            SceneManager.LoadScene("TeamsManagement");
        }

        private void LoadTeamsFromFile()
        {
            var json = string.Empty;

            string filePath = Path.Combine(Application.streamingAssetsPath + "/PlayerDatas/", "Teams.json");

            if (File.Exists(filePath))
            {
                json = File.ReadAllText(filePath);

                _teams = JsonHelper.GetJsonArray<Team>(json).ToList();
            }
            else
                _teams = new List<Team>();
        }

        public void AddCharacter()
        {
            ModalManager.Display($"Do you want to create a new character {Environment.NewLine} or {Environment.NewLine} use an already created template",
                "Create new character", () =>
                {
                    ModalManager.Display("Please enter the name of your new character", "Enter name...", "Confirm", (name) =>
                    {
                        CurrentTeam.Characters.Add(new Character.Player() { Name = name });
                        ModifyGold(50);
                        DisplayCharacters();
                        SaveTeams();
                    });
                }, "Use template", () =>
                {
                    Debug.Log("Asking for templates");
                });
        }

        private void RemoveCharacters()
        {
            while (ScrollViewCharacters.content.childCount > 0)
            {
                GameObject toRemove = ScrollViewCharacters.content.GetChild(0).gameObject;
                ObjectPoolCharacters.ReturnObject(toRemove);
            }
        }

        private void AddCharacters()
        {
            for (int i = 0; i < CurrentTeam.Characters.Count; i++)
            {
                Player item = CurrentTeam.Characters[i];
                GameObject newButton = ObjectPoolCharacters.GetObject();
                newButton.transform.SetParent(ScrollViewCharacters.content, false);

                CharacterComponent sampleButton = newButton.GetComponent<CharacterComponent>();
                sampleButton.Setup(item, i, this);
            }
        }

        private void DisplayCharacters()
        {
            GoldDisplay.text = GetCurrentTeamGold().ToString();
            RemoveCharacters();
            AddCharacters();
        }

        public void SaveTeams()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/PlayerDatas/", "Teams.json");

            string json = JsonHelper.SetJsonArray(_teams.ToArray());

            File.WriteAllText(filePath, json);
        }
    }
}
