using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OrbitFlip : MonoBehaviour
{
    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FlipOrbit();
        }
    }
    
    [SerializeField] private GameObject rotationAxis; // center object
    [SerializeField] private float radius = 3f;       // orbit radius
    [SerializeField] private float duration = 1f;     // flip duration

    public void FlipOrbit()
    {
        if (rotationAxis == null) return;

        Vector3 center = rotationAxis.transform.position;

        // Calculate current relative position from the axis
        Vector3 offset = transform.position - center;
        offset = offset.normalized * radius; // ensure correct radius
        Vector3 startPos = center + offset;

        // Target rotation around Y axis (180° orbit)
        float startAngle = Mathf.Atan2(offset.z, offset.x) * Mathf.Rad2Deg;
        float endAngle = startAngle + 180f;

        // DOTween sequence to combine orbit and Z wiggle
        Sequence seq = DOTween.Sequence();

        // Orbit movement using a custom tween
        seq.Append(DOTween.To(
            () => startAngle,
            angle =>
            {
                // Update orbit position
                float rad = angle * Mathf.Deg2Rad;
                transform.position = center + new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
            },
            endAngle,
            duration
        ).SetEase(Ease.OutQuad)); // slows down as it reaches the end

        // Slight Z-axis rotation (wiggle)
        seq.Join(transform.DORotate(
            new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 5f),
            duration / 2f
        ).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine));

        // Optionally rotate object itself 180° around Y for flip effect
        seq.Join(transform.DORotate(
            new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180f, transform.eulerAngles.z),
            duration,
            RotateMode.FastBeyond360
        ).SetEase(Ease.OutQuad));
    }
}
