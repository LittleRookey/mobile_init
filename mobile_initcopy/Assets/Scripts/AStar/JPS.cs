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

    // �ش� ��ũ��Ʈ�� ȣ�������� ��ũ��Ʈ�� ���� ���ص��� ���̱� ���� ���۵Ǿ����ϴ�.
    // list���� contains�� ã�°��� ���߿��� ū �δ��� �� �� �ֽ��ϴ�.
    // �ش� ��ũ��Ʈ�� �����ؼ� ����Ͻ� ��� �˻� �κп� ���� �ڵ常 ����ȭ ��Ű�� �˴ϴ�.

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
            #region  �ʰ�ȭ 

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


                #region  �ð�ȭ �ϱ� ���� �� ���� ����Ʈ�� �� ��带 �ʷ����� ����
                if (currentNode != startNode)
                {
                    currentNode.tile.SetColor(Color.green);
                }
                #endregion

                if (currentNode == endNode) // ���� ��尡 �� �����
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

                currentNode.moveable = false;    // �ѹ� �� ��� 

                yield return waitTime;
            }
        }

        #region  ������ �˻�

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

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x + 1, y) == false)   // Ž�� ���� ����
                {
                    break;
                }
                currentNode = GetGrid(++x, y); // ���� ������ ���� �̵�

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

                #region ������ ���� �����鼭 ������ ���� �շ��ִ� ���

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y + 1).moveable == false) // ������ ���̸�
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // ������ ���� �������� ������
                        {
                            if (isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            isFind = true;
                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region ������ ���� �����鼭 ������ �Ʒ��� �շ��ִ� ���

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y - 1).moveable == false) // �Ʒ����� ���̰�
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) // ������ �Ʒ��� ���� ���� ������
                        {
                            if (isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            isFind = true;
                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion
            }

            startNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  ���� �˻�

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


                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x - 1, y) == false)   // Ž�� ���� ����
                {
                    break;
                }

                currentNode = GetGrid(--x, y); // ���� ���� ���� �̵�
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

                #region ������ ���� �����鼭 ���� ���� �շ��ִ� ���

                if (y + 1 < mapSize.y && x - 1 >= 0)
                {
                    if (GetGrid(x, y + 1).moveable == false) // ������ ���̸�
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // ���� ���� �������� ������
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region ������ ���� �����鼭 ���� �Ʒ��� �շ��ִ� ���

                if (y > 0 && x - 1 >= 0)
                {
                    if (GetGrid(x, y - 1).moveable == false) // �Ʒ����� ���̰�
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // ���� �Ʒ��� ���� ���� ������
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }
                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion
            }

            startNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  ���� �˻�

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

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x, y + 1) == false)   // Ž�� ���� ����
                {
                    break;
                }

                currentNode = GetGrid(x, ++y); // ���� ���� ���� �̵�
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

                #region ������ ���� �����鼭 ���� ���� �շ��ִ� ���

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // �������� ���̸�
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // ������ ����
                        {
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            isFind = true;
                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region ������ ���� �����鼭 ���� �Ʒ��� �շ��ִ� ���

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // ������ ���̰�
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // ���� ���� ���� ���� ������
                        {
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            isFind = true;
                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion
            }

            _satrtNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  �Ʒ��� �˻�

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

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x, y - 1) == false)   // Ž�� ���� ����
                {
                    break;
                }

                currentNode = GetGrid(x, --y); // ���� ���� ���� �̵�
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

                #region ������ ���� �����鼭 ���� ���� �շ��ִ� ���

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // �������� ���̸�
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) // ������ �Ʒ��� �� �� �ִٸ�
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region ������ ���� �����鼭 ���� �Ʒ��� �շ��ִ� ���

                if (y > 0 && x - 1 >= 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // ������ ���̰�
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // ���� �Ʒ��� ���� ���� ������
                        {
                            isFind = true;
                            if (_isMovealbe == false)
                            {
                                AddOpenList(currentNode, startNode);
                            }

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion
            }

            _satrtNode.moveable = true;
            return isFind;
        }

        #endregion


        #region  ������ �� �밢�� �˻�

        private void RightUp(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x + 1, y + 1) == false)   // Ž�� ���� ����
                {
                    break;
                }



                currentNode = GetGrid(++x, ++y); // ���� ���� ���� �̵�

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

                #region ������ ���� �����鼭 ���� ���� �������� ���� ���

                if (y + 1 < mapSize.y && x > 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // ������ ����
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // ���� ���� �ȸ���
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region �Ʒ��� ���� �����鼭 ������ �Ʒ��� �ȸ�������

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y - 1).moveable == false) // ������ ���̰�
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) // ���� �Ʒ��� ���� ���� ������
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
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


        #region  ������ �Ʒ� �밢�� �˻�

        private void RightDown(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x + 1, y - 1) == false)   // Ž�� ���� ����
                {
                    break;
                }

                currentNode = GetGrid(++x, --y); // ���� ���� ���� �̵�

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

                #region ������ �����ְ� ���� �Ʒ��� ���� ���� ������

                if (y > 0 && x > 0)
                {
                    if (GetGrid(x - 1, y).moveable == false) // ������ ����
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // ���� ���� �ȸ���
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region ������ �����ְ� ������ ���� �������� ������

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x, y + 1).moveable == false) // ������ ���̰�
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // ������ ���� �������� ������
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
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


        #region  ���� �� �밢�� �˻�

        private void LeftUP(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x - 1, y + 1) == false)   // Ž�� ���� ����
                {
                    break;
                }

                currentNode = GetGrid(--x, ++y); // ���� ���� ���� �̵�

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

                #region �������� �����ְ� ������ ���� �������� ������

                if (y + 1 < mapSize.y && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // �������� ����
                    {
                        if (GetGrid(x + 1, y + 1).moveable == true) // ������ ���� �ȸ���
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region �Ʒ��� �����ְ� ���� �Ʒ��� �������� ������

                if (y > 0 && x > 0)
                {
                    if (GetGrid(x, y - 1).moveable == false) // �Ʒ��� ��
                    {
                        if (GetGrid(x - 1, y - 1).moveable == true) // ���� �Ʒ��� �ȸ���
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
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

        #region  ���� �Ʒ� �밢�� �˻�

        private void LeftDown(PathNode _satrtNode)
        {
            int x = _satrtNode.position.x;
            int y = _satrtNode.position.y;

            PathNode startNode = _satrtNode;
            PathNode currentNode = startNode;


            while (currentNode.moveable == true)
            {
                currentNode.moveable = false;

                #region �� ����� �Ѿ���� �� �Ѿ�������� üũ

                if (Comp(x - 1, y - 1) == false)   // Ž�� ���� ����
                {
                    break;
                }

                currentNode = GetGrid(--x, --y); // ���� ���� ���� �̵�

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

                #region �������� �����ְ� ������ ���� �������� ������

                if (y > 0 && x + 1 < mapSize.x)
                {
                    if (GetGrid(x + 1, y).moveable == false) // �������� ����
                    {
                        if (GetGrid(x + 1, y - 1).moveable == true) //������ �Ʒ��� �� ����
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
                        }
                    }
                }

                #endregion

                #region �Ʒ��� �����ְ� ���� �Ʒ��� �������� ������

                if (y + 1 < mapSize.y && x < 0)
                {
                    if (GetGrid(x, y + 1).moveable == false) // ���� ����
                    {
                        if (GetGrid(x - 1, y + 1).moveable == true) // ���� ���� �ȸ���
                        {
                            AddOpenList(currentNode, startNode);

                            break; // �ڳ� �߰��ϸ� �ٷ� ����
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

        #region  Ž�� ���� ����
        private bool Comp(int _x, int _y)
        {
            if (_x < 0 || _y < 0 || _x >= mapSize.x || _y >= mapSize.y)
            {
                return false;
            }
            return true;
        }
        #endregion


        #region ����Ʈ �ȿ� �ִ� ��带 X�� y��ǥ ������ ��ȯ

        private PathNode GetGrid(int _x, int _y)
        {
            return mapData[_x + _y * mapSize.x];
        }

        #endregion


        #region  �� �����ϴ� ��
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


        #region ���� ������ ���� ª�� ����� ���� ��ȯ �޸���ƽ
        private int Heuristic(Vector2Int _currPosition, Vector2Int _endPosition)
        {
            int x = Mathf.Abs(_currPosition.x - _endPosition.x);
            int y = Mathf.Abs(_currPosition.y - _endPosition.y);
            int reming = Mathf.Abs(x - y);

            return MOVE_DIAGONAL_COST * Mathf.Min(x, y) + MOVE_STRAIGHT_COST * reming;
        }

        #endregion


        #region  ����Ʈ �߿� ���� ª�� F���� ���� ��带 ��ȯ
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