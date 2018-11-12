using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MIG.Scripts.Game.Interfaces;
using MIG.Scripts.Dtos;
using MIG.Scripts.Common.Input;
using System;
using MIG.Scripts.Common.Events;
using MIG.Scripts.PlayerManager;
using MIG.Scripts.Sounds;
using MIG;


public class UI_Manager : MonoBehaviour, INewCharacterTurn, IEndOfGame
{
    PlayerManager playerManager;
    SoundsPlayer sound;

    public ConnectorPhoton CP;
    public CanvasGroup PopUp;
    public CanvasGroup Menu;
    public CanvasGroup GameUI;
    public Button ExitGame;
    public Button Escape;
    public Slider SoundSlider;
    public Button PassTurn;
    public float FadeTime = 0.5f;

    bool ChangingState = false;
    bool InMenu = false;
    bool CanPass = true;

    // Use this for initialization
    void Start ()
    {
        sound = SoundsPlayer.Instance;
        if (sound != null)
            SoundSlider.value = sound.GetSoundsVolume();

        CP.Game.AddToListener(this);
        playerManager = GetComponent<PlayerManager>();
        InputController.EscapeEvent += OnTouchEscape;
        ExitGame.onClick.AddListener(OnExit);
        Escape.onClick.AddListener(OnEscape);
        SoundSlider.onValueChanged.AddListener(OnChangeSoundVolume);
        PassTurn.onClick.AddListener(OnPassTurn);
    }

    private void OnPassTurn()
    {
        if (CP.Game.CurrentPlayer != null && CanPass)
        {
            CanPass = false;

            byte evCode = (byte)ConnectorPhoton.EventCode.EvPassCharacterTurn;
            bool reliable = true;

            PhotonNetwork.RaiseEvent(evCode, null, reliable, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.MasterClient });
        }
    }

    private void OnChangeSoundVolume(float percentage)
    {
        sound.SetSoundsVolume(percentage);
    }

    private void OnExit()
    {
        ExitGame.interactable = false;
        PhotonNetwork.LeaveRoom();
    }

    private void OnDestroy()
    {
        CP.Game.RemoveFromListener(this);
    }

    private void OnTouchEscape(object sender, InfoEventArgs<object> e)
    {
        OnEscape();
    }

    private void OnEscape()
    {
        if(playerManager.state == MIG.Scripts.Character.PlayerState.Idle && ChangingState == false)
        {
            if (InMenu == false)
            {
                StartCoroutine(ChangeDisplay(Menu, GameUI));
            }
            else
            {
                StartCoroutine(ChangeDisplay(GameUI, Menu));
            }

            ChangingState = true;
            InMenu = !InMenu;
        }
    }

    public void EndOfGame()
    {
        StartCoroutine(EndGame());
    }

    public void NewCharacterTurn(NextPlayerDto character)
    {
        if (CP.Game.CurrentPlayer != null)
        {
            CanPass = true;
            StartCoroutine(PopUpDisplay("Your turn"));
        }
        else
        {
            StartCoroutine(PopUpDisplay("Opponent turn"));
        }
    }

    IEnumerator ChangeDisplay(CanvasGroup newActif, CanvasGroup oldActif)
    {
        float timeStart;
        float f;

        newActif.interactable = false;
        newActif.blocksRaycasts = false;

        oldActif.interactable = false;
        oldActif.blocksRaycasts = false;

        timeStart = Time.time;
        f = 0;

        while (f < 1f)
        {
            yield return new WaitForEndOfFrame();
            f = (Time.time - timeStart) / FadeTime;
            newActif.alpha = Mathf.Lerp(0, 1, f);
            oldActif.alpha = Mathf.Lerp(1, 0, f);
        }

        newActif.interactable = true;
        newActif.blocksRaycasts = true;

        ChangingState = false;
    }

    IEnumerator EndGame()
    {
        if (CP.Game.CurrentPlayer != null)
        {
            yield return StartCoroutine(PopUpDisplay("Win"));
        }
        else
        {
            yield return StartCoroutine(PopUpDisplay("Lost"));
        }

        PhotonNetwork.LeaveRoom();
    }

    IEnumerator PopUpDisplay(string text)
    {
        PopUp.GetComponentInChildren<Text>().text = text;
        PopUp.gameObject.SetActive(true);

        float timeStart = Time.time;
        float f = 0;

        while (f < 1f)
        {
            yield return new WaitForEndOfFrame();
            f = (Time.time - timeStart)  / FadeTime;
            PopUp.alpha = Mathf.Lerp(0, 1, f);
        }

        yield return new WaitForSeconds(0.5f);

        timeStart = Time.time;
        f = 0;

        while (f < 1f)
        {
            yield return new WaitForEndOfFrame();
            f = (Time.time - timeStart) / FadeTime;
            PopUp.alpha = Mathf.Lerp(1, 0, f);
        }

        PopUp.gameObject.SetActive(false);
    }

}
