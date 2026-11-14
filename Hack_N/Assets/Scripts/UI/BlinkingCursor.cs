using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkingCursor : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private string baseText;

    [Header("Blinking Settings")]
    [SerializeField] private float blinkInterval = 0.5f; // တဖျပ်ဖျပ် လင်းမည့် အမြန်နှုန်း (စက္ကန့်)
    private bool showCursor = true;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            // မူလစာသားကို သိမ်းထားမည် (ဥပမာ - "[ PRESS SPACEBAR TO START HACK ]")
            baseText = textComponent.text;
            StartCoroutine(BlinkingRoutine());
        }
    }

    private IEnumerator BlinkingRoutine()
    {
        while (true)
        {
            showCursor = !showCursor;

            if (showCursor)
            {
                // Cursor လေး ပေါ်နေချိန် (အနောက်တွင် _ ပါမည်)
                textComponent.text = baseText + "_";
            }
            else
            {
                // Cursor လေး ပျောက်နေချိန် (အနောက်တွင် Space သို့မဟုတ် ဘာမှမပါ)
                textComponent.text = baseText + " ";
            }

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}