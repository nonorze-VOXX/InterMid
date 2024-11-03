using System;
using UnityEngine.Events;

[Serializable]
public class DiceModel
{
    private UnityAction<int> OnDiceValueChange;
    private int value;

    public DiceModel()
    {
        value = 0;
    }


    public void AddListener(UnityAction<int> action)
    {
        OnDiceValueChange += action;
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value)
    {
        this.value = value;
        OnDiceValueChange?.Invoke(value);
    }
}