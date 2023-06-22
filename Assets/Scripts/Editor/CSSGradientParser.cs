using UnityEditor;
using UnityEngine;

public class CSSGradientParser : EditorWindow
{
    private string _cssGradientCode;
    private Gradient _parsedGradient;

    [MenuItem("Window/CSS Gradient Parser")]
    public static void ShowWindow()
    {
        GetWindow<CSSGradientParser>("CSS Gradient Parser");
    }
}
