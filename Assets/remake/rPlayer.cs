using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace remake
{
    public enum DiceMoveSpeed
    {
        Normal,
        Fast
    }

    public class rPlayer : MonoBehaviour
    {
        public static float DiceDistance = 1.5f;
        public static int fullHp = 100;
        private readonly int Atk = 1;
        private readonly List<rDice> dices = new();
        private int _hp;
        private int _score;
        private Slider hpSlider;

        private bool is_attacking;
        private rPlayer opponent;

        public int score
        {
            get => _score;
            set
            {
                GetComponentInChildren<TMP_Text>().text = "Score: " + value;
                _score = value;
            }
        }

        public int Hp
        {
            get => _hp;
            set
            {
                _hp = value;
                hpSlider.value = _hp / (float)fullHp;
            }
        }

        private void Awake()
        {
            hpSlider = GetComponentInChildren<Slider>();
            GetComponentInChildren<TMP_Text>().enabled = true;

            Hp = fullHp;
            score = 0;
        }

        public void SetAttack(bool b)
        {
            is_attacking = b;
        }

        public void SetOpponent(rPlayer p)
        {
            opponent = p;
        }

        public rDice GetDice()
        {
            if (dices.Count == 0) return null;
            return dices[0];
        }
        // UnityAction OnBattleEnd;

        public void Turn(UnityAction OnTurnEnd)
        {
            var dice = GetDice();
            if (!dice)
            {
                OnTurnEnd?.Invoke();
                return;
            }

            if (is_attacking)
                dice.Shoot(opponent.GetUseDicePos(), DiceMoveSpeed.Fast, () =>
                    {
                        if (dice.GetValue() > opponent.GetValue())
                            dice.Shoot(opponent.GetPos(), DiceMoveSpeed.Normal, () =>
                                {
                                    opponent.Damage(dice.GetDamage() + Atk);
                                    OnTurnEnd?.Invoke();
                                }
                            );
                        else
                            dice.Shoot(GetAttackFailPos(), DiceMoveSpeed.Fast, () => { OnTurnEnd?.Invoke(); }
                            );
                    }
                );
            else
                dice.Shoot(GetUseDicePos(), DiceMoveSpeed.Fast, () => { });
        }


        private void Damage(int value)
        {
            Hp -= value;
        }

        private Vector2 GetPos()
        {
            return transform.position;
        }

        private Vector2 GetAttackFailPos()
        {
            return transform.position + new Vector3(5, 0) *
                                      Vector2.Dot(transform.position.normalized, Vector2.right)
                                      + new Vector3(0, 10)
                ;
        }

        public void UsedDice()
        {
            var dice = GetDice();
            if (!dice) return;
            dices.RemoveAt(0);
            Destroy(dice.gameObject);
        }

        private int GetValue()
        {
            var dice = GetDice();
            if (dice) return dice.GetValue();

            return 0;
        }

        private Vector2 GetUseDicePos()
        {
            return transform.position + new Vector3(-1, 0) *
                Vector2.Dot(transform.position.normalized, Vector2.right)
                ;
        }


        public void NewRound()
        {
            foreach (var dice in dices) Destroy(dice.gameObject);
            dices.Clear();
            Hp = fullHp;
        }

        public void ChangeAttack()
        {
            is_attacking = !is_attacking;
        }

        #region throw_dice

        public const int maxDiceCount = 3;
        private int throwCount;

        [SerializeField] private rDice dicePrefab;

        public void ResetThrowCount()
        {
            throwCount = 0;
        }

        public void Prepare(UnityAction onPrepared)
        {
            if (throwCount >= maxDiceCount)
            {
                onPrepared?.Invoke();
                return;
            }

            throwCount++;
            var dice = Instantiate(dicePrefab);
            var transformPosition = transform.position + new Vector3(0, DiceDistance * (1 + throwCount));
            dice.transform.position = transformPosition;

            var image = dice.GetComponentInChildren<SpriteRenderer>();

            var colorHex = is_attacking ? "#933E3E" : "#578DD4";

            if (ColorUtility.TryParseHtmlString(colorHex, out var newColor))
                image.color = newColor;
            else
                Debug.LogError("Invalid color code.");
            dices.Add(dice);
            dice.Roll(OnRollEnd(onPrepared));
        }

        private UnityAction OnRollEnd(UnityAction unityAction)
        {
            return () => { Prepare(unityAction); };
        }

        #endregion
    }
}