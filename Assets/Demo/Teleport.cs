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

// 【瞬移】具体凝视接口的实现类，挂载在物体上面，用于处理物体被瞄准相关事件
[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour, INvrGazeResponder {
  private Vector3 startingPosition;
  private bool gazeAt = false;

  void Start() {
    startingPosition = transform.localPosition;
    SetGazedAt(false);
  }

  void LateUpdate() {
    NvrViewer.Instance.UpdateState();
    if (NvrViewer.Instance.BackButtonPressed)
    {
        //Debug.Log("LateUpdate,Click BackButtonPressed"); 
    }
    if (gazeAt && Input.GetKeyDown(KeyCode.JoystickButton0))
    {
        //Debug.Log("When GazedAt Cube ,Click Center Button");
    }
    }

    public void SetGazedAt(bool gazedAt) {
    gazeAt = gazedAt;
    GetComponent<Renderer>().material.color = gazedAt ? Color.green : Color.red;
  }

  public void Reset() {
    transform.localPosition = startingPosition;
  }

    public void ToggleVRMode()
    {
        NvrViewer.Instance.VRModeEnabled = !NvrViewer.Instance.VRModeEnabled;
    }

  public void ToggleDirectRender() {
 
  }

  public void TeleportRandomly() {
    Vector3 direction = Random.onUnitSphere;
    direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
    float distance = 2 * Random.value + 1.5f;
    transform.localPosition = direction * distance;
  }

  #region INvrGazeResponder implementation

  /// Called when the user is looking on a GameObject with this script,
  /// as long as it is set to an appropriate layer (see NvrGaze).
  public void OnGazeEnter() {
    SetGazedAt(true);
    }

  /// Called when the user stops looking on the GameObject, after OnGazeEnter
  /// was already called.
  public void OnGazeExit() {
    SetGazedAt(false);
    }

  /// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
  public void OnGazeTrigger() {
    TeleportRandomly();
  }

  #endregion
}
