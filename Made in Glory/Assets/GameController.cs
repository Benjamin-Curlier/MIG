using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIG
{
    public class GameController : SceneController
    {

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            SendEventReadyForGame();
            if (PhotonNetwork.isMasterClient)
                StartCoroutine(ConnectorPhoton.Game.GameUpdate());
        }

        private void SendEventReadyForGame()
        {
            byte evCode = (byte)ConnectorPhoton.EventCode.EvReadyForGame;
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, null, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.MasterClient });
        }
    }
}