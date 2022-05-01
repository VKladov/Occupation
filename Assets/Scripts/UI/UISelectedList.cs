using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UISelectedList : MonoBehaviour
{
    [Inject] private PersonSelection _personSelection;
    [SerializeField] private UIPersonRow _personRow;

    private void Refresh(List<Person> list)
    {
        foreach (Transform children in transform)
        {
            Destroy(children.gameObject);
        }

        foreach (var person in list)
        {
            var row = Instantiate(_personRow, transform);
            row.ShowData(person);
        }
    }

    private void OnEnable()
    {
        Refresh(_personSelection.Selected);
        _personSelection.Changed += Refresh;
    }

    private void OnDisable()
    {
        _personSelection.Changed -= Refresh;
    }
}
