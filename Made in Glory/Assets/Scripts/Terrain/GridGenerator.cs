using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MIG.Scripts.Common;
using System;
using MIG.Scripts.Common.Events;
using MIG.Scripts.Game.Interfaces;
using MIG.Scripts.Dtos;
using Assets.Scripts.Managers.Dtos;
using UnityInput = UnityEngine.Input;
using MIG.Scripts.Character;

namespace MIG.Scripts.Terrain
{

    public class GridGenerator : Singleton<GridGenerator>, INewCharacterTurn
    {
        [Header("the cell prefab")]
        public GameObject CellPrefab;

        [Header("The layers creating cells")]
        public LayerMask CellLayer;

        [Header("The layers NOT creating cells")]
        public LayerMask NotCellLayer;

        [Header("Cell variables")]
        [Tooltip("The in editor size of the cells")]
        public float CellSize = 1;

        [Tooltip("Width of the grid, in number of cells")]
        public int GridWidth = 50;

        [Tooltip("Height of the grid, in number of cells")]
        public int GridHeight = 50;

        [Tooltip("Y displacement of the cells compared to the ground")]
        public float yOffset = 0.1f;

        [Tooltip("Speed use for moving the character")]
        public float SpeedCharacterMovement = 2f;

        public ConnectorPhoton ConnectorPhoton;

        private PlayerState _playerState;
        private int _playerPreparedAttack;

        List<Cell> Cells;

        public class Cell
        {
            public int x;
            public int y;

            public GameObject cellGameObject;

            public Vector2Int Position { get; set; }
            public CellState State { get; set; }

            public Cell(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            internal void ChangeColor(Color newColor)
            {
                var mesh = cellGameObject.GetComponent<MeshRenderer>();
                mesh.material.color = newColor;
            }
            // Neighbours & shit for pathfinding & stuff
        }

        void Start()
        {
            Cells = new List<Cell>();

            GenerateGrid();

            ConnectorPhoton.Game.AddToListener(this);

            PlayerManager.PlayerManager.PlayerStateChanged += PlayerManager_PlayerStateChanged;
        }

        private void PlayerManager_PlayerStateChanged(object sender, InfoEventArgs<NewStatePlayerDto> e)
        {
            ResetColorOnAllCells();

            _playerState = e.Info.state;
            _playerPreparedAttack = e.Info.indexSpell;

            RaycastHit raycastHitInfo;
            var ray = Camera.main.ScreenPointToRay(UnityInput.mousePosition);

            if (Physics.Raycast(ray, out raycastHitInfo, Mathf.Infinity, LayerMask.GetMask("GameplayElement")))
            {
                if (!raycastHitInfo.collider.gameObject.name.Contains("Grid Cell")) return;

                if (_playerState == PlayerState.Idle)
                {
                    DisplayWayToCell(raycastHitInfo.collider.gameObject);
                }
                else if (_playerState == PlayerState.IsAttacking)
                {
                    DisplaySpell(raycastHitInfo.collider.gameObject);
                }
            }
        }

        void GenerateGrid()
        {
            for (int z = 0; z < GridHeight; z++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    RaycastHit hit;

                    if (CheckPosition(x, z, out hit))
                    {
                        Cell cell = CreateCell(x, z, hit);

                        Cells.Add(cell);
                    }
                }
            }
        }

        internal void ChangeStateCell(Vector2Int positionOnGrid, CellState newState)
        {
            var cell = GetCell(positionOnGrid);
            cell.State = newState;
        }

        bool CheckPosition(int x, int z, out RaycastHit HitInfo)
        {
            Vector3 Origin = new Vector3(x * CellSize, 10000, z * CellSize); // We'll ASSUME for now that you can't have terrain above 10 kilometers

            if (Physics.Raycast(Origin, Vector3.down, out HitInfo, Mathf.Infinity, NotCellLayer))
            {
                Debug.DrawRay(Origin, Vector3.down * 50000, Color.red, 100.0f); // Hit not cell terrain, red
                return false;
            }

            if (Physics.Raycast(Origin, Vector3.down, out HitInfo, Mathf.Infinity, CellLayer))
            {
                Debug.DrawRay(Origin, Vector3.down * 50000, Color.green, 100.0f); // Hit cell terrain, green
                return true;
            }

            //Debug.DrawRay(Origin, Vector3.down * 50000, Color.blue, 100.0f); // Didn't hit shit, blue
            return false;
        }

