﻿using UnityEngine;
using System.Collections;

public abstract  class ConditionParent : MonoBehaviour
{
	private bool _animationPlaying;

	public bool AnimationPlaying
	{
		get { return _animationPlaying; }
	}
}
