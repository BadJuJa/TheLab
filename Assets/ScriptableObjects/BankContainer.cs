using UnityEngine;

[CreateAssetMenu(fileName = "New Bank", menuName = "Economy/Bank")]
public class BankContainer : ScriptableObject {

    public delegate void CrystalsChanged(int value);

    public event CrystalsChanged OnCrystalsChanged;

    private int value;

    public void Add(int value)
    {
        this.value += value;
        OnCrystalsChanged?.Invoke(Get());
    }

    public void Take(int value)
    {
        this.value -= value;
        OnCrystalsChanged?.Invoke(Get());
    }

    public int Get() => value;
}