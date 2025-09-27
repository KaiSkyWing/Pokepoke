using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private Image fadeCanvas; // Assign the white panel's Image in the inspector
    [SerializeField] private float fadeDuration = 1f; // Duration of fade-in

    private void Start()
    {
        StartCoroutine(FadeInStart());
    }

    private IEnumerator FadeInStart()
    {
        fadeCanvas.gameObject.SetActive(true); // Ensure the fade panel is active

        Color fadeColor = fadeCanvas.color;
        fadeColor.a = 1; // Start fully white
        fadeCanvas.color = fadeColor;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(1, 0, t / fadeDuration);
            fadeCanvas.color = fadeColor;
            yield return null;
        }
        fadeCanvas.gameObject.SetActive(false);
    }
}
