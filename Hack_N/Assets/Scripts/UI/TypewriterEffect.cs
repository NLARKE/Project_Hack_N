using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private string fullText;

    [Header("Settings")]
    [SerializeField] private float typewriterSpeed = 0.05f; // စာလုံးတစ်လုံးချင်းစီ ပေါ်လာမည့် အမြန်နှုန်း

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            fullText = textComponent.text; // Text ထဲမှာ ရေးထားတဲ့ စာသားကို သိမ်းထားမည်
            textComponent.text = "";      // စစချင်းမှာ စာသားကို အလွတ်ဖြစ်စေမည်
            StartCoroutine(ShowTextRoutine());
        }
    }

    private IEnumerator ShowTextRoutine()
    {
        foreach (char character in fullText.ToCharArray())
        {
            textComponent.text += character;
            yield return new WaitForSeconds(typewriterSpeed); // တစ်လုံးချင်းစီကြား ခေတ္တစောင့်မည့်အချိန်
        }
    }
}