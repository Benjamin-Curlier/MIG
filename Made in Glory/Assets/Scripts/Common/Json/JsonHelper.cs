using MIG.Scripts.Common.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MIG.Scripts.Common.Json
{
    [Serializable]
    public struct ControlsMapping
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public MultiControl Control;
    }

    public class JsonHelper
    {
        public static T[] GetJsonArray<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.array;
        }

        public static string SetJsonArray<T>(T[] array)
        {
            var wrapper = new Wrapper<T>(array);
            return JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public Wrapper(T[] array)
            {
                this.array = array;
            }

            public T[] array;
        }
    }
}
