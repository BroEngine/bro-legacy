using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client
{
    public class CheatButton : CheatElement
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Button _button;
        
        public void SetTitle(string title)
        {
            _label.text = title;
        }

        public void SetCallback(params Action[] callbacks)
        {
            foreach (var callback in callbacks)
            {
                _button.onClick.AddListener(()=>callback());
            }
        }
    }
}