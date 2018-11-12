using MIG.Scripts.Common.Events;
using MIG.Scripts.Common.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIG.Scripts.Terrain
{
    public enum CellState
    {
        Empty = 0,
        Occupied
    }

    public class GridCell : MonoBehaviour
    {
        public event EventHandler<InfoEventArgs<GameObject>> HoveredTileEvent;
        public event EventHandler<InfoEventArgs<GameObject>> ExitHoverTileEvent;

        private void OnMouseEnter()
        {
            HoveredTileEvent?.Invoke(this, new InfoEventArgs<GameObject>(gameObject));
        }

        private void OnMouseExit()
        {
            ExitHoverTileEvent?.Invoke(this, new InfoEventArgs<GameObject>(gameObject));
        }
    }
}
