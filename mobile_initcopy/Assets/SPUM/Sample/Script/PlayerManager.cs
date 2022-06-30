using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerManager : MonoBehaviour
{
    public PlayerObj _prefabObj;
    public List<GameObject> _savedUnitList = new List<GameObject>();
    public Vector2 _startPos;
    public Vector2 _addPos;
    public int _columnNum;

    public Transform _playerPool;
    public List<PlayerObj> _playerList = new List<PlayerObj>();
    public PlayerObj _nowObj;
    public Transform _playerObjCircle;
    public Transform _goalObjCircle;
    public Camera _camera;
    Texture2D imageSave;

    public bool _generate;
    // Start is called before the first frame update

    public GameObject _bg;
    public bool _screenShot;
    public enum ScreenShotSize
    {
        HD,
        FHD,
        UHD
    }
    public ScreenShotSize _screenShotSize = ScreenShotSize.HD;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_generate)
        {
            GetPlayerList();
            _generate = false;
        }

        if(_screenShot)
        {
            SetScreenShot();
            _screenShot = false;
        }

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                if(hit.collider.CompareTag("Player"))
                {
                    _nowObj = hit.collider.GetComponent<PlayerObj>();
                }
                else
                {
                    //Set move Player object to this point
                    if(_nowObj!=null)
                    {
                        Vector2 goalPos = hit.point;
                        _goalObjCircle.transform.position = hit.point;
                        _nowObj.SetMovePos(goalPos);
                    }
                }
            }
        }

        if(_nowObj!=null)
        {
            _playerObjCircle.transform.position = _nowObj.transform.position;
        }
    }

    public void GetPlayerList()
    {
        //reset list.
        List<GameObject> tList = new List<GameObject>();
        for(var i =0; i < _playerPool.transform.childCount; i++)
        {
            GameObject tOBjj = _playerPool.transform.GetChild(i).gameObject;
            tList.Add(tOBjj);
        }
        foreach(var obj in tList)
        {
            DestroyImmediate(obj);
        }

        //net Edited. 2022.01.18
        _playerList.Clear();
        _savedUnitList.Clear();

        var saveArray = Resources.LoadAll<GameObject>("SPUM/SPUM_Units");
        _savedUnitList.AddRange(saveArray);
        //

        
        float numXStart = _startPos.x;
        float numYStart = _startPos.y;

        float numX = _addPos.x;
        float numY = _addPos.y;
        float ttV = 0;

        int sColumnNum = _columnNum;

        for(var i = 0 ; i < _savedUnitList.Count;i++)
        {
            if(i > sColumnNum-1)
            {
                numYStart -= 1f;
                numXStart -= numX * _columnNum;
                sColumnNum += _columnNum;
                ttV += numY;
            }
            
            GameObject ttObj = Instantiate(_prefabObj.gameObject) as GameObject;
            ttObj.transform.SetParent(_playerPool);
            ttObj.transform.localScale = new Vector3(1,1,1);
            

            GameObject tObj = Instantiate(_savedUnitList[i]) as GameObject;
            tObj.transform.SetParent(ttObj.transform);
            tObj.transform.localScale = new Vector3(1,1,1);
            tObj.transform.localPosition = Vector3.zero;

            ttObj.name = _savedUnitList[i].name;

            PlayerObj tObjST = ttObj.GetComponent<PlayerObj>();
            SPUM_Prefabs tObjSTT = tObj.GetComponent<SPUM_Prefabs>();

            tObjST._prefabs = tObjSTT;

            ttObj.transform.localPosition = new Vector3(numXStart + numX * i,numYStart+ttV,0);
            _playerList.Add(tObjST);
            
        }
    }

    //스크린샷 찍기
    public void SetScreenShot()
    {
        
        _bg.SetActive(false);
        Vector2 _nowSize = new Vector2(Screen.currentResolution.width,Screen.currentResolution.height);
        switch(_screenShotSize)
        {
            case ScreenShotSize.HD:
            Screen.SetResolution(1280, 720, false);
            break;

            case ScreenShotSize.FHD:
            Screen.SetResolution(1920, 1080, false);
            break;

            case ScreenShotSize.UHD:
            Screen.SetResolution(3840, 2160, false);
            break;
        }
        int tX = _camera.scaledPixelWidth;
		int tY = _camera.scaledPixelHeight;

		RenderTexture tempRT = new RenderTexture(tX, tY, 24, RenderTextureFormat.ARGB32)
		{
			antiAliasing = 4
		};
	
		_camera.targetTexture = tempRT;
		RenderTexture.active = tempRT;
		_camera.Render();

		imageSave = new Texture2D(tX, tY, TextureFormat.ARGB32, false, true);
		
		float tXPos = tX*0.5f - imageSave.width*0.5f;
		float tYPos = tY*0.5f - imageSave.height*0.5f;

		imageSave.ReadPixels(new Rect(tXPos, tYPos, imageSave.width, imageSave.height), 0, 0);
		imageSave.Apply();

        byte[] bytes = imageSave.EncodeToPNG();
        string tName = string.Format("{0:yyyy-MM-dd_HH-mm-ss-fff}", System.DateTime.Now);
        string filename = string.Format("{0}/SPUM/ScreenShots/{1}.png",Application.dataPath,tName);
        System.IO.File.WriteAllBytes(filename, bytes);

		RenderTexture.active = null;
		_camera.targetTexture = null;

		DestroyImmediate(tempRT);
        DestroyImmediate(imageSave);

        Screen.SetResolution((int)_nowSize.x, (int)_nowSize.y, false);
        Debug.Log("Screenshot Saved : " + filename);
        _bg.SetActive(true);
    }

}
