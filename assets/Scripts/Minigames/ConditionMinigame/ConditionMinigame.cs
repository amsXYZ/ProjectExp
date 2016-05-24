﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ConditionMinigame : AbstractMinigame
{
	[SerializeField]
	[Tooltip("The prefab that is shown to the player")]
	private GameObject _conditionPrefab;

	[SerializeField]
	[Tooltip("The amount of time action feedback is shown")]
	private float _showFeedbackTime;

	//TODO Maybe find a way to do this more elegant
	private Image _rightFeedback;
	private Image _wrongFeedback;

	private GameObject _currentConditionPrefab;
	private List<ConditionObject> _currentConditionObjects;
	private float _feedbackEndTime;

	protected override void Start()
	{
		base.Start();

		_feedbackEndTime = Mathf.Infinity;

		_rightFeedback = GameObject.Find("RightFeedback").GetComponent<Image>();
		_wrongFeedback = GameObject.Find("WrongFeedback").GetComponent<Image>();

		_rightFeedback.gameObject.SetActive(false);
		_wrongFeedback.gameObject.SetActive(false);

		newElement();
	}

	protected override void Update()
	{
		if (_active)
		{
			if (animationsFinished())
			{
				if (Input.GetMouseButtonDown(0))
				{
					foreach (ConditionObject co in _currentConditionObjects)
					{
						co.SwitchState();
					}

					evaluate();
					newElement();
				}
				else if (Input.GetMouseButtonDown(1))
				{
					evaluate();
					newElement();
				}
			}

			if (_feedbackEndTime <= Time.time)
			{
				_rightFeedback.gameObject.SetActive(false);
				_wrongFeedback.gameObject.SetActive(false);
				_feedbackEndTime = Mathf.Infinity;
			}
		}

		base.Update();
	}

	private void evaluate()
	{
		bool result = _currentConditionObjects.ToArray().All(x => x.State == true);

		if (!result)
		{
			result = _currentConditionObjects.ToArray().All(x => x.State == false);
		}

		Debug.Log("Result = " + result);

		//TODO Show animation indication if right or wrong
		if (result)
		{
			_rightFeedback.gameObject.SetActive(true);
			_feedbackEndTime = Time.time + _showFeedbackTime;
			UseCombo();
		}
		else
		{
			_wrongFeedback.gameObject.SetActive(true);
			_feedbackEndTime = Time.time + _showFeedbackTime;
			EndCombo();
		}
	}

	private bool animationsFinished()
	{
		bool result = FindObjectsOfType<ConditionParent>().All(x => x.AnimationPlaying == false);
		result = result && FindObjectsOfType<ConditionObject>().All(x => x.AnimationPlaying == false);
		return result;
	}

	public void newElement()
	{
		//TODO Play a fancy animation of some sort
		Debug.Log("Destroy current prefab");
		GameObject.Destroy(_currentConditionPrefab);
		Debug.Log("Instantiate new prefab");
		_currentConditionPrefab = (GameObject)GameObject.Instantiate(_conditionPrefab, Vector3.zero, Quaternion.identity);
		Debug.Log("Get list with objects");
		_currentConditionObjects = new List<ConditionObject>(_currentConditionPrefab.GetComponentsInChildren<ConditionObject>());
	}
}
