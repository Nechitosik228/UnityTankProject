using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private PlayerController _player;

    private void Start()
    {
        _slider.maxValue = _player.Health;
        _slider.value = _player.Health;
    }

    private void Update()
    {
        _slider.value = _player.Health;
    }
}