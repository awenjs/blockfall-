using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    [SerializeField] Camera _cam;
    [SerializeField] Transform _playerBody;
    [SerializeField] float _speed_Move;
    [SerializeField] float _wallJumpForce;


    [SerializeField] Transform _foot;
    [SerializeField] LayerMask _layer_Ground;
    [SerializeField] float _jumpHeight;
    [SerializeField] int _numberOfJump;
    int _currentNumOfJump;
    
    
    Rigidbody _rb;
    Vector2 _inputRaw;
    Vector3 _moveDirection;

    
    [SerializeField] float _sensitivity;
    [SerializeField] float _wallRunLeft;
    [SerializeField] float _wallRunRight;
    

    float _rotationX;
    float _rotationY;
    float _rotationZ;
    float _mouseX;
    float _mouseY;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _rb = GetComponent<Rigidbody>();
        _currentNumOfJump = _numberOfJump;
    }

    void Update()
    {
        IsMove();
        Camera();
        CanWallRun();
        MoveInput();
        Move();
        Jump();
        
    }

    
    
    void Move()
    {
        Vector2 normalizedInput = _inputRaw.normalized;

        _moveDirection = _playerBody.forward * normalizedInput.y + _playerBody.right * normalizedInput.x;
        _moveDirection *= _speed_Move;

        _rb.velocity = _moveDirection + Vector3.up * _rb.velocity.y;
        
        if (WallRunInput())
        {
            _rb.useGravity = false;
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);


            if (CanWallRunLeft())
            {
                _rotationZ = Mathf.Lerp(_rotationZ, _wallRunLeft, 25f * Time.deltaTime);
                if (JumpInput())
                {
                    _rb.AddForce(_playerBody.right * _wallJumpForce,ForceMode.Impulse);
                }
            }

            if (CanWallRunRight())
            {
                _rotationZ = Mathf.Lerp(_rotationZ, _wallRunRight, 25f * Time.deltaTime);
                if (JumpInput())
                {
                    _rb.AddForce(-_playerBody.right * _wallJumpForce,ForceMode.Impulse);
                }
            }

        }
        
        if (!WallRunInput())
        {
            _rb.useGravity = true;
            
            _rotationZ = Mathf.Lerp(_rotationZ, 0, 25f * Time.deltaTime);
        }
    }
    
    
    
    void Jump()
    {
        if (JumpInput() && _currentNumOfJump != 0)
        {
            _currentNumOfJump -= 1;
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpHeight, _rb.velocity.z);
        }

        if (WallRunInput() || IsGround() && _rb.velocity.y < 0)
        {
            _currentNumOfJump = _numberOfJump;
        }
    }
    
    
    void Camera()
    {
        _mouseX = Input.GetAxis("Mouse X") * _sensitivity * Time.deltaTime;
        _mouseY = Input.GetAxis("Mouse Y") * _sensitivity * Time.deltaTime;
        _rotationX = Mathf.Clamp( _rotationX - _mouseY, -90f, 90f);
        _rotationY += _mouseX;
        _cam.transform.localRotation = Quaternion.Euler(_rotationX, 0f, _rotationZ);
        _playerBody.rotation = Quaternion.Euler(0f, _rotationY, 0f);
    }
    
    
    
    
    
    void MoveInput()
    {
        _inputRaw = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }


    bool JumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    bool IsMove()
    {
        Vector3 currentVelocity = _rb.velocity;
        currentVelocity.y = 0;
        return currentVelocity.magnitude > 0;
    }
    
    bool CanWallRun()
    {
        return CanWallRunLeft() || CanWallRunRight();
    }

    bool IsGround()
    {
        return Physics.CheckSphere(_foot.position, 0.4f, _layer_Ground);
    }

    bool CanWallRunLeft()
    {
        return Physics.Raycast(_playerBody.position, -_playerBody.right, 1f);
    }

    bool CanWallRunRight()
    {
        return Physics.Raycast(_playerBody.position, _playerBody.right, 1f);
    }

    bool WallRunInput()
    {
        return CanWallRun() && IsMove() && !IsGround();
    }
    
}
