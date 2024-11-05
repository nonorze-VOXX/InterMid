using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static float DiceDistance = 1.5f;
    public static int fullHp = 100;

    [SerializeField] private GameObject _DiceViewPrefab;
    public Player enemy;

    [SerializeField] private int _hp;
    [SerializeField] private int _score;


    private readonly List<DiceView> _diceViews = new();
    [SerializeField] private PlayerMachine _playerMachine;
    private TMP_Text _stateText;
    private Slider hpSlider;
    private bool isBeforeBattleSignal;
    private UnityAction<float> onHpChange;

    private UnityAction<Player> OnPlayerDie;

    private UnityAction<Player> OnPlayerPrepared;

    private UnityAction OnTurnAddOne;

    public int score
    {
        get => _score;
        set => _score = value;
    }

    [SerializeField]
    public int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            onHpChange?.Invoke((float)_hp / fullHp);
        }
    }

    [SerializeField] public int Atk { get; set; }


    public void Awake()
    {
        hpSlider = GetComponentInChildren<Slider>();
        onHpChange += OnHpChange;
        _playerMachine = new PlayerMachine(this);
        _stateText = GetComponentInChildren<TMP_Text>();
        _playerMachine.AddListener(OnStateChange);

        Hp = fullHp;
        Atk = 100;

        // DiceInit();
    }


    private void Start()
    {
        _playerMachine.OnStart();
    }

    private void Update()
    {
        if (isBeforeBattleSignal)
            _playerMachine.Update();
    }

    private void OnDestroy()
    {
        onHpChange = null;
        OnPlayerDie = null;
        OnPlayerPrepared = null;
        OnTurnAddOne = null;
    }

    public void OnHpChange(float percent)
    {
        hpSlider.value = percent;
    }

    private void DiceInit()
    {
        _diceViews.Clear();
        for (var i = 0; i < 3; i++)
        {
            var transformPosition = transform.position + new Vector3(0, DiceDistance * (1 + i));
            var diceView = Instantiate(_DiceViewPrefab, transformPosition,
                transform.rotation, transform).GetComponent<DiceView>();
            diceView.gameObject.SetActive(false);
            _diceViews.Add(diceView);
        }
    }

    public void SetDebug(bool b)
    {
        var componentInChildren = GetComponentInChildren<TMP_Text>();
        componentInChildren.enabled = b;
    }

    private void OnStateChange(string arg0)
    {
        // print("state change" + arg0);
        _stateText.text = arg0;
    }


    public void AddListener(UnityAction<Player> action)
    {
        OnPlayerPrepared += action;
    }

    public void Prepared()
    {
        OnPlayerPrepared?.Invoke(this);
    }

    public List<DiceView> GetDiceView()
    {
        return _diceViews;
    }

    public void ReceiveBeforeBattleSignal()
    {
        isBeforeBattleSignal = true;
    }

    public void SetAttacker()
    {
        _playerMachine.CombatState = COMBAT_STATE.Attack;
    }

    public void SetDefender()
    {
        _playerMachine.CombatState = COMBAT_STATE.Defend;
    }

    public Vector2 GetDiceUsingPosition()
    {
        return _playerMachine.GetUsingDicePosition();
    }

    public int GetUsingDiceValue()
    {
        return _playerMachine.Dices.First().GetDiceValue();
    }

    public void AddPlayerDieListener(UnityAction<Player> action)
    {
        OnPlayerDie += action;
    }

    public void TakeDamage(int atk)
    {
        Hp -= atk;
        if (Hp <= 0)
        {
            Hp = 0;

            OnPlayerDie?.Invoke(this);
        }
    }

    public void OnDiceDestroy(DiceView arg0)
    {
        _diceViews.Remove(arg0);
        Destroy(arg0.gameObject);
    }

    public void DefenderPreparedAcceptAttack()
    {
        _playerMachine.AttackerReceiveDefenderPrepared();
    }

    public List<DiceView> GetNewDiceView()
    {
        DiceInit();
        return _diceViews;
    }

    public void ToThrowState()
    {
        _playerMachine.ToThrowState();
    }

    public void DefenderGetAttackerShootDone()
    {
        _playerMachine.DefenderGetAttackerShootDone();
    }

    public void NextTurn(COMBAT_STATE nextState)
    {
        _playerMachine.NextTurn(nextState);
    }

    public void AddTurnAddOneListener(UnityAction action)
    {
        OnTurnAddOne += action;
    }

    public void TurnEnd()
    {
        OnTurnAddOne?.Invoke();
    }

    public COMBAT_STATE GetCombatState()
    {
        return _playerMachine.CombatState;
    }
}