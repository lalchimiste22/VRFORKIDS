using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class JoystickListener : MonoBehaviour , INibiruJoystickListener{
    private Text textField;
    // Use this for initialization
    void Start()
    {
        textField = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnLeftStickX(float axisValue)
    {
       if(textField != null)
        {
            textField.text = "OnLeftStickX:"+axisValue;
        }
    }

    public void OnLeftStickY(float axisValue)
    {
        if (textField != null)
        {
            textField.text = "OnLeftStickY:" + axisValue;
        }
    }

    public void OnPressA()
    {
        if (textField != null)
        {
            textField.text = "OnPressA";
        }
    }

    public void OnPressB()
    {
        if (textField != null)
        {
            textField.text = "OnPressB";
        }
    }

    public void OnPressDpadDown()
    {
        if (textField != null)
        {
            textField.text = "OnPressDpadDown";
        }
    }

    public void OnPressDpadLeft()
    {
        if (textField != null)
        {
            textField.text = "OnPressDpadLeft";
        }
    }

    public void OnPressDpadRight()
    {
        if (textField != null)
        {
            textField.text = "OnPressDpadRight";
        }
    }

    public void OnPressDpadUp()
    {
        if (textField != null)
        {
            textField.text = "OnPressDpadUp";
        }
    }

    public void OnPressL1()
    {
        if (textField != null)
        {
            textField.text = "OnPressL1";
        }
    }

    public void OnPressL2()
    {
        if (textField != null)
        {
            textField.text = "OnPressL2";
        }
    }

    public void OnPressR1()
    {
        if (textField != null)
        {
            textField.text = "OnPressR1";
        }
    }

    public void OnPressR2()
    {
        if (textField != null)
        {
            textField.text = "OnPressR2";
        }
    }

    public void OnPressSelect()
    {
        if (textField != null)
        {
            textField.text = "OnPressSelect";
        }
    }

    public void OnPressStart()
    {
        if (textField != null)
        {
            textField.text = "OnPressStart";
        }
    }

    public void OnPressX()
    {
        if (textField != null)
        {
            textField.text = "OnPressX";
        }
    }

    public void OnPressY()
    {
        if (textField != null)
        {
            textField.text = "OnPressY";
        }
    }

    public void OnRightStickX(float axisValue)
    {
        if (textField != null)
        {
            textField.text = "OnRightStickX:" + axisValue;
        }
    }

    public void OnRightStickY(float axisValue)
    {
        if (textField != null)
        {
            textField.text = "OnRightStickY:" + axisValue;
        }
    }

}
