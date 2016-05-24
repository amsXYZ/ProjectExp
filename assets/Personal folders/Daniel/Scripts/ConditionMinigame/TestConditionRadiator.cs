﻿using UnityEngine;
using System.Collections;
using System;

public class TestConditionRadiator : ConditionObject
{
	protected override void InitTrue()
	{
		transform.parent.FindChild("Sommerlandschaft").gameObject.SetActive(false);

		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.material.SetColor("_Color", Color.red);
		}
	}

	protected override void InitFalse()
	{
		transform.parent.FindChild("Schneelandschaft").gameObject.SetActive(false);

		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.material.SetColor("_Color", Color.white);
		}
	}
}