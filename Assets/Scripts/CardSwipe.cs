using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CardSwipe : MonoBehaviour
{
    private int currentIndex = 0;
    private Transform[] children;
    [SerializeField] private float moveDistance = 5f; // Distance to move
    [SerializeField] private float moveDuration = 0.5f; // Duration of the move
    [SerializeField] private GameObject resultCanvasPrefab; // Prefab for result canvas
    [SerializeField] private CanvasGroup whiteoutCanvas; // Assign a full-screen white panel with CanvasGroup

    private void Start()
    {

        int count = transform.childCount;
        children = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            children[i] = transform.GetChild(i);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MoveNextChild();
        }
    }

    private void MoveNextChild()
    {
        if (currentIndex < children.Length)
        {
            children[currentIndex].DOMoveX(children[currentIndex].position.x - moveDistance, moveDuration)
                .SetEase(Ease.InOutSine);
            currentIndex++;
        }
        else
        {
            //StartCoroutine(ShowResultCanvas());
        }
    }

    private IEnumerator ShowResultCanvas()
    {
        // Whiteout effect
        whiteoutCanvas.gameObject.SetActive(true);
        whiteoutCanvas.alpha = 0;
        whiteoutCanvas.DOFade(1, 0.5f); // Whiteout duration

        yield return new WaitForSeconds(0.5f); // Wait for whiteout to complete

        // Instantiate result canvas
        GameObject resultCanvas = Instantiate(resultCanvasPrefab, transform.position, Quaternion.identity);
        CanvasGroup resultCanvasGroup = resultCanvas.GetComponent<CanvasGroup>();
        resultCanvasGroup.alpha = 0;

        // Fade in the result canvas
        resultCanvasGroup.DOFade(1, 1f).SetEase(Ease.InOutSine);

        // Fade out the whiteout
        whiteoutCanvas.DOFade(0, 0.5f).OnComplete(() => whiteoutCanvas.gameObject.SetActive(false));
    }
}
