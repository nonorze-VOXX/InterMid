using System;
using System.Collections.Generic;
using CoolEffect;
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

    internal enum PlayerText
    {
        Score,
        Atk
    }

    internal enum PlayerAnimation
    {
        Idle,
        Attack,
        Damaged,
        Die
    }

    public class Player : MonoBehaviour
    {
        private static readonly float DiceDistance = 1.5f;
        private static readonly int fullHp = 100;
        private static readonly float initAtk = 1;
        [SerializeField] private TMP_Text scoreText;

        [SerializeField] private TMP_Text statusText;

        private readonly List<Dice> dices = new();

        private Slider hpSlider;

        private bool isAttacking;
        private Player opponent;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            attackSprites = new List<Sprite>(Resources.LoadAll<Sprite>("Image/King/Attack"));
            idleSprites = new List<Sprite>(Resources.LoadAll<Sprite>("Image/King/Idle"));
            damagedSprites = new List<Sprite>(Resources.LoadAll<Sprite>("Image/King/Hit"));
            dieSprites = new List<Sprite>(Resources.LoadAll<Sprite>("Image/King/Dead"));
            hpSlider = GetComponentInChildren<Slider>();
            GetComponentInChildren<TMP_Text>().enabled = true;

            #region GetText

            var texts = GetComponentsInChildren<TMP_Text>();
            statusText = texts[(int)PlayerText.Atk];
            scoreText = texts[(int)PlayerText.Score];

            #endregion

            InvokeRepeating("UpdateAnimation", 0.0f, 0.1f);
            Hp = fullHp;
            Atk = initAtk;
            score = 0;
        }

        public void SetAttack(bool b)
        {
            isAttacking = b;
        }

        public void SetOpponent(Player p)
        {
            opponent = p;
        }

        public Dice GetDice()
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

            if (isAttacking)
            {
                SetAnimation(PlayerAnimation.Attack);
                dice.Shoot(opponent.GetUseDicePos(), DiceMoveSpeed.Fast, () =>
                    {
                        if (dice.GetValue() > opponent.GetValue() || dice.GetTrueDamage())
                            dice.Shoot(opponent.GetPos(), DiceMoveSpeed.Normal, () =>
                                {
                                    if (dice.GetTrueDamage())
                                        opponent.Damage((int)(dice.GetDamage() * Atk));
                                    else
                                        opponent.Damage((int)(dice.GetDamage() * Atk) - opponent.GetValue());

                                    dice.UseSkill(this);
                                    OnTurnEnd?.Invoke();
                                }
                            );
                        else
                            dice.Shoot(GetAttackFailPos(), DiceMoveSpeed.Fast, () => { OnTurnEnd?.Invoke(); }
                            );
                    }
                );
            }
            else
            {
                dice.Shoot(GetUseDicePos(), DiceMoveSpeed.Fast, () =>
                {
                    dice.UseSkill(this);
                    OnTurnEnd?.Invoke();
                });
            }
        }


        private void Damage(int value)
        {
            EffectManager.Instance.CreateRedText("-" + value, transform.position);
            Hp -= value;
            SetAnimation(PlayerAnimation.Damaged);
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
            Atk = initAtk;
            SetAnimation(PlayerAnimation.Idle);
        }

        public void ChangeAttackState()
        {
            isAttacking = !isAttacking;
        }

        public void AddAttack(float i)
        {
            EffectManager.Instance.CreateRedText("+" + i, transform.position);
            Atk += i;
        }

        public void MultiplyAttack(float num)
        {
            EffectManager.Instance.CreateRedText("*" + num, transform.position);
            Atk *= num;
        }

        public void AddHp(int num)
        {
            if (Hp <= 0) return;
            Hp += num;
            Hp = Mathf.Min(Hp, fullHp);
            EffectManager.Instance.CreateGreenText("Hp + " + num, transform.position);
        }

        public void FullHeal()
        {
            Hp = fullHp;
            EffectManager.Instance.CreateGreenText("Full Heal", transform.position);
        }

        public bool IsAtkState()
        {
            return isAttacking;
        }

        public void AtkReset()
        {
            Atk = initAtk;
        }

        public int GetDiceCount()
        {
            return dices.Count;
        }

        public void ResetScore()
        {
            score = 0;
        }

        #region Animation

        private PlayerAnimation animationState = PlayerAnimation.Idle;
        private List<Sprite> attackSprites;
        private List<Sprite> dieSprites;
        private List<Sprite> damagedSprites;
        private List<Sprite> idleSprites;

        private int animationIndex;

        private void SetAnimation(PlayerAnimation state)
        {
            if (animationState == PlayerAnimation.Die && state == PlayerAnimation.Damaged) return;
            animationState = state;
            animationIndex = 0;
            spriteRenderer.sprite = state switch
            {
                PlayerAnimation.Idle => idleSprites[0],
                PlayerAnimation.Attack => attackSprites[0],
                PlayerAnimation.Die => dieSprites[0],
                PlayerAnimation.Damaged => damagedSprites[0],
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void UpdateAnimation()
        {
            animationIndex++;
            switch (animationState)
            {
                case PlayerAnimation.Idle:
                    spriteRenderer.sprite = idleSprites[animationIndex % idleSprites.Count];
                    break;
                case PlayerAnimation.Attack:
                    if (animationIndex >= attackSprites.Count)
                    {
                        SetAnimation(PlayerAnimation.Idle);
                        return;
                    }

                    spriteRenderer.sprite = attackSprites[animationIndex % attackSprites.Count];
                    break;
                case PlayerAnimation.Die:
                    if (animationIndex >= dieSprites.Count) animationIndex = dieSprites.Count - 1;
                    spriteRenderer.sprite = dieSprites[animationIndex % dieSprites.Count];
                    break;
                case PlayerAnimation.Damaged:
                    if (animationIndex >= damagedSprites.Count * 3)
                    {
                        SetAnimation(PlayerAnimation.Idle);
                        return;
                    }

                    spriteRenderer.sprite = damagedSprites[animationIndex % damagedSprites.Count];
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Properties

        private float _atk = 1;
        private int _hp;
        private int _score;

        private float Atk
        {
            get => _atk;
            set
            {
                _atk = value;
                UpdateStatus();
            }
        }

        public int score
        {
            get => _score;
            set
            {
                _score = value;
                scoreText.text = "Score: " + value;
            }
        }

        public int Hp
        {
            get => _hp;
            set
            {
                _hp = value;
                _hp = Math.Max(0, _hp);
                hpSlider.value = _hp / (float)fullHp;
                if (_hp <= 0) SetAnimation(PlayerAnimation.Die);
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            statusText.text = name + ", Atk: " + Atk + ", Hp: " + Hp;
        }

        #endregion

        #region debug

        private bool nextTriple;

        public void NextThrowTripleDice()
        {
            nextTriple = true;
        }

        #endregion

        #region throw_dice

        public const int maxDiceCount = 3;
        private int throwCount;

        [SerializeField] private Dice dicePrefab;

        public void ResetThrowCount()
        {
            throwCount = 0;
        }

        private void CheckDiceIsTriple(List<Dice> dices, UnityAction onPrepared)
        {
            if (dices.Count != 3) return;
            // check all value is same
            if (dices[0].GetValue() == dices[1].GetValue() && dices[1].GetValue() == dices[2].GetValue())
                // set dice type to triple
            {
                foreach (var dice in dices)
                    dice.SetDiceType(DiceType.Triple);
                dices[1].MergeTo(dices[0], () =>
                {
                    dices[2].MergeTo(dices[0], () =>
                    {
                        var t = dices[2];
                        dices.RemoveAt(2);
                        Destroy(t.gameObject);
                        t = dices[1];
                        dices.RemoveAt(1);
                        Destroy(t.gameObject);
                        SkillManager.SetDiceFunc(dices[0], this);

                        onPrepared?.Invoke();
                    });
                });
            }
            else
            {
                onPrepared?.Invoke();
            }
        }

        private void MergeDice(UnityAction onPrepared)
        {
            checkCount = 0;
            CheckDiceIsTriple(dices, OnCheckDone(onPrepared));
            CheckDiceIsPair(dices, OnCheckDone(onPrepared));
        }

        private int checkCount;

        private UnityAction OnCheckDone(UnityAction onPrepared)
        {
            return () =>
            {
                checkCount++;
                if (checkCount == 2) onPrepared?.Invoke();
            };
        }

        private void CheckDiceIsPair(List<Dice> dices, UnityAction onPrepared)
        {
            // onPrepared?.Invoke();
            // return;
            if (dices[0].GetValue() == dices[1].GetValue() &&
                dices[0].GetDiceType() == DiceType.Single
                && dices[1].GetDiceType() == DiceType.Single)
            {
                dices[0].SetDiceType(DiceType.Pair);
                dices[1].MergeTo(dices[0], () =>
                {
                    var t = dices[1];
                    dices.RemoveAt(1);
                    Destroy(t.gameObject);
                    SkillManager.SetDiceFunc(dices[0], this);
                    onPrepared?.Invoke();
                });
            }
            else if (dices[1].GetValue() == dices[2].GetValue()
                     && dices[1].GetDiceType() == DiceType.Single
                     && dices[2].GetDiceType() == DiceType.Single)
            {
                dices[1].SetDiceType(DiceType.Pair);
                dices[2].MergeTo(dices[1], () =>
                {
                    var t = dices[2];
                    dices.RemoveAt(2);
                    Destroy(t.gameObject);
                    SkillManager.SetDiceFunc(dices[0], this);
                    onPrepared?.Invoke();
                });
            }
            else if (dices[0].GetValue() == dices[2].GetValue()
                     && dices[0].GetDiceType() == DiceType.Single
                     && dices[2].GetDiceType() == DiceType.Single)
            {
                dices[0].SetDiceType(DiceType.Pair);
                dices[2].MergeTo(dices[0], () =>
                {
                    var t = dices[2];
                    dices.RemoveAt(2);
                    Destroy(t.gameObject);
                    SkillManager.SetDiceFunc(dices[0], this);
                    onPrepared?.Invoke();
                });
            }
            else
            {
                onPrepared?.Invoke();
            }
        }

        public void Prepare(UnityAction onPrepared)
        {
            if (throwCount >= maxDiceCount)
            {
                nextTriple = false;
                // onPrepared?.Invoke();
                MergeDice(onPrepared);
                return;
            }

            var dice = Instantiate(dicePrefab);
            var transformPosition = transform.position + new Vector3(0, DiceDistance * (1 + throwCount));
            dice.transform.position = transformPosition;

            var sameBefore = nextTriple && throwCount != 0;
            throwCount++;

            dice.SetColorByAttack(isAttacking);
            dices.Add(dice);
            dice.Roll(() =>
            {
                if (sameBefore) dice.SetValue(dices[0].GetValue());
                OnRollEnd(onPrepared)?.Invoke();
            });
        }

        private UnityAction OnRollEnd(UnityAction unityAction)
        {
            return () => { Prepare(unityAction); };
        }

        #endregion
    }
}