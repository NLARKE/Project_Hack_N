using System.Collections;
using UnityEngine;
using TMPro;

public class TextGlitch : MonoBehaviour
{
    public TMP_Text target;
    public float minInterval = 2f;
    public float maxInterval = 5f;
    public float glitchDuration = 0.18f;
    public float shake = 10f;

    const string glyphs = "01#@$%&*<>/\\|";
    RectTransform rt;
    Vector2 basePos;
    Coroutine routine;

    void Awake()
    {
        if (target == null) target = GetComponent<TMP_Text>();
        rt = target.rectTransform;
    }

    void OnEnable()
    {
        basePos = rt.anchoredPosition;
        routine = StartCoroutine(Loop());
    }

    void OnDisable()
    {
        if (routine != null) StopCoroutine(routine);
        rt.anchoredPosition = basePos;
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            string original = target.text;
            float end = Time.time + glitchDuration;

            while (Time.time < end)
            {
                target.text = Scramble(original);
                rt.anchoredPosition = basePos + new Vector2(
                    Random.Range(-shake, shake), Random.Range(-shake * 0.4f, shake * 0.4f));
                yield return new WaitForSeconds(0.04f);
            }

            target.text = original;
            rt.anchoredPosition = basePos;
        }
    }

    string Scramble(string s)
    {
        char[] arr = s.ToCharArray();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == ' ' || arr[i] == '\n') continue;
            if (Random.value < 0.15f) arr[i] = glyphs[Random.Range(0, glyphs.Length)];
        }
        return new string(arr);
    }
}