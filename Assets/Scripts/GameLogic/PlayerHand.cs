using GameData;
using UnityEngine;
using Utils;

namespace GameLogic
{
    public class PlayerHand : MonoBehaviour
    {        
        [SerializeField]
        private AnimationCurve fanCurve;
        [SerializeField, Range(8, 15)]
        private float MaxSpread = 8;
        [SerializeField, Range(0, 30)]
        private float maxRotationAngle;
        [SerializeField, Range(0, 4)]
        private float maxStep = 1.5f;
        [SerializeField]
        private Transform leftBound;
        [SerializeField]
        private Transform rightBound;
        [SerializeField]
        private CardController cardControllerPrefab;

        private bool _isPrepared;
        private Quaternion _minRotation;
        private Quaternion _maxRotation;
        private float _boundsLength;
        private Pool<CardController> _cardsPool;
        private System.Random _rnd = new System.Random();
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var halfSpread = MaxSpread / 2;
            leftBound.localPosition = new Vector3(-halfSpread, 0);
            rightBound.localPosition = new Vector3(halfSpread, 0);
        }
#endif
        
        public void Setup(CardModel[] cardDatas)
        {
            Prepare();
            
            _cardsPool.DeactivateAll();

            for (var i = 0; i < cardDatas.Length; i++)
                _cardsPool.Get().SetupInHand(i, cardDatas[i]);

            AdjustCardsPosition(false);
        }
        
        private void Prepare()
        {
            if (_isPrepared)
                return;

            _minRotation = Quaternion.Euler(0, 0, maxRotationAngle);
            _maxRotation = Quaternion.Euler(0, 0, -maxRotationAngle);
            _boundsLength = rightBound.localPosition.x - leftBound.localPosition.x;
            
            cardControllerPrefab.Deactivate();
            _cardsPool = new Pool<CardController>(InstantiateCard);
            
            _isPrepared = true;
        }

        private CardController InstantiateCard()
        {
            return Instantiate(cardControllerPrefab, transform);
        }

        private void AdjustCardsPosition(bool instant)
        {
            if (_cardsPool.Count == 0)
                return;

            if (_cardsPool.Count == 1)
            {
                _cardsPool[0].SetLocalPosAndRot(new Vector3(0, fanCurve.Evaluate(.5f), 0), Quaternion.identity, instant);
                return;
            }

            var step = _boundsLength / (_cardsPool.Count - 1);
            step = Mathf.Clamp(step, 0, maxStep);
            var cardsSpread = step * (_cardsPool.Count - 1) / 2;
            var fromPos = leftBound.localPosition;

            if (fromPos.x < -cardsSpread)
                fromPos.x = -cardsSpread;
            
            for (int i = 0; i < _cardsPool.Count; i++)
            {
                var curveX = (float) i / (_cardsPool.Count - 1);
                
                var pos = fromPos + new Vector3(
                    step * i,
                    fanCurve.Evaluate(curveX),
                    -i * 5);

                var rot = Quaternion.Lerp(_minRotation, _maxRotation, curveX);

                _cardsPool[i].SetLocalPosAndRot(pos, rot, instant);
            }
        }

        public void ChangeCardsValues()
        {
            var minDelta = -2;
            var maxDelta = 9;
            
            var i = 0;
            var shouldAdjustPositions = false;
            
            while (i < _cardsPool.Count)
            {
                var card = _cardsPool[i];
                var stat = Extensions.GetRandom<CardStatName>();
                var value = _rnd.Next(minDelta, maxDelta + 1);

                card.UpdateStatValue(stat, value);

                if (card.IsDead)
                {
                    _cardsPool.Deactivate(card);
                    shouldAdjustPositions = true;
                }
                else
                {
                    i++;
                }
            }
            
            if (shouldAdjustPositions)
                AdjustCardsPosition(false);
        }

        public void RemoveCard(CardController card)
        {
            _cardsPool.Deactivate(card);
            AdjustCardsPosition(false);
        }
    }
}