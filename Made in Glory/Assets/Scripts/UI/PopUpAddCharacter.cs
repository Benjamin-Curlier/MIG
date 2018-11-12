using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MIG.Scripts.UI
{
    public class PopUpAddCharacter : MonoBehaviour
    {
        public GameObject PopUpNewCharacter;
        public Button NewCharacterButton;
        public Button TemplatesButton;

        public void Start()
        {
            Assert.raiseExceptions = true;
            Assert.IsNotNull(PopUpNewCharacter, "PopUpNewCharacter need to be set");
            Assert.IsNotNull(NewCharacterButton, "NewCharacterButton need to be set");
            Assert.IsNotNull(TemplatesButton, "TemplatesButton need to be set");

            NewCharacterButton.onClick.AddListener(HandleClickNewListener);
        }

        private void HandleClickNewListener()
        {
            var pop = Instantiate(PopUpNewCharacter);
        }
    }
}
