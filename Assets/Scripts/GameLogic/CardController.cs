using System;
using DG.Tweening;
using GameData;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace GameLogic
{
    public class CardController : MonoBehaviour, IFactoryItem, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private CardView cardView;
        [SerializeField]
        private Renderer[] renderers;
        [SerializeField]
        private CardImageMask cardImageMask;

        public bool IsDead => _model.Health < 1;

        private CardModel _model;
        private Sequence _moveTween;
        private Vector3 _myPosition;
        private Quaternion _myRotation;
        private bool _isDragging;
        
        public bool IsInDropPanel { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
#endif

        public void SetupInHand(int index, CardModel model)
        {
            _model = model;
            cardImageMask.Setup(index);
            cardView.UpdateView(model);
            cardView.DisableShine();
        }
        
        public void SetupInDropPanel(CardModel model)
        {
            _model = model;
            cardImageMask.Setup(0);
            cardView.UpdateView(model);
            cardView.DisableShine();
            IsInDropPanel = true;
        }

        public void UpdateStatValue(CardStatName statName, int delta)
        {
            var oldStat = _model[statName];
            _model.UpdateStatValue(statName, delta);

            if (!IsDead)
                cardView.AnimateValueUpdate(statName, oldStat, _model[statName]);
        }
        
        public void SetLocalPosAndRot(Vector3 position, Quaternion rotation, bool instant)
        {
            _myPosition = position;
            _myRotation = rotation;
            
            if (instant)
            {
                transform.localPosition = position;
                transform.localRotation = rotation;
                return;
            }

            _moveTween?.Kill(true);
            _moveTween = DOTween.Sequence()
                .Append(transform.DOLocalMove(position, .3f))
                .Join(transform.DOLocalRotateQuaternion(rotation, .3f));
        }
        
        public void Activate()
        {
            gameObject.SetActive(true);
        }
        
        public void Deactivate()
        {
            _model?.OnDestroy();
            _model = null;
            _moveTween?.Kill();
            _moveTween = null;
            _isDragging = false;
            gameObject.SetActive(false);
        }

        public void StartDrag()
        {
            foreach (var r in renderers)
                r.sortingLayerName = "Drag";

            cardImageMask.sortingLayerName = "Drag";

            var pos = transform.localPosition;
            pos.z = -100;
            transform.localPosition = pos;
            transform.localRotation = quaternion.identity;

            _isDragging = true;
            cardView.EnableShine(Color.cyan);
        }

        public void StopDrag()
        {
            cardView.DisableShine();
            _isDragging = false;
            
            _moveTween?.Kill(true);
            _moveTween = DOTween.Sequence()
                .Append(transform.DOLocalMove(_myPosition, .3f))
                .Join(transform.DOLocalRotateQuaternion(_myRotation, .3f));

            _moveTween.onComplete += () =>
            {
                foreach (var r in renderers)
                    r.sortingLayerName = "Default";

                cardImageMask.sortingLayerName = "Default";
            };
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInDropPanel || _isDragging)
                return;
            
            cardView.EnableShine(Color.yellow);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsInDropPanel || _isDragging)
                return;
            
            cardView.DisableShine();
        }

        public CardModel EjectModel()
        {
            var m = _model;
            _model = null;
            return m;
        }
    }
}