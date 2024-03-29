﻿using UnityEngine;
using System.Collections;

public class SimpleAgent : MonoBehaviour {

    NavMeshAgent navAgent;
    Camera _camera;

	// Use this for initialization
	void Start () {
        navAgent = GetComponent<NavMeshAgent>();
        _camera = FindObjectOfType<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            NavMeshHit navHit;
            NavMesh.SamplePosition(objectHit.position, out navHit, 50, NavMesh.AllAreas);
            navAgent.SetDestination(navHit.position);
        }
	}
}
