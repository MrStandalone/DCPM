using System;
using UnityEngine;

//Try attaching this script to each of the jumpers and not the player
//Might be what's causing the double call OnTriggerEnter()
//Can confirm this is what was causing it, not sure why

class JumperAttachment : MonoBehaviour
{
	public static EventHandler<PlayerUsedJumpPadEventArgs> PlayerUsedJumpPad;

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.GetComponent<Jumper>() != null || collider.gameObject.GetComponent<Android>() != null)
		{
			if (PlayerUsedJumpPad != null)
			{
				PlayerUsedJumpPadEventArgs args = new PlayerUsedJumpPadEventArgs(Time.time);
				PlayerUsedJumpPad(this, args);
			}

		}
	}

	public class PlayerUsedJumpPadEventArgs : EventArgs
	{
		public float timeUsed;

		public PlayerUsedJumpPadEventArgs(float timeUsed)
		{
			this.timeUsed = timeUsed;
		}
	}
}

