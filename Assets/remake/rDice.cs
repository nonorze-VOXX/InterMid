using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace remake
{
    public class rDice : MonoBehaviour
    {
        public float diceSpeed = 5;
        private TMP_Text _text;
        private int _value;
        private bool moveable;
        private UnityAction OnDicePrepared;
        private Vector2 pos = new(0, 0);

        private int value
        {
            get => _value;
            set
            {
                _value = value;
                _text.text = value.ToString();
            }
        }

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
        }

        private void Update()
        {
            if (moveable)
            {
                transform.position = Vector2.MoveTowards(transform.position, pos, diceSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, pos) < 0.1f)
                {
                    moveable = false;
                    // for function call and shoot again, action will be null problem
                    var tmpAction = OnDicePrepared;
                    OnDicePrepared = null;
                    tmpAction?.Invoke();
                }
            }
            // move towards target position
        }

        private void OnDestroy()
        {
            OnDicePrepared = null;
        }

        public int GetValue()
        {
            return value;
        }

        public void SetMoveSpeed(DiceMoveSpeed speed)
        {
            switch (speed)
            {
                case DiceMoveSpeed.Normal:
                    diceSpeed = 5;
                    break;
                case DiceMoveSpeed.Fast:
                    diceSpeed = 50;
                    break;
            }
        }


        public void Shoot(Vector2 getUsingDicePosition, DiceMoveSpeed speed, UnityAction action)
        {
            SetTargetPosition(getUsingDicePosition);
            SetMoveSpeed(speed);
            moveable = true;
            AddListener(action);
        }

        private void AddListener(UnityAction action)
        {
            OnDicePrepared += action;
        }

        private void SetTargetPosition(Vector2 getUsingDicePosition)
        {
            pos = getUsingDicePosition;
        }

        public int GetDamage()
        {
            return value;
        }

        public void Roll(UnityAction onRollEnd)
        {
            StartCoroutine(RollWithCoroutine(onRollEnd));
        }

        public IEnumerator RollWithCoroutine(UnityAction onRollEnd)
        {
            for (var i = 0; i < 5; i++)
            {
                var v = Random.Range(1, 7);
                value = v;
                yield return new WaitForSeconds(0.1f);
            }

            onRollEnd?.Invoke();
        }
    }
}