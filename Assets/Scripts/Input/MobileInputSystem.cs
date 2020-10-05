using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class MobileInputSystem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
        public event Action OnSwipeUpOccured;
        public event Action OnSwipeDownOccured;
        public event Action OnSwipeLeftOccured;
        public event Action OnSwipeRightOccured;
        
        [Inject]
        private Settings _settings;

        private int pointerId = int.MinValue;
        private Vector2 _startPos;
        
        
        private void Update()
        {
            
            var up = Input.GetKeyDown(KeyCode.UpArrow);
            var down = Input.GetKeyDown(KeyCode.DownArrow);
            var left = Input.GetKeyDown(KeyCode.LeftArrow);
            var right = Input.GetKeyDown(KeyCode.RightArrow);
            
            if (up)
                OnSwipeUpOccured?.Invoke();
            else if (down)
                OnSwipeDownOccured?.Invoke();
            else if (left)
                OnSwipeLeftOccured?.Invoke();
            else if (right)
                OnSwipeRightOccured?.Invoke();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (pointerId == eventData.pointerId)
            {
                Vector2 _touchVector = eventData.position - _startPos;
                if (_touchVector.magnitude > _settings.thresholdMagnitude)
                {
                    pointerId = int.MinValue;

                    if (_touchVector.x > _touchVector.y)
                    {
                        if (Vector3.Dot(_touchVector, Vector3.right) > 0)
                            OnSwipeRightOccured?.Invoke();
                        else
                            OnSwipeLeftOccured?.Invoke();
                    }
                    else
                    {
                        if (Vector3.Dot(_touchVector, Vector3.up) > 0)
                            OnSwipeUpOccured?.Invoke();
                        else
                            OnSwipeDownOccured?.Invoke();
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (pointerId < 0)
            {
                pointerId = eventData.pointerId;
                _startPos = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (pointerId == eventData.pointerId)
            {
                pointerId = int.MinValue;
            }
        }
        
        [Serializable]
        public class Settings
        {
            public float thresholdMagnitude;
        }
}