        List<Cell> SpawnCellsTeam1 = new List<Cell>();
        List<Cell> SpawnCellsTeam2 = new List<Cell>();

        Cell CreateCell(int x, int z, RaycastHit hit)
        {
            Cell cell = new Cell(x, z);
            GameObject cellGameObject = Instantiate(CellPrefab);
            Vector3 position = hit.collider.transform.position;

            position.y = hit.point.y + yOffset;
            cellGameObject.transform.position = position;

            BoxCollider coll = hit.collider.GetComponent<BoxCollider>();

            Vector3 CellScale = coll.size;
            CellScale.Scale(coll.transform.localScale);
            cellGameObject.transform.localScale = CellScale;

            cellGameObject.transform.parent = transform;
            cellGameObject.name = $"Grid Cell {x} {z}";
            cellGameObject.layer = LayerMask.NameToLayer("GameplayElement");

            GridCell gridCell = cellGameObject.AddComponent<GridCell>();

            gridCell.HoveredTileEvent += GridCell_HoveredTileEvent;
            gridCell.ExitHoverTileEvent += GridCell_ExitHoverTileEvent;

            cell.Position = new Vector2Int(x, z);
            cell.cellGameObject = cellGameObject;

            // TODO : support more than 2 teams
            switch (hit.collider.tag)
            {
                case "SpawnCell1":
                    SpawnCellsTeam1.Add(cell);
                    break;

                case "SpawnCell2":
                    SpawnCellsTeam2.Add(cell);
                    break;

                default:
                   break;
            }

            return cell;
        }

        List<Cell> AlreadyUsedFreeCells = new List<Cell>();

        /// <summary>
        /// Use this to get the index of a free spawn cell for the team passed in parameter. Will return -1 in case of incorrect input or lack of available cells
        /// </summary>
        /// <param name="team">The team number</param>
        /// <returns>The index of a free cell</returns>
        public int GetFreeCellIndex(int team = 0)
        {
            switch (team)
            {
                // Should probably never be called, here for backward compatibility
                case 0:
                    return GetFreeCellIndexFromList(Cells);

                case 1:
                    return GetFreeCellIndexFromList(SpawnCellsTeam1);
 
                case 2:
                    return GetFreeCellIndexFromList(SpawnCellsTeam2);

                default:
                    break;
            }

            return -1;
        }

        int GetFreeCellIndexFromList(List<Cell> list)
        {
            List<Cell> FreeCells = new List<Cell>(list);
            FreeCells = FreeCells.Except(AlreadyUsedFreeCells).ToList();

            if (FreeCells.Count == 0)
            {
                Debug.LogError("Couldn't find a free cell to spawn player, aborting");
                return -1;
            }

            Cell ChosenCell = FreeCells[UnityEngine.Random.Range(0, FreeCells.Count)];
            AlreadyUsedFreeCells.Add(ChosenCell);

            Debug.Log("Index of chosen cell : " + Cells.IndexOf(ChosenCell) + "/ name : " + ChosenCell.cellGameObject.name);
            return Cells.IndexOf(ChosenCell);
        }

        private void GridCell_ExitHoverTileEvent(object sender, InfoEventArgs<GameObject> e)
        {
            var player = ConnectorPhoton.Game.CurrentPlayer;

            if (player != null)
            {
                ResetColorOnAllCells();
            }
        }

        private void GridCell_HoveredTileEvent(object sender, InfoEventArgs<GameObject> e)
        {
            if (_playerState == PlayerState.Idle)
                DisplayWayToCell(e.Info);
            else if (_playerState == PlayerState.IsAttacking)
                DisplaySpell(e.Info);
        }

        // TODO : Have game pause during animations
        public Vector3 MovePlayerOnGrid(Vector2Int basePosition, Vector2Int newPosition, GameObject player, Stack<Vector2Int> path = null)
        {
            if (path == null)
            {
                // A null path means we got the command from photon. The path was evaluated on the master client and thus, we can safely assume it will exist.
                path = ComputeAStar(basePosition, newPosition);
            }

            ResetColorOnAllCells();

            StopAllCoroutines(); // Stop all old movements. Will fuck things up if you guys add other coroutines.
            StartCoroutine(PlayerMovementCoroutine(player, path));

            return GetCell(newPosition).cellGameObject.transform.position;
        }

