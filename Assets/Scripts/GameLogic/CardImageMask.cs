using UnityEngine;

namespace GameLogic
{
    public class CardImageMask : MonoBehaviour
    {
        [SerializeField]
        private SpriteMask mask;
        [SerializeField]
        private SpriteRenderer renderer;

        public string sortingLayerName
        {
            set
            {
                var id = SortingLayer.NameToID(value);
                mask.frontSortingLayerID = id;
                mask.backSortingLayerID = id;
            }
        }

        public void Setup(int index)
        {
            mask.isCustomRangeActive = true;
            mask.backSortingOrder = index;
            mask.frontSortingOrder = index + 2;

            renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            renderer.sortingOrder = index + 1;
        }
    }
}