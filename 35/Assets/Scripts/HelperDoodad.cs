using UnityEngine;

namespace DefaultNamespace {
    public class HelperDoodad : MonoBehaviour {
        public void SetSprite(Sprite sprite) {
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}