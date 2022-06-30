using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Astar
{
    class PathNode
    {
        public GameObject obj;
        public Vector2Int position;

        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public PathNode prevNode;


        public PathNode(GameObject _obj, int _x, int _y)
        {
            this.obj = _obj;
            this.position = new Vector2Int(_x, _y);
            this.gCost = int.MaxValue;
        }
    }

    public class AStar : MonoBehaviour
    {
        [SerializeField] Transform map;

        [SerializeField] Vector2Int startPosiiton;
        [SerializeField] Vector2Int endPosition;

        [SerializeField] Vector2Int mapSize;

        List<PathNode> mapData = new List<PathNode>();
        PathNode lastNode;

        bool[,] closePath;

        const int MOVE_STRAIGHT_COST = 10;
        const int MOVE_DIAGONAL_COST = 14;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            RenwalMapData();
            StartCoroutine(PathFinding());
        }

        private void RenwalMapData()
        {
            closePath = new bool[mapSize.y, mapSize.x];

            int len = map.childCount;

            int width = mapSize.x;

            int x, y;
            bool isWall;
            for (int i = 0; i < len; i++)
            {
                x = i % width;
                y = i / width;
                isWall = map.GetChild(i).CompareTag("Wall");

                if (isWall == true)
                {
                    map.GetChild(i).gameObject.GetComponent<Image>().color = Color.black;
                    AddClosePath(new Vector2Int(x, y));
                }
                mapData.Add(new PathNode(map.GetChild(i).gameObject, x, y));
            }
        }

        public IEnumerator PathFinding()
        {
            List<PathNode> nextPaths = new List<PathNode>();
            List<PathNode> neighbours = new List<PathNode>();
            PathNode neighbour;

            PathNode startNode = GetPathNode(startPosiiton);
            startNode.gCost = 0;
            startNode.hCost = Heuristics(startPosiiton, endPosition);
            startNode.obj.GetComponent<Image>().color = Color.blue;

            PathNode endNode = GetPathNode(endPosition);
            endNode.obj.GetComponent<Image>().color = Color.red;

            nextPaths.Add(startNode);

            while (nextPaths.Count > 0)
            {
                PathNode currentNode = GetLowestFCostNode(nextPaths);
                currentNode.obj.GetComponent<Image>().color = Color.green;

                if (currentNode == endNode)
                {
                    lastNode = currentNode;
                    FindPath(lastNode);
                    yield break;
                }

                AddClosePath(currentNode.position);
                nextPaths.Remove(currentNode);

                neighbours = GetNeighbourList(currentNode);

                for (int i = 0; i < neighbours.Count; i++)
                {
                    neighbour = neighbours[i];
                    if (GetClosePath(neighbour.position) == true) continue;
                    int nextGCost = currentNode.gCost + Heuristics(currentNode.position, neighbour.position);
                    if (nextGCost < neighbour.gCost)
                    {

                        neighbour.prevNode = currentNode;
                        neighbour.gCost = nextGCost;
                        neighbour.hCost = Heuristics(neighbour.position, endPosition);

                        if (nextPaths.Contains(neighbour) == false)
                        {
                            nextPaths.Add(neighbour);
                        }
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

        }

        private List<PathNode> GetNeighbourList(PathNode _currentNode)
        {
            List<PathNode> neighbourList = new List<PathNode>();

            int[] x = { 0, 0, -1, 1, 1, -1, 1, -1 };
            int[] y = { -1, 1, 0, 0, 1, -1, -1, 1 };
            int len = 8;

            int nx, ny;
            for (int i = 0; i < len; i++)
            {
                nx = _currentNode.position.x + x[i];
                ny = _currentNode.position.y + y[i];

                if (nx >= 0 && nx < mapSize.x && ny >= 0 && ny < mapSize.y)
                {
                    neighbourList.Add(GetPathNode(nx, ny));
                }
            }
            return neighbourList;
        }

        public int Heuristics(Vector2Int _currPosition, Vector2Int _endPosition)
        {
            int xDistance = Mathf.Abs(_currPosition.x - endPosition.x);
            int yDistnace = Mathf.Abs(_currPosition.y - _endPosition.y);
            int reming = Mathf.Abs(xDistance - yDistnace);
            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistnace) + MOVE_STRAIGHT_COST * reming;
        }

        private bool GetClosePath(Vector2Int _position)
        {
            return closePath[_position.y, _position.x];
        }

        private void AddClosePath(Vector2Int _position)
        {
            print($"Add Close {_position.y} {_position.x}");
            closePath[_position.y, _position.x] = true;
        }

        private PathNode GetPathNode(int _x, int _y)
        {
            return mapData[_x + _y * mapSize.x];
        }

        private PathNode GetPathNode(Vector2Int _posiiton)
        {
            return mapData[_posiiton.x + _posiiton.y * mapSize.x];
        }

        private PathNode GetLowestFCostNode(List<PathNode> _pathNodeList)
        {
            PathNode lowestFCostNode = _pathNodeList[0];

            for (int i = 1; i < _pathNodeList.Count; i++)
            {
                if (_pathNodeList[i].fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = _pathNodeList[i];
                }
            }
            return lowestFCostNode;
        }

        void FindPath(PathNode _node)
        {
            _node.obj.GetComponent<Image>().color = Color.cyan;
            if (_node.prevNode != null)
            {
                FindPath(_node.prevNode);
            }
        }
    }
}