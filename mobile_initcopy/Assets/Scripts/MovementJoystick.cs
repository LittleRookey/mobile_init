using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour
{
    public GameObject joyStick;
    public GameObject joyStickBG;
    public Vector2 joystickVec;
    [SerializeField] private GameObject nw_highlight;
    [SerializeField] private GameObject ne_highlight;
    [SerializeField] private GameObject sw_highlight;
    [SerializeField] private GameObject se_highlight;

    private Vector2 joystickOriginalPos;
    [SerializeField] private float joystickRadius = 1f;
    RectTransform jt;
    RectTransform jtBG;
    Camera mainCam;

    Vector2 nextPos;
    private Vector2 pointA;
    private Vector2 pointB;
    void Start()
    {
        nextPos = Vector2.zero;
        mainCam = Camera.main;
        jt = joyStick.GetComponent<RectTransform>();
        jtBG = joyStickBG.GetComponent<RectTransform>();
        joystickOriginalPos = joyStickBG.transform.position;
        //joystickRadius = jtBG.sizeDelta.y / joystickBGDivider;
    }

    public void PointerDown()
    {
        pointA = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCam.transform.position.z));
        joyStick.transform.position = pointA;
        joyStickBG.transform.position = pointA;
        //joyStick.GetComponent<SpriteRenderer>().enabled = true;
        //joyStickBG.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Drag(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        pointB = mainCam.ScreenToWorldPoint(dragPos);

        Vector2 offset = pointB - pointA;
        Vector2 direction = Vector2.ClampMagnitude(offset, joystickRadius);

        nextPos.Set(pointA.x + direction.x, pointA.y + direction.y);
        joyStick.transform.position = nextPos;

        joystickVec = direction;
        if (joystickVec.x > 0 && joystickVec.y > 0)
        {
            ne_highlight.gameObject.SetActive(true);
            nw_highlight.gameObject.SetActive(false);
            se_highlight.gameObject.SetActive(false);
            sw_highlight.gameObject.SetActive(false);
        } else if(joystickVec.x < 0 && joystickVec.y > 0)
        {
            nw_highlight.gameObject.SetActive(true);
            ne_highlight.gameObject.SetActive(false);
            se_highlight.gameObject.SetActive(false);
            sw_highlight.gameObject.SetActive(false);
        } else if (joystickVec.x < 0 && joystickVec.y < 0)
        {
            sw_highlight.gameObject.SetActive(true);
            nw_highlight.gameObject.SetActive(false);
            ne_highlight.gameObject.SetActive(false);
            se_highlight.gameObject.SetActive(false);
        } else if (joystickVec.x > 0 && joystickVec.y < 0)
        {
            se_highlight.gameObject.SetActive(true);
            nw_highlight.gameObject.SetActive(false);
            ne_highlight.gameObject.SetActive(false);
            sw_highlight.gameObject.SetActive(false);
        } else
        {
            se_highlight.gameObject.SetActive(false);
            nw_highlight.gameObject.SetActive(false);
            ne_highlight.gameObject.SetActive(false);
            sw_highlight.gameObject.SetActive(false);
        }
    }

    public void PointerUp()
    {
        se_highlight.gameObject.SetActive(false);
        nw_highlight.gameObject.SetActive(false);
        ne_highlight.gameObject.SetActive(false);
        sw_highlight.gameObject.SetActive(false);
        joystickVec = Vector2.zero;
        joyStick.transform.position = joystickOriginalPos;
        joyStickBG.transform.position = joystickOriginalPos;
        //joyStick.GetComponent<SpriteRenderer>().enabled = false;
        //joyStickBG.GetComponent<SpriteRenderer>().enabled = false;
    }
}
