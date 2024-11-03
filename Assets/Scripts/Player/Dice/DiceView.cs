using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DiceView : MonoBehaviour
{
    private TMP_Text _text;

    private UnityAction OnDicePrepared;

    private Vector2 position = new(0, 0);

    public bool moveable { get; set; }

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (moveable)
        {
            transform.position = Vector2.MoveTowards(transform.position, position, 5 * Time.deltaTime);

            if (Vector2.Distance(transform.position, position) < 0.1f)
            {
                // arrived at target position
                // do something
                moveable = false;
                var tmpAction = OnDicePrepared;
                OnDicePrepared = null;
                tmpAction?.Invoke();
            }
        }
        // move towards target position
    }

    public void SetText(int value)
    {
        _text.text = value.ToString();
    }

    public void SetTargetPosition(Vector2 targetPosition)
    {
        position = targetPosition;
    }

    public void AddListener(UnityAction action)
    {
        OnDicePrepared += action;
    }

    public void RollWithCoroutine(IEnumerator rollWithCoroutine)
    {
        StartCoroutine(rollWithCoroutine);
    }
}