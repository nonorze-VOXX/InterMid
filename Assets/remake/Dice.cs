using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace remake
{
    public enum DiceType
    {
        Single,
        Pair,
        Triple
    }

    public class Dice : MonoBehaviour
    {
        public float diceSpeed = 5;
        private DiceType _diceType;
        private TMP_Text _text;
        private int _value;
        private UnityAction<Player, Dice> diceSkills;
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
            _diceType = DiceType.Single;
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
                    diceSpeed = 20;
                    break;
            }
        }


        public void Shoot(Vector2 getUsingDicePosition, DiceMoveSpeed speed, UnityAction onShootEndAction)
        {
            SetTargetPosition(getUsingDicePosition);
            SetMoveSpeed(speed);
            moveable = true;
            AddListener(onShootEndAction);
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

        public void SetDiceType(DiceType newType)
        {
            _diceType = newType;
        }

        public DiceType GetDiceType()
        {
            return _diceType;
        }

        public void Add(Dice dice)
        {
            value += dice.GetValue();
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public void MergeTo(Dice dice, UnityAction onMergeDone)
        {
            Shoot(dice.transform.position, DiceMoveSpeed.Normal, () =>
            {
                dice.Add(this);
                gameObject.SetActive(false);
                onMergeDone?.Invoke();
            });
        }

        public void SetColorByAttack(bool isAttacking)
        {
            var image = GetComponentInChildren<SpriteRenderer>();

            var colorHex = isAttacking ? "#933E3E" : "#578DD4";

            if (ColorUtility.TryParseHtmlString(colorHex, out var newColor))
                image.color = newColor;
            else
                Debug.LogError("Invalid color code.");
        }

        public void AddFunction(UnityAction<Player, Dice> skillItem2)
        {
            diceSkills += skillItem2;
        }

        public void UseSkill(Player player)
        {
            diceSkills?.Invoke(player, this);
        }

        #region trueDamage

        private bool trueDamage;

        public bool GetTrueDamage()
        {
            return trueDamage;
        }

        public void SetTrueDamage()
        {
            trueDamage = true;
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }

        #endregion
    }
}