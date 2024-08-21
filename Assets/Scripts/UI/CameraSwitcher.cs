using System;
using System.Resources;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _camera1;
        [SerializeField] private GameObject _camera2;
        [SerializeField] private Toggle _toggle1;
        [SerializeField] private Toggle _toggle2;

        private void Start()
        {
            _toggle1.onValueChanged.AddListener(arg0 =>
            {
                _camera1.SetActive(arg0);
                _camera2.SetActive(!arg0);
            });
            _toggle2.onValueChanged.AddListener(arg0 =>
            {
                _camera1.SetActive(!arg0);
                _camera2.SetActive(arg0);
            });
        }
    }
}