using UnityEngine;

namespace CoolEffect
{
    public class EffectManager : MonoBehaviour
    {
        private static EffectManager _instance;
        private Canvas canvas;

        private GreenText greenTextPrefab;

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
                    _instance.greenTextPrefab = Resources.Load<GreenText>("CoolEffect/GreenText");
                }

                return _instance;
            }
        }

        public void CreateGreenText(string s, Vector2 worldPosition)
        {
            print(greenTextPrefab);

            var go = Instantiate(greenTextPrefab, canvas.transform);
            go.SetText(s);
            // world space vector2  to cameraspace v2

            var screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            go.transform.position = screenPosition;
        }
    }
}