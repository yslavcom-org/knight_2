using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void DisplayFPS(int w, int h)
    {
        Rect rect = new Rect(0, 0, w, h * 2 / 100);

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 20;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }


    private float dbg_value;
    public void SetDebugFloatValue(float val)
    {
        dbg_value = val;
    }

    void DisplayDEBUG(int w, int h)
    {
        Rect rect = new Rect(0, 0, w, h * 2 / 100);

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleLeft;
        style.fontSize = h * 2 / 20;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        string text = string.Format("dbg_val = {0}", dbg_value);
        GUI.Label(rect, text, style);
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        DisplayFPS(w, h);
        DisplayDEBUG(w, h);
    }
}
