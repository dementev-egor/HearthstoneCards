using DG.Tweening;
using GameData;
using UnityEngine;
using Utils;

namespace GameLogic
{
    public class CardDropPanel : MonoBehaviour
    {
        [SerializeField]
        private CardController cardControllerPrefab;
        [SerializeField]
        private Transform cardsParent;

        private bool _isPrepared;
        private Pool<CardController> _cardsPool;

        public void AddCard(CardModel cardModel, Vector3 position)
        {
            Prepare();
            
            var card = _cardsPool.Get();
            card.SetupInDropPanel(cardModel);
            card.transform.position = position;
            card.transform.DOLocalMove(Vector3.right * 2.5f * (_cardsPool.Count - 1), .5f);
        }
        
        private void Prepare()
        {
            if (_isPrepared)
                return;
            
            cardControllerPrefab.Deactivate();
            _cardsPool = new Pool<CardController>(InstantiateCard);
            
            _isPrepared = true;
        }

        private CardController InstantiateCard()
        {
            return Instantiate(cardControllerPrefab, cardsParent);
        }
    }
}