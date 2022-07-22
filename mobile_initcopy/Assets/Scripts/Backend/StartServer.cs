using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class StartServer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var bro = Backend.Initialize(true);
        if (bro.IsSuccess())
        {
            // 초기화 성공 시 로직
            Debug.Log("초기화 성공!");
            //CustomSignUp();
        }
        else
        {
            // 초기화 실패 시 로직
            Debug.LogError("초기화 실패!");
        }
    }


    //초기화 성공 이후 버튼 등을 통해 함수 실행
    public void CustomSignUp()
    {
        string id = "user1"; // 원하는 아이디
        string password = "1234"; // 원하는 비밀번호

        var bro = Backend.BMember.CustomSignUp(id, password);
        if (bro.IsSuccess())
        {
            Debug.Log("회원가입 성공!");
        }
        else
        {
            Debug.LogError("회원가입 실패!");
            Debug.LogError(bro); // 뒤끝의 리턴케이스를 로그로 보여줍니다.
        }
    }
}
