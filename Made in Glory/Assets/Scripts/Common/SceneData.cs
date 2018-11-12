using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MIG.Scripts.Common
{
    public static class SceneData
    {
        private static Dictionary<string, object> _datas = new Dictionary<string, object>();

        public static void SetData(string key, object data)
        {
            if (!_datas.ContainsKey(key))
                _datas[key] = data;
        }

        public static T GetData<T>(string key, T defaultValue)
        {
            if (_datas.ContainsKey(key))
            {
                var valueInDictionary = _datas[key];

                if (valueInDictionary is T)
                    return (T)valueInDictionary;
            }

            return defaultValue;
        }

        public static void ClearAllData() => _datas.Clear();
        public static void DeleteValue(string key) => _datas.Remove(key);
    }
}
