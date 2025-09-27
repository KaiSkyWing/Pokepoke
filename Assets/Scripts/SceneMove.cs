using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMove : MonoBehaviour
{
    [SerializeField] private Image fadeCanvas; // Assign the white panel's Image in the inspector
    [SerializeField] private float fadeDuration = 1f; // Duration of fade-out

    public void ClickPack()
    {
        StartCoroutine(FadeAndLoadScene("Main"));
    }

    public void ClickStart()
    {
        StartCoroutine(FadeAndLoadScene("Title"));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // Ensure the fade panel is visible
        fadeCanvas.gameObject.SetActive(true);

        // Get the current color
        Color fadeColor = fadeCanvas.color;

        // Gradually increase alpha to 1 (fade to white)
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(0, 1, t / fadeDuration);
            fadeCanvas.color = fadeColor; // Apply the new color
            yield return null;
        }

        // Load next scene
        SceneManager.LoadScene(sceneName);
    }
}
