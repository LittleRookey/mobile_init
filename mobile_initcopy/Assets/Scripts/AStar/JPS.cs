using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JSP
{
    [System.Serializable]
    public class PathNode
    {
        public Tile tile;
        public Vector2Int position;

        public int gCost, hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }


        public PathNode parentNode;
        public bool moveable = true;


        public PathNode(GameObject _tile, int _x, int _y, bool _moveable)
        {
            tile = _tile.GetComponent<Tile>();
            position = new Vector2Int(_x, _y);
            gCost = int.MaxValue;
            moveable = _moveable;
        }
    }

    // 해당 스크립트는 호율성보다 스크립트에 대한 이해도를 높이기 위해 제작되었습니다.
    // list에서 contains로 찾는것이 나중에는 큰 부담이 될 수 있습니다.
    // 해당 스크립트를 수정해서 사용하실 경우 검색 부분에 대한 코드만 최적화 시키면 됩니다.

    public class JPS : MonoBehaviour
    {

        const int MOVE_STRAIGHT_COST = 10;
        const int MOVE_DIAGONAL_COST = 14;

        [SerializeField] Transform map;
        [SerializeField] Vector2Int mapSize;

        PathNode startNode, endNode;

        List<PathNode> mapData = new List<PathNode>();
        List<PathNode> openNodes = new List<PathNode>();

        public void PathFinding()
        {
            StartCoroutine(PathFindingCor());
        }

        private IEnumerator PathFindingCor()
        {
            #region  초가화 

            openNodes.Clear();
            mapData.Clear();
            InitMap();

            WaitForSeconds waitTime = new WaitForSeconds(.05f);

            #endregion

            startNode.gCost = 0;
            startNode.hCost = Heuristic(startNode.position, endNode.position);
            openNodes.Add(startNode);

            // PAthFinding
            while (openNodes.Count > 0)
            {
                PathNode currentNode = GetLowestFCost(openNodes);
                openNodes.Remove(currentNode);


                #region  시각화 하기 위한 곳 오픈 리스트에 들어간 노드를 초록으로 변경
                if (currentNode != startNode)
                {
                    currentNode.tile.SetColor(Color.green);
                }
                #endregion

                if (currentNode == endNode) // 현재 노드가 끝 노드라면
                {
                    print("Find");
                    ShowPath(endNode);
                    yield break;
                }

                if (currentNode.moveable == true)
                {
                    Right(currentNode);
                    yield return waitTime;

                    Down(currentNode);
                    yield return waitTime;

                    Left(currentNode);
                    yield return waitTime;

                    Up(currentNode);
                    yield return waitTime;

                    RightUp(currentNode);
                    yield return waitTime;

                    RightDown(currentNode);
                    yield return waitTime;

                    LeftUP(currentNode);
                    yield return waitTime;

                    LeftDown(currentNode);
                    yield return waitTime;
                }

                currentNode.moveable = false;    // 한번 간 노드 

                yield return waitTime;
            }
        }

        #region  오른쪽 검사

        private bool Right(PathNode _startNode, bool isMovealbe = false)
        {
            int x = _startNode.position.x;
            int y = _startNode.position.y;

            PathNode startNode = _startNode;
            PathNode currentNode = startNode;

            bool isFind = false;
            while (currentNode.moveable == true)
            {
                currentNode.moveable = isMovealbe;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x + 1, y) == false)   // 탐색 가능 여부
                {
                    break;
                }
                currentNode = GetGrid(++x, y); // 다음 오른쪽 노드로 이동

                if (currentNode.moveable == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    isFind = true;
                    if (isMovealbe == false)
                    {
                        AddOpenList(currentNode, startNode);
                    }
                    break;
                }

                currentNode.tile.SetColor(Color.gray);

                #endregion

                #region 위쪽이 막혀 있으면서 오른쪽 위는 뚫려있는 경우

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y + 1).moveable == false) // 위쪽이 벽이면
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // 오른쪽 위가 막혀있지 않으면
                        {
                            if (isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            isFind = true;
                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위쪽이 막혀 있으면서 오른쪽 아래는 뚫려있는 경우

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y - 1).moveable == false) // 아래쪽이 벽이고
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) // 오른쪽 아래가 막혀 있지 않으면
                        {
                            if (isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            isFind = true;
                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion
            }

            startNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  왼쪽 검사

        private bool Left(PathNode _startNode, bool _isMovealbe = false)
        {
            int x = _startNode.position.x;
            int y = _startNode.position.y;

            PathNode startNode = _startNode;
            PathNode currentNode = startNode;

            bool isFind = false;

            while (currentNode.moveable == true)
            {
                currentNode.moveable = _isMovealbe;


                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x - 1, y) == false)   // 탐색 가능 여부
                {
                    break;
                }

                currentNode = GetGrid(--x, y); // 다음 왼쪽 노드로 이동
                if (currentNode.moveable == false)
                {
                    break;
                }
                currentNode.tile.SetColor(Color.gray);

                if (currentNode == this.endNode)
                {
                    isFind = true;

                    if (_isMovealbe == false)
                    {
                        AddOpenList(currentNode, startNode);
                    }
                    break;
                }

                #endregion

                #region 위쪽이 막혀 있으면서 왼쪽 위는 뚫려있는 경우

                if (y + 1 < mapSize.y && x - 1 >= 0)
                {
                    if (GetGrid(x, y + 1).moveable == false) // 위쪽이 벽이면
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // 왼쪽 위가 막혀있지 않으면
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위쪽이 막혀 있으면서 왼쪽 아래는 뚫려있는 경우

                if (y > 0 && x - 1 >= 0)
                {
                    if (GetGrid(x, y - 1).moveable == false) // 아래쪽이 벽이고
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // 왼쪽 아래가 막혀 있지 않으면
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion
            }

            startNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  위쪽 검사

        private bool Up(PathNode _satrtNode, bool _isMovealbe = false)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;

            bool isFind = false;

            while (currentNode.moveable == true)
            {
                currentNode.moveable = _isMovealbe;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }

                currentNode = GetGrid(x, ++y); // 다음 왼쪽 노드로 이동
                if (currentNode.moveable == false)
                {
                    break;
                }
                currentNode.tile.SetColor(Color.gray);

                if (currentNode == this.endNode)
                {
                    isFind = true;
                    if (_isMovealbe == false)
                    {
                        AddOpenList(currentNode, startNode);
                    }

                    break;
                }

                #endregion

                #region 위쪽이 막혀 있으면서 왼쪽 위는 뚫려있는 경우

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // 오른쪽이 벽이면
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // 오른쪽 위가
                        {
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            isFind = true;
                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위쪽이 막혀 있으면서 왼쪽 아래는 뚫려있는 경우

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // 왼쪽이 벽이고
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // 왼쪽 위가 막혀 있지 않으면
                        {
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            isFind = true;
                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion
            }

            _satrtNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  아래쪽 검사

        private bool Down(PathNode _satrtNode, bool _isMovealbe = false)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;

            bool isFind = false;

            while (currentNode.moveable == true)
            {
                currentNode.moveable = _isMovealbe;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }

                currentNode = GetGrid(x, --y); // 다음 왼쪽 노드로 이동
                if (currentNode.moveable == false)
                {
                    break;
                }
                currentNode.tile.SetColor(Color.gray);

                if (currentNode == this.endNode)
                {
                    isFind = true;
                    if (_isMovealbe == false)
                    {
                        AddOpenList(currentNode, startNode);
                    }

                    break;
                }

                #endregion

                #region 위쪽이 막혀 있으면서 왼쪽 위는 뚫려있는 경우

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // 오른쪽이 벽이면
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) // 오른쪽 아래가 갈 수 있다면
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위쪽이 막혀 있으면서 왼쪽 아래는 뚫려있는 경우

                if (y > 0 && x - 1 >= 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // 왼쪽이 벽이고
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // 왼쪽 아래가 막혀 있지 않으면
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion
            }

            _satrtNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  오른쪽 위 대각선 검사

        private void RightUp(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x + 1, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }



                currentNode = GetGrid(++x, ++y); // 다음 왼쪽 노드로 이동

                if (currentNode.moveable == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                currentNode.tile.SetColor(Color.gray);

                #endregion

                #region 왼쪽이 막혀 있으면서 왼쪽 위가 막혀있지 않은 경우

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // 왼쪽이 막힘
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // 왼쪽 위가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 아래가 막혀 있으면서 오른쪽 아래가 안막혔으면

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y - 1).moveable == false) // 왼쪽이 벽이고
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) // 왼쪽 아래가 막혀 있지 않으면
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Right(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Up(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _satrtNode.moveable = true;
        }

        #endregion


        #region  오른쪽 아래 대각선 검사

        private void RightDown(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x + 1, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }

                currentNode = GetGrid(++x, --y); // 다음 왼쪽 노드로 이동

                if (currentNode.moveable == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                currentNode.tile.SetColor(Color.gray);

                #endregion

                #region 왼쪽이 막혀있고 왼쪽 아래가 막혀 있지 않으면

                if (y > 0 && x > 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // 왼쪽이 막힘
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // 왼쪽 위가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 위쪽이 막혀있고 오른쪽 위가 막혀있지 않으면

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y + 1).moveable == false) // 위쪽이 벽이고
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // 오른쪽 위가 막혀있지 않으면
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Right(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Down(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _satrtNode.moveable = true;
        }

        #endregion


        #region  왼쪽 위 대각선 검사

        private void LeftUP(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x - 1, y + 1) == false)   // 탐색 가능 여부
                {
                    break;
                }

                currentNode = GetGrid(--x, ++y); // 다음 왼쪽 노드로 이동

                if (currentNode.moveable == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                currentNode.tile.SetColor(Color.gray);

                #endregion

                #region 오른쪽이 막혀있고 오른쪽 위가 막혀있지 않으면

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // 오른쪽이 막힘
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // 오른쪽 위가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 아래가 막혀있고 왼쪽 아래가 막혀있지 않으면

                if (y > 0 && x > 0)
                {
                    if (GetGrid(x, y - 1).moveable == false) // 아래가 벽
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // 왼쪽 아래가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Left(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Up(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _satrtNode.moveable = true;
        }

        #endregion

        #region  왼쪽 아래 대각선 검사

        private void LeftDown(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region 맵 사이즈가 넘어가는지 안 넘어가지는지를 체크

                if (Comp(x - 1, y - 1) == false)   // 탐색 가능 여부
                {
                    break;
                }

                currentNode = GetGrid(--x, --y); // 다음 왼쪽 노드로 이동

                if (currentNode.moveable == false)
                {
                    break;
                }

                if (currentNode == this.endNode)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                currentNode.tile.SetColor(Color.gray);

                #endregion

                #region 오른쪽이 막혀있고 오른쪽 위가 막혀있지 않으면

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // 오른쪽이 막힘
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) //오른쪽 아래가 안 막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }

                #endregion

                #region 아래가 막혀있고 왼쪽 아래가 막혀있지 않으면

                if (y + 1 < mapSize.y && x < 0)
                {
                    if (GetGrid(x, y + 1).moveable == false) // 위가 막힘
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // 왼쪽 위가 안막힘
                        {
                            AddOpenList(currentNode, startNode);

                            break; // 코너 발견하면 바로 종료
                        }
                    }
                }


                #endregion

                if (Left(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }

                if (Down(currentNode, true) == true)
                {
                    AddOpenList(currentNode, startNode);

                    break;
                }
            }

            _satrtNode.moveable = true;
        }

        #endregion

        #region  탐색 가능 여부
        private bool Comp(int _x, int _y)
        {
            if (_x < 0 || _y < 0 || _x >= mapSize.x || _y >= mapSize.y)
            {
                return false;
            }
            return true;
        }
        #endregion


        #region 리스트 안에 있는 노드를 X와 y좌표 값으로 반환

        private PathNode GetGrid(int _x, int _y)
        {
            return mapData[_x + _y * mapSize.x];
        }

        #endregion


        #region  맵 셋팅하는 곳
        private void InitMap()
        {
            Transform child;

            int width = mapSize.x;
            int x, y;

            for (int i = 0; i < map.childCount; i++)
            {
                child = map.GetChild(i);

                x = i % width;
                y = i / width;

                // Create New Node
                bool moveable = !child.CompareTag("Wall");
                PathNode newNode = new PathNode(child.gameObject, x, y, moveable);
                mapData.Add(newNode);

                // Add Wall
                if (child.CompareTag("Start"))
                {
                    startNode = newNode;
                    newNode.tile.SetColor(Color.red);
                }
                else if (child.CompareTag("End"))
                {
                    endNode = newNode;
                    newNode.tile.SetColor(Color.blue);
                }
                else if (child.CompareTag("Wall"))
                {
                    newNode.tile.SetColor(Color.black);
                }
                else
                {
                    newNode.tile.SetColor(Color.white);
                }
            }
        }
        #endregion


        #region 도착 노드까지 가장 짧은 경로의 값을 반환 휴리스틱
        private int Heuristic(Vector2Int _currPosition, Vector2Int _endPosition)
        {
            int x = Mathf.Abs(_currPosition.x - _endPosition.x);
            int y = Mathf.Abs(_currPosition.y - _endPosition.y);
            int reming = Mathf.Abs(x - y);

            return MOVE_DIAGONAL_COST * Mathf.Min(x, y) + MOVE_STRAIGHT_COST * reming;
        }

        #endregion


        #region  리스트 중에 가장 짧은 F값을 가진 노드를 반환
        private PathNode GetLowestFCost(List<PathNode> _pathList)
        {
            PathNode lowestNode = _pathList[0];

            for (int i = 1; i < _pathList.Count; i++)
            {
                if (_pathList[i].fCost < lowestNode.fCost)
                {
                    lowestNode = _pathList[i];
                }
            }
            return lowestNode;
        }


        #endregion

        private void ShowPath(PathNode _node)
        {
            if (_node != null)
            {
                if (_node == startNode)
                {
                    _node.tile.SetColor(Color.red);
                }
                else if (_node == endNode)
                {
                    _node.tile.SetColor(Color.blue);
                }
                else
                {
                    _node.tile.SetColor(Color.cyan);
                }

                if (_node.parentNode != null)
                {
                    Vector3 start = _node.tile.transform.position;
                    Vector3 end = _node.parentNode.tile.transform.position;
                    Debug.DrawLine(start, end, Color.yellow, 5);
                }

                ShowPath(_node.parentNode);
            }
        }

        private void AddOpenList(PathNode _currentNode, PathNode _parentNode)
        {
            int nextCost = _parentNode.gCost + Heuristic(_parentNode.position, _currentNode.position);
            //if (nextCost < _currentNode.gCost)
            // {
            _currentNode.parentNode = _parentNode;
            _currentNode.gCost = _parentNode.gCost + Heuristic(_parentNode.position, _currentNode.position);
            _currentNode.hCost = Heuristic(_currentNode.position, endNode.position);
            openNodes.Add(_currentNode);
            // }
        }
    }
}