        // Only move up/down when you can so it's a bit prettier, but it'll do for now
        IEnumerator PlayerMovementCoroutine(GameObject player, Stack<Vector2Int> path)
        {
            Cell CurrentTargetCell;
            var animation = player.GetComponent<CharacterAnimationManager>();
            animation.Run();

            foreach (Vector2Int CellPos in path)
            {
                CurrentTargetCell = GetCell(CellPos);
                Vector3 basePlayerPosition = player.transform.position;
                player.transform.LookAt(CurrentTargetCell.cellGameObject.transform);
                float acc = 0.0f;

                while (player.transform.position != CurrentTargetCell.cellGameObject.transform.position)
                {
                    acc += Time.deltaTime * SpeedCharacterMovement; // MAGIC ! Also we're going F A S T !
                    player.transform.position = Vector3.Lerp(basePlayerPosition, CurrentTargetCell.cellGameObject.transform.position, acc);
                    yield return new WaitForFixedUpdate();
                }
            }

            animation.Idle();
        }

        public Cell GetCell(Vector2Int position) => Cells.FirstOrDefault(cell => cell.Position.x == position.x && cell.Position.y == position.y);

        public Cell GetCell(GameObject target) => Cells.FirstOrDefault(cell => cell.cellGameObject == target);

        public Vector2 GetPositionFromCell(GameObject target)
        {
            Cell cell = GetCell(target);

            if (cell != null)
            {
                return new Vector2(cell.x, cell.y);
            }

            Debug.LogError("GetPositionFromCell isn't used correctly pls fix");
            return new Vector2();
        }

        public Vector3 GetCellFomIndex(int index)
        {
            return Cells[index].cellGameObject.transform.position; // idk ask simon
        }

        public Vector2Int GetGridPositionFromIndex(int index)
        {
            Cell cell = Cells[index];

            return new Vector2Int(cell.x, cell.y);
        }

        public Stack<Vector2Int> ComputeAStar(Vector2Int from, Vector2Int to)
        {
            matrixNode endNode = AStar(from.x, from.y, to.x, to.y);

            Stack<Vector2Int> path = new Stack<Vector2Int>();

            while (endNode.x != from.x || endNode.y != from.y)
            {
                path.Push(new Vector2Int(endNode.x, endNode.y));
                endNode = endNode.parent;
            }
            path.Push(new Vector2Int(endNode.x, endNode.y));

            #region DEBUG_DISPLAY
            //Debug.Log("The shortest path from  " +
            //                  "(" + from.x + "," + from.y + ")  to " +
            //                  "(" + to.x + "," + to.y + ")  is:  \n");

            //while (path.Count > 0)
            //{
            //    matrixNode node = path.Pop();
            //    Debug.Log("(" + node.x + "," + node.y + ")");
            //}
            #endregion

            return path;
        }

        public class matrixNode
        {
            public int fr = 0, to = 0, sum = 0;
            public int x, y;
            public matrixNode parent;
        }

