using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;

    private Gamepad pad1;

    private Coroutine stopRumbleAfterTimeCoroutine;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        //get reference to gamepad
        pad1 = Gamepad.current;

        //if we have a current Gamepad
        if (pad1 != null)
        {
            //start rumble
            pad1.SetMotorSpeeds(lowFrequency, highFrequency);

            //stop rumble
            stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad1));
        }
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //once our duration is finished
        pad.SetMotorSpeeds(0f, 0f);
    }
}
