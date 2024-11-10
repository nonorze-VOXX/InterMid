using UnityEngine;

namespace CoolEffect
{
    public class EffectManager : MonoBehaviour
    {
        private static EffectManager _instance;

        private EffectText _effectTextPrefab;
        private Canvas canvas;

        public static EffectManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<EffectManager>();
                if (_instance == null)
                {
                    var go = new GameObject("EffectManager");
                    _instance = go.AddComponent<EffectManager>();
                    var c = new GameObject("effect canvas");
                    _instance.canvas = c.AddComponent<Canvas>();
                    _instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    _instance._effectTextPrefab = Resources.Load<EffectText>("CoolEffect/EffectText");
                }

                return _instance;
            }
        }

        public void CreateRedText(string s, Vector2 worldPosition)
        {
            var go = CreateText(s, worldPosition);
            var colorHex = "#FF5F5F";
            if (ColorUtility.TryParseHtmlString(colorHex, out var newColor))
                go.SetColor(newColor);
            else
                Debug.LogError("Invalid color code.");
        }

        public void CreateGreenText(string s, Vector2 worldPosition)
        {
            var go = CreateText(s, worldPosition);
            var colorHex = "#5FFF62";
            if (ColorUtility.TryParseHtmlString(colorHex, out var newColor))
                go.SetColor(newColor);
            else
                Debug.LogError("Invalid color code.");
        }

        public EffectText CreateText(string s, Vector2 worldPosition)
        {
            var go = Instantiate(_effectTextPrefab, canvas.transform);
            go.SetText(s);
            // world space vector2  to cameraspace v2
            var screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            go.transform.position = screenPosition;
            return go;
        }
    }
}