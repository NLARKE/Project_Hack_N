using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterWithCursor : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    [Header("Settings")]
    [SerializeField, TextArea] private string fullText = "[ PRESS SPACEBAR TO START HACK ]"; // ဖော်ပြလိုသည့် စာသားအပြည့်အစုံ
    [SerializeField] private float typewriterSpeed = 0.05f; // စာလုံးတစ်လုံးချင်း ပေါ်မည့်အမြန်နှုန်း
    [SerializeField] private float blinkInterval = 0.5f;   // ကာဆာ တဖျပ်ဖျပ်လင်းမည့် အမြန်နှုန်း

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            // စာသားကို အစကနေ စတင်၍ Typewriter + Blinking ပြုလုပ်မည့် Coroutine ကို စတင်မည်
            StartCoroutine(ShowTextAndBlinkRoutine());
        }
    }

    private IEnumerator ShowTextAndBlinkRoutine()
    {
        textComponent.text = "";

        // ၁။ ပထမအဆင့် - စာသားများကို တစ်လုံးချင်းစီ ရေးပြခြင်း (Typewriter Effect)
        foreach (char character in fullText.ToCharArray())
        {
            textComponent.text += character;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        // ၂။ ဒုတိယအဆင့် - စာသားပေါ်ပြီးသွားသောအခါ ကာဆာ (_) လေး တဖျပ်ဖျပ် လင်းနေစေခြင်း
        bool showCursor = true;
        while (true)
        {
            showCursor = !showCursor;

            if (showCursor)
            {
                textComponent.text = fullText + "_"; // ကာဆာ ပေါ်နေချိန်
            }
            else
            {
                textComponent.text = fullText + " "; // ကာဆာ ပျောက်နေချိန် (Space ဖြင့် ကွက်လပ်ထားမည်)
            }

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}