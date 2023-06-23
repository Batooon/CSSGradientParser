using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CSSGradientParser : EditorWindow
{
    private string _cssGradientCode;
    private Gradient _parsedGradient;
    private Texture2D _gradientTexture;
    private Vector2 _previousSize;

    [MenuItem("Window/CSS Gradient Parser")]
    public static void ShowWindow()
    {
        GetWindow<CSSGradientParser>("CSS Gradient Parser");
    }

    private void OnGUI()
    {
        if (_gradientTexture != null)
        {
            GUI.DrawTexture(new Rect(0, 0, (int)position.width, (int)position.height), _gradientTexture, ScaleMode.StretchToFill);
        }

        EditorGUILayout.BeginVertical();

        _cssGradientCode = EditorGUILayout.TextField("CSS Gradient", _cssGradientCode);

        if (GUILayout.Button("Parse Gradient"))
        {
            _parsedGradient = ParseGradient(_cssGradientCode);
            if (_parsedGradient != null)
            {
                _gradientTexture = GenerateGradientTexture(_parsedGradient);
            }
        }

        if (_parsedGradient != null)
        {
            EditorGUILayout.GradientField(_parsedGradient);
        }

        EditorGUILayout.EndVertical();
    }

    private void OnInspectorUpdate()
    {
        if (position.size != _previousSize && _parsedGradient != null)
        {
            _previousSize = position.size;
            _gradientTexture = GenerateGradientTexture(_parsedGradient);
            Repaint();
        }
    }

    private Texture2D GenerateGradientTexture(Gradient gradient)
    {
        int width = (int)position.width;
        int height = (int)position.height;

        var texture = new Texture2D(width, height);
        var center = new Vector2Int((int)(width * .5f), (int)(height * .5f));
        var max = new Vector2Int(width, height);

        float radius = (max - center).magnitude;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int diff = new Vector2Int(x, y) - center;

                float length = diff.magnitude / radius;

                texture.SetPixel(x, y, gradient.Evaluate(length));
            }
        }

        texture.Apply();
        return texture;
    }

    private Gradient ParseGradient(string cssGradientCode)
    {
        var matches = Regex.Matches(cssGradientCode, @"#([0-9a-fA-F]{6}) (\d{1,3})%");

        if (matches.Count > 0)
        {
            var gradient = new Gradient();
            var colorKeys = new GradientColorKey[matches.Count];

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                float position = int.Parse(match.Groups[2].Value) / 100f;

                Color color = HexToColor(match.Groups[1].Value);
                colorKeys[i].color = color;
                colorKeys[i].time = position;
            }

            gradient.SetKeys(colorKeys, gradient.alphaKeys);

            return gradient;
        }
        else
        {
            Debug.LogError("Unable to parse CSS Gradient");
            return null;
        }
    }

    private Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        return new Color(r / 255f, g / 255f, b / 255f, 1f);
    }
}