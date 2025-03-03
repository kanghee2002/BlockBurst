using UnityEngine;

public class SimpleLetterbox : MonoBehaviour
{
    [SerializeField] private float minAspectRatio = 5f / 3f;  // 최소 화면 비율
    private Camera cam;

    /*private void Start()
    {
        cam = GetComponent<Camera>();
        cam.backgroundColor = Color.black;  // 레터박스 색상
    }

    private void Update()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        
        if (currentAspect < minAspectRatio)
        {
            float normalizedHeight = currentAspect / minAspectRatio;
            float yOffset = (1f - normalizedHeight) / 2f;
            cam.rect = new Rect(0, yOffset, 1, normalizedHeight);
        }
        else
        {
            cam.rect = new Rect(0, 0, 1, 1);
        }
    }*/
}