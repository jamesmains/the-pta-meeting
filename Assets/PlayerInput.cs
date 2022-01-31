using System;
using System.Collections;
using System.Collections.Generic;
using JimJam.Gameplay;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public enum FrogColors
    {
        Purple = 0,
        Yellow = 1
    }

    [Header("Frog Settings")]
    public FrogColors frogColor;
    [SerializeField] private float moveDelay = 0.25f;
    [SerializeField] private float leapTime = 0.25f;
    [SerializeField] private KeyCode up, down, left, right;
    
    [Header("")]
    [SerializeField] private LayerMask col;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask saveGround;
    [SerializeField] private float deathFloor;
    [SerializeField] private Vector3 respawnPos;
    public int x, y;
    [HideInInspector] public int r;
    private int lastR;
    private Vector3 _destination;
    private bool _canMove;
    public List<KeyCode> keyPressOrder;
    public KeyCode[] keys;
    
    public SmoothMoves _mover;
    public SmoothMoves _turner;
    public SmoothMoves _gfx;
    private Rigidbody _rb;
    private AudioSource _sfx;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _sfx = GetComponent<AudioSource>();
        _canMove = true;
        _destination = transform.position;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        keys = new[] {up, down, left, right};
        SetCheckPoint(_destination);
    }

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        int keysPressed = 0;
        foreach (var key in keys)
        {
            if (Input.GetKey(key))
                keysPressed++;
            if(Input.GetKey(key) && !keyPressOrder.Contains(key))
                keyPressOrder.Add(key);
            else if (Input.GetKeyUp(key) && keyPressOrder.Contains(key))
                keyPressOrder.Remove(key);
        }
        if(keysPressed==0) keyPressOrder.Clear();
        if (keyPressOrder.Count > 0)
        {
            var key = keyPressOrder[^1];
            x = key == left ? -1 : key == right ? 1 : 0;
            y = key == down ? -1 : key == up ? 1 : 0;
        }
        else x = y = 0;
    }

    private void FixedUpdate()
    {
        r = x == 0 && y == 1 ? 0 : x == 1 && y == 0 ? 90 : x == 0 && y == -1 ? 180 : x == -1 && y == 0 ? 270 : lastR;
        if (lastR != r && _canMove)
        {
            lastR = r;
            Turn();
        }
        if(_canMove && (x !=0 || y!=0) )
            Move();
        if(transform.position.y<deathFloor)
            Reset();
    }

    private void Reset()
    {
        ToggleMovers(true);
        _destination = transform.position = respawnPos;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        Turn();
    }
    
    private void Turn()
    {
        _turner.SetPoint(0, new Vector3(0,r,0));
        _turner.TravelToPoint(0);
    }
    
    private void Move()
    {
        var dir = Vector3.zero;
        dir.x += x;
        dir.z += y;
        // Shoot ray
        int state = CheckCols(dir);
        if (state == 1) Leap();
        else if(state == 2) return;
        
        if (!_canMove) return;
        _sfx.PlayOneShot(_sfx.clip);
        _canMove = false;
        _destination += dir;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        _gfx.GotoEnd();
        
        ResetMove();
    }

    public void Leap()
    {
        _canMove = false;
        _destination.x += x*4;
        _destination.z += y*4;
        _destination.y += 2;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        
        StopCoroutine(MoveCooldown());
        StopCoroutine(JumpCooldown());
        StartCoroutine(LeapCooldown());
    }

    private int CheckCols(Vector3 dir)
    {
        RaycastHit hit;
        var origin = transform.position;
        origin.y -= .45f;
        if (Physics.Raycast(origin, dir, out hit, 1, col))
        {
            if (hit.transform.CompareTag("Frog") && hit.transform.gameObject != this.gameObject)
            {
                if (hit.transform.GetComponent<PlayerInput>().r == r)
                    return 1;
                else return 2;
            }
            else return 2;
        }
        else return 0;
    }

    private bool CheckGround()
    {
        RaycastHit hit;
        RaycastHit hit2;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2, ground))
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit2, 2, saveGround))
            {
                SetCheckPoint(_destination);
            }
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
        if (CheckGround()) _canMove = true;
        else ToggleMovers(false);
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(moveDelay/2);
        _gfx.TravelToPoint(0);
    }

    IEnumerator LeapCooldown()
    {
        yield return new WaitForSeconds(moveDelay/2);
        _destination.y -= 2;
        _mover.SetPoint(0,_destination);
        _mover.TravelToPoint(0);
        yield return new WaitForSeconds(leapTime/4);
        if (!CheckGround())
        {
            ToggleMovers(false);
            yield break;
        }
        yield return new WaitForSeconds(leapTime/2);
        if (CheckGround()) _canMove = true;
        else ToggleMovers(false);
    }

    private void ToggleMovers(bool state)
    {
        _canMove = state;
        _mover.enabled = state;
        _rb.isKinematic = state;
    }

    public void SetCheckPoint(Vector3 pos)
    {
        respawnPos = pos;
    }
}
