using System.Collections;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float count;
    
    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 30;
        GUI.Label(new Rect(0, 40, 900, 500), "FPS: " + Mathf.Round(count));
    }
}