        public matrixNode AStar(int fromX, int fromY, int toX, int toY)
        {
            //Debug.Log($"From x : {fromX} y : {fromY} To x : {toX} y : {toY}");
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // in this version an element in a matrix can move left/up/right/down in one step, two steps for a diagonal move.
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //the keys for greens and reds are x.ToString() + y.ToString() of the matrixNode 
            Dictionary<string, matrixNode> greens = new Dictionary<string, matrixNode>(); //open 
            Dictionary<string, matrixNode> reds = new Dictionary<string, matrixNode>(); //closed 

            matrixNode startNode = new matrixNode { x = fromX, y = fromY };
            string key = startNode.x.ToString() + startNode.y.ToString();
            greens.Add(key, startNode);

            Func<KeyValuePair<string, matrixNode>> smallestGreen = () =>
            {
                KeyValuePair<string, matrixNode> smallest = greens.ElementAt(0);

                foreach (KeyValuePair<string, matrixNode> item in greens)
                {
                    if (item.Value.sum < smallest.Value.sum)
                        smallest = item;
                    else if (item.Value.sum == smallest.Value.sum
                            && item.Value.to < smallest.Value.to)
                        smallest = item;
                }

                return smallest;
            };


            //add these values to current node's x and y values to get the left/up/right/bottom neighbors
            List<KeyValuePair<int, int>> fourNeighbors = new List<KeyValuePair<int, int>>()
                                            { new KeyValuePair<int, int>(-1,0),
                                              new KeyValuePair<int, int>(0,1),
                                              new KeyValuePair<int, int>(1, 0),
                                              new KeyValuePair<int, int>(0,-1) };

            while (true)
            {
                if (greens.Count == 0)
                    return null;

                KeyValuePair<string, matrixNode> current = smallestGreen();
                if (current.Value.x == toX && current.Value.y == toY)
                    return current.Value;

                greens.Remove(current.Key);
                reds.Add(current.Key, current.Value);

                foreach (KeyValuePair<int, int> plusXY in fourNeighbors)
                {
                    int nbrX = current.Value.x + plusXY.Key;
                    int nbrY = current.Value.y + plusXY.Value;
                    string nbrKey = nbrX.ToString() + nbrY.ToString();
                    if (nbrX < 0 || nbrY < 0 || nbrX >= GridWidth || nbrY >= GridHeight
                        || GetCell(new Vector2Int(nbrX, nbrY)) == null
                        || reds.ContainsKey(nbrKey))
                        continue;

                    if (greens.ContainsKey(nbrKey))
                    {
                        matrixNode curNbr = greens[nbrKey];
                        int from = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                        if (from < curNbr.fr)
                        {
                            curNbr.fr = from;
                            curNbr.sum = curNbr.fr + curNbr.to;
                            curNbr.parent = current.Value;
                        }
                    }
                    else
                    {
                        matrixNode curNbr = new matrixNode { x = nbrX, y = nbrY };
                        curNbr.fr = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                        curNbr.to = Math.Abs(nbrX - toX) + Math.Abs(nbrY - toY);
                        curNbr.sum = curNbr.fr + curNbr.to;
                        curNbr.parent = current.Value;
                        greens.Add(nbrKey, curNbr);
                    }
                }
            }
        }

        public void NewCharacterTurn(NextPlayerDto character)
        {
            ResetColorOnAllCells();
        }

        private void ResetColorOnAllCells()
        {
            foreach (var cell in Cells)
                cell.ChangeColor(Color.white);
        }

        private void DisplayWayToCell(GameObject targetCell)
        {
            var player = ConnectorPhoton.Game.CurrentPlayer;

            if (player != null)
            {
                var cell = GetCell(targetCell);

                if (_playerState == PlayerState.Idle)
                {
                    if (cell.State == CellState.Occupied)
                        return;

                    var cellToWalk = ComputeAStar(player.PositionOnGrid, cell.Position);

                    var colorToDisplay = Color.green;

                    if (cellToWalk.Count - 1 > player.Attributes.ActionPoints.FinalValue)
                        colorToDisplay = Color.red;

                    while (cellToWalk.Count > 0)
                    {
                        var posToWalk = cellToWalk.Pop();
                        var walkingCell = GetCell(posToWalk);

                        if (walkingCell == null)
                            Debug.Log($"Cell to walk position {posToWalk}");

                        walkingCell.ChangeColor(colorToDisplay);
                    }
                }
            }
        }

        private void DisplaySpell(GameObject targetGameobject)
        {
            var player = ConnectorPhoton.Game.CurrentPlayer;

            if (player == null) return;

            var targetCell = GetCell(targetGameobject);

            var distance = Vector2Int.Distance(targetCell.Position, player.PositionOnGrid);

            var colorToDisplay = distance > player.Spells[_playerPreparedAttack].Attributes.Range.FinalValue ? Color.red : Color.green;

            var colliders = Physics.OverlapSphere(targetGameobject.transform.position, player.Spells[_playerPreparedAttack].Attributes.Radius.FinalValue, LayerMask.GetMask("GameplayElement"))
                                    .Where(x => x.gameObject.name.Contains("Grid Cell"));

            foreach (var collider in colliders)
            {
                var target = GetCell(collider.gameObject);
                target.ChangeColor(colorToDisplay);
            }
        }
    }
}
