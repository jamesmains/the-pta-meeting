using System.Collections;
using JimJam.Gameplay;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float moveDelay = 0.25f;
    [SerializeField] private KeyCode up, down, left, right;
    [SerializeField] private LayerMask ground;
    private int x, y;
    [HideInInspector] public int r;
    private int lastR;
    private Vector3 _destination;
    
    private bool _canMove;

    
    public SmoothMoves _mover;
    public SmoothMoves _turner;
    public SmoothMoves _gfx;
    private Rigidbody _rb;
    
    private void Awake()
    {
        //_mover = GetComponent<SmoothMoves>();
        _rb = GetComponent<Rigidbody>();
        _canMove = true;
        _destination = transform.position;
        _mover.SetPoint(0,_destination);
    }

    private void Update()
    {
        x = Input.GetKey(left) ? -1 : Input.GetKey(right) ? 1 : 0;
        y = Input.GetKey(down) ? -1 : Input.GetKey(up) ? 1 : 0;
    }

    private void FixedUpdate()
    {
        r = x == 0 && y == 1 ? 0 : x == 1 && y == 0 ? 90 : x == 0 && y == -1 ? 180 : x == -1 && y == 0 ? 270 : lastR;
        if (lastR != r)
        {
            lastR = r;
            Turn();
        }
        if(_canMove && (x !=0 || y!=0) )
            Move();
    }

    private void Turn()
    {
        _turner.SetPoint(0, new Vector3(0,r,0));
        _turner.TravelToPoint(0);
    }
    
    private void Move()
    {
        _canMove = false;
        _destination.x += x;
        _destination.z += y;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        _gfx.GotoEnd();
        ResetMove();
    }

    public void Leap(int comparedRotation)
    {
        if (comparedRotation == r && !_canMove)
        {
            _canMove = false;
            _destination.x += x*2;
            _destination.z += y*2;
            _destination.y += 1;
            
            _mover.SetPoint(0,_destination);
            _mover.TravelToPoint(0);
            _gfx.GotoEnd();
            StopCoroutine(MoveCooldown());
            StopCoroutine(JumpCooldown());
            StartCoroutine(LeapCooldown());
        }
    }

    private bool CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2, ground))
        {
            return true;
        }
        else return false;
    }

    private void ResetMove()
    {
        StartCoroutine(MoveCooldown());
        StartCoroutine(JumpCooldown());
    }

    IEnumerator MoveCooldown()
    {
        yield return new WaitForSeconds(moveDelay);
        if (CheckGround())
        {
            _canMove = true;
            
        }
        else
        {
            _canMove = false;
            _mover.enabled = false;
            _rb.isKinematic = false;
        }
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(moveDelay/2);
        _gfx.TravelToPoint(0);
    }

    IEnumerator LeapCooldown()
    {
        yield return new WaitForSeconds(moveDelay);
        _destination.y -= 1;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        yield return new WaitForSeconds(moveDelay);
        if (CheckGround())
        {
            _canMove = true;
            
        }
        else
        {
            _canMove = false;
            _mover.enabled = false;
            _rb.isKinematic = false;
        }
    }
}
