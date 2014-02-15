using UnityEngine;
using System.Collections;

public class CharacterAnimator : MonoBehaviour {

//	public AnimationClip idle;
//	public AnimationClip walkright;
//	public AnimationClip walkleft;
//	public AnimationClip walkup;
//	public AnimationClip walkdown;
//	public AnimationClip trading;
//	public AnimationClip disrupting;

	public float walkingX;
	public float walkingY;
	public bool isTrading;
	public bool isDisrupting;


	// Update is called once per frame
	void Update () {
	
		Animator an = GetComponent<Animator>();

		//TODO: Assign variables from the last frames to determine what animation to play

		if (isTrading)
		{
			an.Play("Trading");
		}
		else if (isDisrupting)
		{
			an.Play("Disrupting");
		}
		else
		{
			//Movement or idle

			if (walkingY > 0.5f && Mathf.Abs(walkingX) < 0.5f)
			{
				an.Play("WalkUp");
			}
			else if (walkingY < -0.5f && Mathf.Abs(walkingX) < 0.5f)
			{
				an.Play("WalkDown");
			}
			else if (walkingX > 0.5f)
			{
				an.Play("WalkRight");
			}
			else if (walkingX < -0.5f)
			{
				an.Play("WalkLeft");
			}
			else
			{
				an.Play("Idle");
			}
		}
	}
}
