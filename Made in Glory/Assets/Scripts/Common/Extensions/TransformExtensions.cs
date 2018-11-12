using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIG.Scripts.Common.Extensions
{


    public static class TransformExtensions
    {
        public static void ChangeLayersRecursively(this Transform trans, string name)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(name);
            foreach (Transform child in trans)
            {
                child.ChangeLayersRecursively(name);
            }
        }
    }
}