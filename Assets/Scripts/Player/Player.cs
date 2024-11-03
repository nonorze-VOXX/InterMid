using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static float DiceDistance = 1.5f;
    [SerializeField] private PlayerMachine _playerMachine;

    [SerializeField] private GameObject _DiceViewPrefab;


    private readonly List<DiceView> _diceViews = new();
    private TMP_Text _stateText;
    private bool isBeforeBattleSignal;

    private UnityAction<Player> OnPlayerPrepared;

    [SerializeField] public int Hp { get; set; }
    [SerializeField] public int Atk { get; set; }

    public void Awake()
    {
        Hp = 100;
        Atk = 10;
        _playerMachine = new PlayerMachine(this);
        _stateText = GetComponentInChildren<TMP_Text>();
        _playerMachine.AddListener(OnStateChange);
        for (var i = 0; i < 3; i++)
        {
            var transformPosition = transform.position + new Vector3(0, DiceDistance * (1 + i));
            var diceView = Instantiate(_DiceViewPrefab, transformPosition,
                transform.rotation, transform).GetComponent<DiceView>();
            diceView.gameObject.SetActive(false);
            _diceViews.Add(diceView);
        }

        _playerMachine.OnStart();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.S)) Prepared();
        if (isBeforeBattleSignal)
            _playerMachine.Update();
    }

    public void SetDebug(bool b)
    {
        var componentInChildren = GetComponentInChildren<Canvas>();
        componentInChildren.enabled = b;
    }

    private void OnStateChange(string arg0)
    {
        print("state change" + arg0);
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
}