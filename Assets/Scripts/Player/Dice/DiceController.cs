using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DiceController
{
    private readonly DiceModel model;
    private readonly DiceView view;

    public DiceController(DiceView view)
    {
        var diceModel = new DiceModel();
        model = diceModel;
        this.view = view;

        diceModel.AddListener(view.SetText);
    }

    public void Roll()
    {
        var value = Random.Range(1, 7);
        model.SetValue(value);
    }

    public IEnumerator RollWithCoroutine(UnityAction onRollEnd)
    {
        for (var i = 0; i < 10; i++)
        {
            var value = Random.Range(1, 7);
            model.SetValue(value);
            yield return new WaitForSeconds(0.1f);
        }

        onRollEnd?.Invoke();
    }

    public void AddOnMoveDoneListener(UnityAction action)
    {
        view.AddListener(action);
    }

    public void SetViewActive(bool b)
    {
        view.gameObject.SetActive(b);
    }

    public void Roll(UnityAction onRollEnd)
    {
        view.RollWithCoroutine(RollWithCoroutine(onRollEnd));
    }
}