using System;
using System.Collections;
using System.Collections.Generic;
using JimJam.Gameplay;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.25f;
    [SerializeField] private KeyCode up, down, left, right;
    public float x, y;
    
    public Vector3 _destination;
    private SmoothMoves _mover;
    public bool _canMove;

    private void Awake()
    {
        _mover = GetComponent<SmoothMoves>();
        _canMove = true;
        _destination = transform.position;
    }

    private void Update()
    {
        x = Input.GetKey(left) ? -1 : Input.GetKey(right) ? 1 : 0;
        y = Input.GetKey(down) ? -1 : Input.GetKey(up) ? 1 : 0;
    }

    private void FixedUpdate()
    {
        if(_canMove && (x !=0 || y!=0) )
            Move();
    }

    private void Move()
    {
        _canMove = false;
        _destination.x += x;
        _destination.z += y;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        ResetMove();
    }

    private void ResetMove()
    {
        StartCoroutine(MoveCooldown());
    }

    IEnumerator MoveCooldown()
    {
        yield return new WaitForSeconds(moveSpeed);
        _canMove = true;
    }
}
