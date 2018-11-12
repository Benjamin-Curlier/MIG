using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

namespace MIG
{
    public class SceneController : MonoBehaviour
    {
        public ConnectorPhoton ConnectorPhoton;
        public ConnectorPhoton.PhotonState PhotonState;
        public CanvasGroup InteractableCanvas;

        protected virtual void Start()
        {
            if (ConnectorPhoton.State == ConnectorPhoton.PhotonState.NONE)
            {
                InteractableCanvas.interactable = false;
                StartCoroutine( ConnectorPhoton.SetUp(this) );
            }
        }

        public virtual void SetUpDone()
        {
            InteractableCanvas.interactable = true;
        }
    }
}