using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParent : MonoBehaviour {
	public enum BodyPart
		{
			Head,
			LeftHand,
			RightHand
		}
	public BodyPart selected;

	public float offsetX = 0f;
	public float offsetY = 0f;
	public float offsetZ = 0f;
	private Vector3 destination;
	private bool parented;

	void Start() {
		parented = false;
		destination.x = offsetX;
		destination.y = offsetY;
		destination.z = offsetZ;
	}

	void LateUpdate () {
		// Dropdown menu handle
		if (!parented) {
			switch(selected) {
				case BodyPart.Head:
				{
					transform.SetParent(GM.CurrentPlayerBody.Head, false);
					break;
				}
				case BodyPart.LeftHand:
				{
					transform.SetParent(GM.CurrentPlayerBody.LeftHand, false);
					break;
				}
				case BodyPart.RightHand:
				{
					transform.SetParent(GM.CurrentPlayerBody.RightHand, false);
					break;
				}
			}
			transform.localPosition = destination;
			parented = true;
		}
	}
}