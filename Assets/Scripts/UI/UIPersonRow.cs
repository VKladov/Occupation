using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPersonRow : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private Color _deathTextColor;
    [SerializeField] private Color _warningTextColor;
    [SerializeField] private Color _criticalTextColor;
    [SerializeField] private TMP_Text _healthLabel;
    [SerializeField] private TMP_Text _hungerLabel;
    [SerializeField] private TMP_Text _tiredLabel;

    private Person _person;

    public void ShowData(Person person)
    {
        _person = person;
        _nameLabel.text = person.Name;
        _person.Health.StateChanged += Refresh;
        _person.Health.Changed += UpdateHealthValue;
        Refresh();
        UpdateHealthValue();
    }

    private void Refresh()
    {
        _healthLabel.text = _person.Health.State.Name;
        _healthLabel.color = _person.Health.State.TextColor;
        _hungerLabel.color = _person.Health.State.TextColor;
    }

    private void UpdateHealthValue()
    {
        _hungerLabel.text = Mathf.CeilToInt(_person.Health.Value).ToString();
    }

    private void OnDestroy()
    {
        _person.Health.StateChanged -= Refresh;
        _person.Health.Changed -= UpdateHealthValue;
    }
}
