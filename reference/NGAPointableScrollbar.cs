using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace JalexInteractive
{
	public class NGAPointableScrollbar : FVRPointable
	{
		public Scrollbar Scrollbar;

		public RectTransform TrackRect;

		private Collider _col;

		private bool _grabbing;

		private FVRViveHand _grabHand;

		private Plane _dragPlane;

		private float _grabOffset01;

		private bool _vertical;

		private bool _reversed;

		private float _usableMinLocal;

		private float _usableMaxLocal;
		private void Awake()
		{
			this._col = base.GetComponent<Collider>();
			bool flag = this.Scrollbar == null;
			if (flag)
			{
				this.Scrollbar = base.GetComponentInParent<Scrollbar>();
			}
			bool flag2 = this.TrackRect == null && this.Scrollbar != null;
			if (flag2)
			{
				this.TrackRect = this.Scrollbar.GetComponent<RectTransform>();
			}
		}
		public override void Update()
		{
			base.Update();
			bool flag = !this._grabbing || this._grabHand == null || this.Scrollbar == null || this.TrackRect == null;
			if (!flag)
			{
				Vector3 oneEuroPointingPos = this._grabHand.Input.OneEuroPointingPos;
				Vector3 direction = this._grabHand.Input.OneEuroPointRotation * Vector3.forward;
				Ray ray = new Ray(oneEuroPointingPos, direction);
				bool flag2 = this._grabHand.Input.TriggerUp || this._grabHand.Input.GripUp;
				if (flag2)
				{
					this._grabbing = false;
					this._grabHand = null;
				}
				else
				{
					float distance;
					bool flag3 = this._dragPlane.Raycast(ray, out distance);
					if (flag3)
					{
						Vector3 point = ray.GetPoint(distance);
						Vector3 vector = this.TrackRect.InverseTransformPoint(point);
						float value = this._vertical ? vector.y : vector.x;
						float num = Mathf.InverseLerp(this._usableMinLocal, this._usableMaxLocal, value);
						num = Mathf.Clamp01(num);
						float num2 = this._reversed ? (1f - num) : num;
						this.Scrollbar.value = Mathf.Clamp01(num2 - this._grabOffset01);
					}
				}
			}
		}
		public override void OnPoint(FVRViveHand hand)
		{
			base.OnPoint(hand);
			bool flag = this._col == null || this.Scrollbar == null || this.TrackRect == null;
			if (!flag)
			{
				bool grabbing = this._grabbing;
				if (!grabbing)
				{
					Vector3 oneEuroPointingPos = hand.Input.OneEuroPointingPos;
					Vector3 direction = hand.Input.OneEuroPointRotation * Vector3.forward;
					Ray ray = new Ray(oneEuroPointingPos, direction);
					bool flag2 = !hand.Input.TriggerDown && !hand.Input.GripDown;
					if (!flag2)
					{
						float maxDistance = Mathf.Max(this.MaxPointingRange, 0.01f);
						RaycastHit raycastHit;
						bool flag3 = !this._col.Raycast(ray, out raycastHit, maxDistance);
						if (!flag3)
						{
							this.BeginGrab(hand, ray);
						}
					}
				}
			}
		}
		private void BeginGrab(FVRViveHand hand, Ray ray)
		{
			this._grabbing = true;
			this._grabHand = hand;
			this._vertical = (this.Scrollbar.direction == Scrollbar.Direction.BottomToTop || this.Scrollbar.direction == Scrollbar.Direction.TopToBottom);
			this._reversed = (this.Scrollbar.direction == Scrollbar.Direction.RightToLeft || this.Scrollbar.direction == Scrollbar.Direction.TopToBottom);
			this._dragPlane = new Plane(this.TrackRect.forward, this.TrackRect.position);
			float num = this._vertical ? this.TrackRect.rect.yMin : this.TrackRect.rect.xMin;
			float num2 = this._vertical ? this.TrackRect.rect.yMax : this.TrackRect.rect.xMax;
			float num3 = 0f;
			bool flag = this.Scrollbar.handleRect != null;
			if (flag)
			{
				num3 = (this._vertical ? this.Scrollbar.handleRect.rect.height : this.Scrollbar.handleRect.rect.width);
			}
			float num4 = num3 * 0.5f;
			this._usableMinLocal = num + num4;
			this._usableMaxLocal = num2 - num4;
			bool flag2 = Mathf.Abs(this._usableMaxLocal - this._usableMinLocal) < 0.0001f;
			if (flag2)
			{
				this._usableMinLocal = num;
				this._usableMaxLocal = num2;
			}
			float num5 = this.Scrollbar.value;
			float distance;
			bool flag3 = this._dragPlane.Raycast(ray, out distance);
			if (flag3)
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = this.TrackRect.InverseTransformPoint(point);
				float value = this._vertical ? vector.y : vector.x;
				float num6 = Mathf.InverseLerp(this._usableMinLocal, this._usableMaxLocal, value);
				num6 = Mathf.Clamp01(num6);
				num5 = (this._reversed ? (1f - num6) : num6);
			}
			this._grabOffset01 = num5 - this.Scrollbar.value;
		}
		public override void EndPoint(FVRViveHand hand)
		{
			base.EndPoint(hand);
		}

	}
}
