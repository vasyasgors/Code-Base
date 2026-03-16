using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace CodeBase.UI
{

    [RequireComponent(typeof(Button), typeof(Image))]
    public class ToggleButton : MonoBehaviour
    {
        public event UnityAction<bool> Changed;

        [Header("Sprites")]
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        [Header("Colors")]
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = Color.white;

        [SerializeField] private bool isOn;

        private Image _image;
        private Button _button;

        public bool Toggle
        {
            get => isOn;
            set
            {
                if (isOn == value) return;
                isOn = value;

                Changed?.Invoke(value);
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
            Toggle = !isOn;
        }

        private void ApplyState()
        {
            if (_image == null) return;

            _image.sprite = isOn ? onSprite : offSprite;
            _image.color = isOn ? onColor : offColor;
        }
    }

}