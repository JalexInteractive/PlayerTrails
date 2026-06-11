using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace JalexInteractive
{
	public class NgaScrollPointable : FVRPointable
	{
        public float scrollSpeed = 5000f;
		private void Awake()
        {
            this._scrollRect = base.GetComponent<ScrollRect>();
        }

		public override void Update()
		{
			if (m_isBeingPointedAt)
			{
				Vector2 vector = Vector2.zero;

				// Get touchpad input from any pointing hand
				foreach (FVRViveHand fvrviveHand in PointingHands)
				{
					if (!Mathf.Approximately(fvrviveHand.Input.TouchpadAxes.sqrMagnitude, 0f))
					{
						vector = fvrviveHand.Input.TouchpadAxes;
						break;
					}
				}

				if (_scrollRect && vector != Vector2.zero)
				{
					// Vertical scrolling
					if (_scrollRect.vertical && Mathf.Abs(vector.y) > 0.01f)
					{
						float contentHeight = _scrollRect.content.sizeDelta.y;
						float deltaY = scrollSpeed * vector.y * Time.deltaTime;
						float newY = _scrollRect.verticalNormalizedPosition + deltaY / contentHeight;
						_scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newY);
					}

					// Horizontal scrolling
					if (_scrollRect.horizontal && Mathf.Abs(vector.x) > 0.01f)
					{
						float contentWidth = _scrollRect.content.sizeDelta.x;
						float deltaX = scrollSpeed * vector.x * Time.deltaTime;
						float newX = _scrollRect.horizontalNormalizedPosition + deltaX / contentWidth;
						_scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newX);
					}
				}
			}
		}
		private ScrollRect _scrollRect;
	}
}
