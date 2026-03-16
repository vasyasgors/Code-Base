using UnityEngine;
using UnityEngine.UI;


namespace CodeBase.UI
{

    [RequireComponent(typeof(Button), typeof(Image))]
    public class SpriteToggleButton : MonoBehaviour
    {
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Color[] colors;

        private Image _image;
        private int _index;

        private void Awake()
        {
            _image = GetComponent<Image>();

            if (sprites != null && sprites.Length > 0)
            {
                _index = 0;
                ApplyState();
            }

            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (sprites == null || sprites.Length == 0) return;

            _index = (_index + 1) % sprites.Length;
            ApplyState();
        }

        private void ApplyState()
        {
            if (_index >= 0 && _index < sprites.Length && sprites[_index] != null)
                _image.sprite = sprites[_index];

            if (colors != null && colors.Length > 0)
            {
                int colorIndex = Mathf.Clamp(_index, 0, colors.Length - 1);
                _image.color = colors[colorIndex];
            }
        }
    }

}