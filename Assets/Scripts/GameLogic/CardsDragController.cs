using UnityEngine;

namespace GameLogic
{
    public class CardsDragController : MonoBehaviour
    {
        [SerializeField]
        private Camera camera;
        [SerializeField]
        private PlayerHand playerHand;
        
        private RaycastHit2D[] _hits = new RaycastHit2D[5];
        private bool _isDragging;
        private CardController _card;

        private Vector3 CursorPosition => camera.ScreenToWorldPoint(Input.mousePosition);
        
        private void Update()
        {
            if (_isDragging)
            {
                if (Input.GetMouseButton(0))
                    DragCard();
                else if (Input.GetMouseButtonUp(0))
                    DropCard();
                else
                    StopDrag();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                GrabCard();
            }
        }

        private void DragCard()
        {
            var t = _card.transform;
            var from = t.position;
            var to = CursorPosition;
            to.z = from.z;
            t.position = Vector3.Slerp(from, to, Time.deltaTime * 7);
        }

        private void DropCard()
        {
            var dropPanel = GetFromRaycast<CardDropPanel>();

            if (dropPanel != null)
            {
                dropPanel.AddCard(_card.EjectModel(), _card.transform.position);
                playerHand.RemoveCard(_card);
                _card = null;
                _isDragging = false;
            }
            else
            {
                StopDrag();
            }
        }

        private void StopDrag()
        {
            _card.StopDrag();
            _card = null;
            _isDragging = false;
        }

        private void GrabCard()
        {
            _card = GetFromRaycast<CardController>();
            
            if (_card == null || _card.IsInDropPanel)
                return;

            _card.StartDrag();
            _isDragging = true;
        }

        private T GetFromRaycast<T>() where T : class
        {
            T result = null;
            
            var hitsCount = Physics2D.RaycastNonAlloc(CursorPosition, Vector2.zero, _hits, camera.farClipPlane);

            if (hitsCount > 0)
            {
                for (int i = 0; i < hitsCount; i++)
                {
                    if (result == null)
                    {
                        var t = _hits[i].collider?.GetComponent<T>();

                        if (t != null)
                            result = t;
                    }
                    
                    _hits[i] = default;
                }
            }

            return result;
        }
    }
}