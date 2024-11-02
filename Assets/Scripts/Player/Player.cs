using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static float DiceDistance = 1.5f;
    [SerializeField] private PlayerMachine _playerMachine;

    [SerializeField] private GameObject _DiceViewPrefab;

    private readonly List<DiceView> _diceViews = new();
    private bool isBeforeBattleSignal;

    private UnityAction<Player> OnPlayerPrepared;

    [SerializeField] public int Hp { get; set; }
    [SerializeField] public int Atk { get; set; }


    public void Start()
    {
        Hp = 100;
        Atk = 10;
        _playerMachine = new PlayerMachine(this);
        for (var i = 0; i < 3; i++)
        {
            var transformPosition = transform.position + new Vector3(DiceDistance * (1 + i), 0, 0);
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


    public void AddListener(UnityAction<Player> action)
    {
        OnPlayerPrepared += action;
    }

    private void Prepared()
    {
        OnPlayerPrepared.Invoke(this);
    }

    public List<DiceView> GetDiceView()
    {
        return _diceViews;
    }

    public void ReceiveBeforeBattleSignal()
    {
        isBeforeBattleSignal = true;
    }
}