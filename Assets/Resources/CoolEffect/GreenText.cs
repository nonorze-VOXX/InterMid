using System.Collections;
using TMPro;
using UnityEngine;

namespace CoolEffect
{
    public class GreenText : MonoBehaviour
    {
        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            StartCoroutine(DestroyAfter3S());
        }

        private void Update()
        {
            transform.position = (Vector2)transform.position + Vector2.up * (Time.deltaTime * 100);
        }

        public void SetText(string s)
        {
            text.text = s;
        }

        private IEnumerator DestroyAfter3S()
        {
            for (var i = 0; i < 30; i++)
            {
                var c = text.color;
                c.a -= 0.03f;
                text.color = c;
                yield return new WaitForSeconds(0.1f);
            }

            Destroy(gameObject);
        }
    }
}