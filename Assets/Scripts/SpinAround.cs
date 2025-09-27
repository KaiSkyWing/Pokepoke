using UnityEngine;
using DG.Tweening; // Import DoTween

public class SpinAround : MonoBehaviour
{
    private const float ROTATION_STEP = 36f; // Defines the rotation step size

    [SerializeField] private float radius;
    [SerializeField] private bool isRef;
    [SerializeField] private float damping; // Speed reduction factor
    [SerializeField] private float swipeSensitivity; // Swipe control factor
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float arrangeDuration = 2f; // Duration for arranging children
    [SerializeField] private GameObject packopenPrefab; // Prefab to instantiate on swipe
    [SerializeField] private GameObject cardsPrefab; // Prefab for Cards

    private bool firstClick = true;
    private bool isSnapping = false;

    private Transform[] children;
    private float spinSpeed = 0f;
    private Vector2 lastTouchPosition;
    private bool isSwiping = false;
    private bool isClick = false;
    private float swipeThreshold = 5f; // Minimum movement to consider a swipe
    private Transform keptChild = null; // The child that is kept

    void Start()
    {
        ArrangeChildren();
    }

    void Update()
    {
        HandleSwipe();
        ApplySpin();
    }

    private void ArrangeChildren()
    {
        int count = transform.childCount;
        children = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            children[i] = transform.GetChild(i);
            
            // Calculate position in circle
            float angle = i * Mathf.PI * 2f / count;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 10f, Mathf.Sin(angle) * radius);
            children[i].localPosition = pos; // Start from above
            
            // Reset rotation before applying LookAt
            children[i].rotation = Quaternion.identity;
            children[i].LookAt(transform.position);
            
            // Ensure the child is upright after looking at center
            children[i].rotation = Quaternion.Euler(0, children[i].rotation.eulerAngles.y, 0);
        }
        
        // Animate children moving down and spinning counterclockwise
        Sequence sequence = DOTween.Sequence();
        
        foreach (Transform child in children)
        {
            sequence.Join(child.DOLocalMoveY(0f, arrangeDuration).SetEase(Ease.OutBounce));
        }
        
        sequence.Insert(0f, transform.DORotate(new Vector3(0, 360f, 0), arrangeDuration, RotateMode.FastBeyond360)
            .SetRelative().SetEase(Ease.Linear));
    }

    private void HandleSwipe()
    {
        bool isStationary = Mathf.Abs(spinSpeed) < 0.01f && !isSnapping;

        if (!firstClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (keptChild != null)
                {
                    Vector3 keptPosition = keptChild.position;
                    Destroy(keptChild.gameObject);
                    GameObject packopen = Instantiate(packopenPrefab, keptPosition, Quaternion.identity);

                    Transform topChild = packopen.transform.Find("Top");
                    if (topChild != null)
                    {
                        topChild.DOMoveX(topChild.position.x + 10, 1f).SetEase(Ease.InOutSine);
                        topChild.DORotate(new Vector3(0, 0, -35), 1f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine);
                        topChild.DOMoveY(topChild.position.y - 10, 2f).SetEase(Ease.InOutSine);
                    }
                    
                    // Wait for 1 second before starting animations
                    DOVirtual.DelayedCall(1f, () => {
                        packopen.transform.DOMoveY(keptPosition.y - 10f, 1f).SetEase(Ease.InOutSine);
                        mainCamera.transform.DOMoveZ(mainCamera.transform.position.z + 1, 1f).SetEase(Ease.InOutSine)
                            .OnComplete(() => {
                                if (!isRef)
                                {
                                    GameObject cards = Instantiate(cardsPrefab, keptPosition, Quaternion.identity);
                                    cards.transform.DOMoveY(6, 1f).SetEase(Ease.InOutSine);
                                }
                            });
                    });
                }
            }
        }

        if (firstClick && isStationary) // New conditions added
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastTouchPosition = Input.mousePosition;
                isSwiping = false;
                isClick = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isClick) // If it was a click (not a swipe)
                {
                    KeepClosestChild();
                    MoveCamera();
                    firstClick = false; // First click condition no longer applies after this
                }
                else
                {
                    SnapToClosestRotation();
                }
                isSwiping = false;
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 currentTouchPosition = Input.mousePosition;
                float deltaX = currentTouchPosition.x - lastTouchPosition.x;

                if (Mathf.Abs(deltaX) > swipeThreshold)
                {
                    isSwiping = true;
                    isClick = false; // It's a swipe, not a click
                }

                if (isSwiping)
                {
                    spinSpeed = deltaX * swipeSensitivity;
                    lastTouchPosition = currentTouchPosition;
                }
            }
        }
    }

    private void ApplySpin()
    {
        if (Mathf.Abs(spinSpeed) > 0.01f)
        {
            transform.Rotate(-Vector3.up, spinSpeed * Time.deltaTime);
            spinSpeed *= damping; // Gradually reduce spin speed
            UpdateChildRotation();
        }
        else
        {
            spinSpeed = 0f; // Stop completely when very slow
        }
    }

    private void SnapToClosestRotation()
    {
        float currentY = transform.eulerAngles.y;
        float closestTargetY = Mathf.Round(currentY / ROTATION_STEP) * ROTATION_STEP + ROTATION_STEP / 2;

        isSnapping = true;
        transform.DORotate(new Vector3(transform.eulerAngles.x, closestTargetY, transform.eulerAngles.z), 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => isSnapping = false);
    }

    private void KeepClosestChild()
    {
        float closestDistance = float.MaxValue;

        foreach (Transform child in children)
        {
            float distance = Vector3.Distance(mainCamera.transform.position, child.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                keptChild = child;
            }
        }

        foreach (Transform child in children)
        {
            if (child != keptChild)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void MoveCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.DOMove(new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + 5, mainCamera.transform.position.z + 5), 1f)
                .SetEase(Ease.InOutSine);
        }

        if (keptChild != null)
        {
            keptChild.DOLookAt(mainCamera.transform.position, 1f, AxisConstraint.Y)
                .SetEase(Ease.InOutSine);
        }
    }

    private void UpdateChildRotation()
    {
        foreach (Transform child in children)
        {
            child.LookAt(transform.position);

            if (isRef) child.rotation *= Quaternion.Euler(180, 0, 0);
        }
    }
}
