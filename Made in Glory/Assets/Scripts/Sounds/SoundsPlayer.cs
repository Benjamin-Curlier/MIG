using MIG.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MIG.Scripts.Sounds
{
    public class SoundsPlayer : Singleton<SoundsPlayer>
    {
        // Update is called once per frame
        #region internal vars
        private List<AudioClip> _playlist;

        private AudioSource _source0;
        private AudioSource _source1;

        private bool cur_is_source0 = true; //is _source0 currently the active AudioSource (plays some sound right now)

        private Coroutine _curSourceFadeRoutine = null;
        private Coroutine _newSourceFadeRoutine = null;
        #endregion

        private const float MAXIMUM_VOLUME = 0.02f;

        private float maxVolume;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            if (_source0 == null || _source1 == null)
            {
                InitAudioSources();
            }

            maxVolume = PlayerPrefs.GetFloat("SoundsVolume", MAXIMUM_VOLUME);

            DontDestroyOnLoad(this.gameObject);
        }

        public void SetSoundsVolume(float volume)
        {
            maxVolume = MAXIMUM_VOLUME * volume;

            if (cur_is_source0)
                _source0.volume = maxVolume;
            else
                _source1.volume = maxVolume;

            PlayerPrefs.SetFloat("SoundsVolume", maxVolume);
        }

        public float GetSoundsVolume()
        {
            return maxVolume / MAXIMUM_VOLUME;
        }

        public void LaunchPlaylist(List<AudioClip> playlist)
        {
            _playlist = playlist;

            var audioToPlay = _playlist.ElementAt(Random.Range(0, _playlist.Count - 1));

            CrossFade(audioToPlay, maxVolume, 3f, 0);
        }

        #region internal functionality
        void Reset()
        {
            if (_source0 == null || _source1 == null)
            {
                InitAudioSources();
            }
        }
        
        //re-establishes references to audio sources on this game object:
        void InitAudioSources()
        {
            //re-connect _source0 and _source1 to the ones in attachedSources[]
            AudioSource[] audioSources = gameObject.GetComponents<AudioSource>();

            if (audioSources.Length != 2)
            {
                Debug.LogError($"{audioSources.Length} found, should only have 2.");
                return;
            }

            _source0 = audioSources[0];
            _source1 = audioSources[1];
        }
        #endregion

        public void CrossFade(AudioClip clipToPlay, float maxVolume, float fadingTime, float delay_before_crossFade = 0)
        {
            StartCoroutine(Fade(clipToPlay, maxVolume, fadingTime, delay_before_crossFade));
        }

        IEnumerator Fade(AudioClip playMe, float maxVolume, float fadingTime, float delay_before_crossFade)
        {
            if (delay_before_crossFade > 0)
            {
                yield return new WaitForSeconds(delay_before_crossFade);
            }
            
            AudioSource curActiveSource, newActiveSource;
            if (cur_is_source0)
            {
                //_source0 is currently playing the most recent AudioClip
                curActiveSource = _source0;
                //so launch on _source1
                newActiveSource = _source1;
            }
            else
            {
                //otherwise, _source1 is currently active
                curActiveSource = _source1;
                //so play on _source0
                newActiveSource = _source0;
            }

            //perform the switching
            newActiveSource.clip = playMe;
            newActiveSource.Play();
            newActiveSource.volume = 0;

            if (_curSourceFadeRoutine != null)
            {
                StopCoroutine(_curSourceFadeRoutine);
            }

            if (_newSourceFadeRoutine != null)
            {
                StopCoroutine(_newSourceFadeRoutine);
            }

            _curSourceFadeRoutine = StartCoroutine(fadeSource(curActiveSource, curActiveSource.volume, 0, fadingTime));
            _newSourceFadeRoutine = StartCoroutine(fadeSource(newActiveSource, newActiveSource.volume, maxVolume, fadingTime));

            cur_is_source0 = !cur_is_source0;

            var audioToPlay = _playlist.ElementAt(Random.Range(0, _playlist.Count - 1));

            //Select next music in the playlist
            StartCoroutine(Fade(audioToPlay, maxVolume, 3f, playMe.length - 3f));

            yield break;
        }


        IEnumerator fadeSource(AudioSource sourceToFade, float startVolume, float endVolume, float duration)
        {
            float startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;

                sourceToFade.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, endVolume, elapsed / duration));

                if (sourceToFade.volume == endVolume)
                {
                    break;
                }

                yield return null;
            }//end while
        }

        //returns false if BOTH sources are not playing and there are no sounds are staged to be played.
        //also returns false if one of the sources is not yet initialized
        public bool isPlaying
        {
            get
            {
                if (_source0 == null || _source1 == null)
                {
                    return false;
                }

                //otherwise, both sources are initialized. See if any is playing:
                return _source0.isPlaying || _source1.isPlaying;
            }//end get
        }
    }
}
