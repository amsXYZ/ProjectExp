﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonScript : MonoBehaviour {

	[SerializeField]
	Sprite[] buttonPics;

	[SerializeField]
	Sprite[] starPics;

	GameObject[] buttons;
	GameObject[] stars;

	// Use this for initialization
	void Start () {

		buttons = GameObject.FindGameObjectsWithTag ("Button");
		stars = GameObject.FindGameObjectsWithTag ("Star");
	}

	// Update is called once per frame
	void Update () {
		
		foreach(GameObject button in buttons){
			if (button.GetComponent<Button>().interactable) {
				button.GetComponentInChildren<Image> ().sprite = buttonPics [0];
			} else {
				button.GetComponentInChildren<Image>().sprite = buttonPics [1];
			}
		}

		foreach(GameObject star in stars){
			if (star.GetComponentInParent<Button>().interactable) {
				star.GetComponentInChildren<Image> ().sprite = starPics [0];
			} else {
				star.GetComponentInChildren<Image> ().sprite = starPics [1];
			}
		}
	}
}