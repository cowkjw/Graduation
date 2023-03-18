using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpText : MonoBehaviour
{

    public float offset = 1.8f;
    public float maxScale = 2.0f;
    public float minScale = 1.0f;

    private Transform player;
    private Camera mainCamera;
    private float startTime;

    private void Start()
    {
        player = Managers.Game.GetPlayer().transform;
        mainCamera = Camera.main;
        startTime = Time.time;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        // Update the position and rotation of the text
        Vector3 screenPos = mainCamera.WorldToScreenPoint(player.position + Vector3.up * offset);
        transform.position = mainCamera.ScreenToWorldPoint(screenPos);
        transform.rotation = Quaternion.Euler(0, mainCamera.transform.rotation.eulerAngles.y, 0);

        // Scale the text over time
        float t = (Time.time - startTime) / 1f;
        float scale = Mathf.SmoothStep(minScale, maxScale, t);
        transform.localScale = new Vector3(scale, scale, scale);

        // Destroy the text object when it reaches maxScale
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator AnimateText()
    {
        float t = 0.0f;
        float duration = 1f; // How long it takes for the text to reach maxScale
        while (true)
        {
            // Scale the text using a smoothstep function
            float scale = Mathf.SmoothStep(minScale, maxScale, t);
            transform.localScale = new Vector3(scale, scale, scale);
            t += Time.deltaTime / duration;

            // End the animation once the text reaches maxScale
            if (t >= 1.0f)
            {
                break;
            }

            yield return null;
        }
        Destroy(gameObject);
    }
}
