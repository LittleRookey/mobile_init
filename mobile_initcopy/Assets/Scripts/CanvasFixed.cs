using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFixed : MonoBehaviour
{
    [SerializeField] Camera[] cameras;
    [SerializeField] Vector2 viewArea;
    [SerializeField] float standardOrthographicSize = 20;
    [SerializeField] int setWidth = 1920;
    [SerializeField] int setHeight = 1080;
    private void Start()
    {
        SetResolution(); // �ʱ⿡ ���� �ػ� ����
    }

    /* �ػ� �����ϴ� �Լ� */
    public void SetResolution()
    {


        int deviceWidth = Screen.width; // ��� �ʺ� ����
        int deviceHeight = Screen.height; // ��� ���� ����

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true); // SetResolution �Լ� ����� ����ϱ�

        if ((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) // ����� �ػ� �� �� ū ���
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight); // ���ο� �ʺ�
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f); // ���ο� Rect ����
            }
           
        }
        else // ������ �ػ� �� �� ū ���
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight); // ���ο� ����

            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // ���ο� Rect ����
            }
        }
    }
}
