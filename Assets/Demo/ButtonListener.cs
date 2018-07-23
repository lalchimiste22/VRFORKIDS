// Copyright 2016 Nibiru. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using UnityEngine;
using System.Collections;
using System;
using Nvr.Internal;

//  按钮点击事件
public class ButtonListener : MonoBehaviour, INvrButtonListener
{ 
    void Start()
    {
        // 修改FOV，越大越远看的越多
        //  NvrViewer.Instance.UpdateCameraFov(60);
        //  修改相机Far
        //  NvrViewer.Instance.UpateCameraFar(20000);

    }

    public void OnPressBack()
    {
        Debug.Log("OnPressBack");
        Application.Quit();
    }

    public void OnPressDown()
    {
        Debug.Log("OnPressDown");
        // NvrViewer.Instance.GazeApi(GazeTag.Hide, "");
    }

    public void OnPressEnter(bool isKeyUp)
    {
        if (isKeyUp)
        {
            Debug.Log("Confirm Key Up."+ gameObject.name);
        }else
        {
            Debug.Log("Confirm Key Down."+ gameObject.name);
        }
        Debug.Log("OnPressEnter."+gameObject.name + ", isKeyUp." + isKeyUp);
    }

    public void OnPressLeft()
    {
        Debug.Log("OnPressLeft.");
        // NvrViewer.Instance.GazeApi(GazeTag.Show, "");
        // NvrViewer.Instance.SetSystemVRMode(0);
        // NvrViewer.Instance.GazeApi(GazeTag.Set_Size, ((int)GazeSize.Large).ToString());
        // NvrViewer.Instance.ResetCameraFov();
        // NvrViewer.Instance.SetHorizontalAngleRange(-30, 30);
        // NvrViewer.Instance.RequestLock();
    }

    public void OnPressRight()
    {
        Debug.Log("OnPressRight_"+gameObject.name);
        // NvrViewer.Instance.SetSystemVRMode(1);
        // NvrViewer.Instance.GazeApi(GazeTag.Set_Color, "0.043f_0.435f_0.043f");
        // NvrViewer.Instance.UpdateCameraFov(45);
        // NvrViewer.Instance.SetVerticalAngleRange(-30, 30);
       //  NvrViewer.Instance.OpenVideoPlayer("/storage/emulated/0/nibiru.mp4", 0,2,1);// 跳转视频播放
        // NvrViewer.Instance.RequestUnLock();
    }

    public void OnPressUp()
    {
        Debug.Log("OnPressUp");
        // NvrViewer.Instance.UpdateCameraFov(40);
        // NvrViewer.Instance.GazeApi(GazeTag.Set_Size, ((int)GazeSize.Small).ToString());
        // NvrViewer.Instance.RemoveAngleLimit();
        // NvrViewer.Instance.CloseVideoPlayer();// 关闭视频播放
    }

    public void OnPressVolumnUp()
    {
        Debug.Log("OnPressVolumnUp");
    }

    public void OnPressVolumnDown()
    {
        Debug.Log("OnPressVolumnDown");

    } 
}
