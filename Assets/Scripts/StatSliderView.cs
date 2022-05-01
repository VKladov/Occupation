using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSliderView : MonoBehaviour
{
    [SerializeField] private Image _slider;
    [SerializeField] private TMP_Text _label;
    
    private Stat _stat;

    public void SetSource(Stat stat)
    {
        _stat = stat;
        _label.text = stat?.Name ?? "";
        _slider.fillAmount = _stat?.NormalizedValue ?? 0f;
    }

    private void Update()
    {
        if (_stat == null)
        {
            return;
        }

        _slider.fillAmount = _stat.NormalizedValue;
    }
}
