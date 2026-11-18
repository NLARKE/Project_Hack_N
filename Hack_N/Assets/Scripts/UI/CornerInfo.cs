using System;
using UnityEngine;
using TMPro;

public class CornerInfo : MonoBehaviour
{
    public TMP_Text topLeft;
    public TMP_Text bottomRight;
    public string version = "0.1.0";

    float nextUpdate;

    void Update()
    {
        if (Time.time < nextUpdate) return;
        nextUpdate = Time.time + 0.5f;

        topLeft.text =
            "SYS_TIME: " + DateTime.Now.ToString("HH:mm:ss") + "\n" +
            "NODE: 127.0.0.1";

        bottomRight.text =
            "MEM: " + UnityEngine.Random.Range(40, 60) + "%\n" +
            "CPU: " + UnityEngine.Random.Range(10, 80) + "%\n" +
            "VER " + version;
    }
}