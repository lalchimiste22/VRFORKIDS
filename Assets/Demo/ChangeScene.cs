// Copyright 2016 Nibiru. All rights reserved.
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;//注意这个不能少  
public class ChangeScene : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject btnObj = GameObject.Find("Button_Jump");//"Button"为你的Button的名称  
        Button btn = btnObj.GetComponent<Button>();
        btn.onClick.AddListener(delegate ()
        {
            Debug.Log("场景切换->DemoScene2");
        });
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        //{
        //    GoNextScene();
        //}

        //if (Application.platform == RuntimePlatform.Android && Input.anyKeyDown)
        //{
        //    Event e = Event.current;
        //    if (e.isKey)
        //    {
        //        KeyCode currentKey = e.keyCode;
        //        if ((int)currentKey == 10)
        //        {
        //            GoNextScene();
        //        }
        //    }
        //}

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {

                Debug.Log("触摸事件开始 " + touch.position.x + "," + touch.position.y + ",mouse : " + Input.mousePosition.x + "," + Input.mousePosition.y);

            }
        }
      

    }

    public void GoNextScene()
    {
        SceneManager.LoadScene("DemoScene2");
    }

    public void OpenVideoPlayer()
    {
        NvrViewer.Instance.OpenVideoPlayer(NvrViewer.Instance.GetStoragePath() + "/nibiru.mp4", 0, 2, 1);// 跳转视频播放
    }
}
