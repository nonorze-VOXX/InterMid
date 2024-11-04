using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static float DiceDistance = 1.5f;
    [SerializeField] private PlayerMachine _playerMachine;

    [SerializeField] private GameObject _DiceViewPrefab;
    public Player enemy;


    private readonly List<DiceView> _diceViews = new();
    private TMP_Text _stateText;
    private bool isBeforeBattleSignal;

    private UnityAction<Player> OnPlayerPrepared;

    [SerializeField] public int Hp { get; set; }
    [SerializeField] public int Atk { get; set; }

    public void Awake()
    {
        Hp = 100;
        Atk = 1;
        _playerMachine = new PlayerMachine(this);
        _stateText = GetComponentInChildren<TMP_Text>();
        _playerMachine.AddListener(OnStateChange);

        // DiceInit();

        _playerMachine.OnStart();
    }

    private void Update()
    {
        if (isBeforeBattleSignal)
            _playerMachine.Update();
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
        var componentInChildren = GetComponentInChildren<Canvas>();
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
        _playerMachine.CombatState = CombatState.Attack;
    }

    public void SetDefender()
    {
        _playerMachine.CombatState = CombatState.Defend;
    }

    public Vector2 GetDiceUsingPosition()
    {
        return _playerMachine.GetUsingDicePosition();
    }

    public int GetUsingDiceValue()
    {
        return _playerMachine.Dices.First().GetDiceValue();
    }

    public void TakeDamage(int atk)
    {
        Hp -= atk;
        if (Hp <= 0)
        {
            Hp = 0;
            Debug.Log("Player Die");
        }
    }

    public void OnDiceDestroy(DiceView arg0)
    {
        _diceViews.Remove(arg0);
        print("destroy dice view" + arg0 + " " + arg0.transform.position);
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

    public void NextTurn()
    {
        _playerMachine.NextTurn();
    }
}