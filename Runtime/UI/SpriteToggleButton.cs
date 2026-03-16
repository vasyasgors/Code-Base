using UnityEngine;
using UnityEngine.UI;


namespace CodeBase.UI
{

    [RequireComponent(typeof(Button), typeof(Image))]
    public class ToggleButton : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        [Header("Colors")]
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = Color.white;

        private Image _image;
        private Button _button;
        private bool _isOn;

        public bool Toggle
        {
            get => _isOn;
            set
            {
                if (_isOn == value) return;
                _isOn = value;
                ApplyState();
            }
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);

            ApplyState();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Toggle = !_isOn;
        }

        private void ApplyState()
        {
            if (_image == null) return;

            _image.sprite = _isOn ? onSprite : offSprite;
            _image.color = _isOn ? onColor : offColor;
        }
    }

}