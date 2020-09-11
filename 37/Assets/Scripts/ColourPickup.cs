using System;
using UnityEngine;

namespace DefaultNamespace {
    public enum PickupColor {
        None = 0,
        Olivine = 1,
        PalatinatePurple = 2,
        RifleGreen = 4,
        MaximumBluePurple = 8,
        FrenchRaspberry = 16
    }
    
    public class ColourPickup : MonoBehaviour {
        public PickupColor color;

        private float _initialY;
        
        private void Start() {
            _initialY = transform.position.y;
            GetComponent<SpriteRenderer>().color = GetColor(color);
        }

        private void Update() {
            transform.position = new Vector3(transform.position.x, _initialY + Mathf.Sin(Time.time) * 0.25f);
        }

        public static Color GetColor(PickupColor desiredColor) {
            switch (desiredColor) {
                case PickupColor.Olivine:
                    return new Color(182 / 255f, 208 / 255f, 148 / 255f);
                case PickupColor.PalatinatePurple:
                    return new Color(76 / 255f, 30 / 255f, 79 / 255f);
                case PickupColor.RifleGreen:
                    return new Color(63 / 255f, 69 / 255f, 49 / 255f);
                case PickupColor.MaximumBluePurple:
                    return new Color(173 / 255f, 189 / 255f, 255 / 255f);
                case PickupColor.FrenchRaspberry:
                    return new Color(195 / 255f, 49 / 255f, 73 / 255f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(desiredColor), desiredColor, null);
            }
        }

        private void OnTriggerStay2D(Collider2D other) {
            GameController.instance.AddColor(color);
            Destroy(gameObject);
        }
    }
}