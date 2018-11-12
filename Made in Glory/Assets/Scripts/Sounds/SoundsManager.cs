using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIG.Scripts.Sounds
{
    public class SoundsManager : MonoBehaviour
    {
        public List<AudioClip> Musics;

        // Use this for initialization
        void Start()
        {
            SoundsPlayer.Instance.LaunchPlaylist(Musics);
            DestroyImmediate(this.gameObject);
        }
    }
}
