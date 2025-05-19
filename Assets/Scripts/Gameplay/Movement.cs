using System;
using BitBox.Gameplay;
using BitBox.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    public Transform target;

    public float speed;

    public Animator animator;

    public float dampTime;

    public bool IsLockedOnTarget;
    public float RotationSpeed;

    private Rigidbody _rigidbody;
    private Transform _transform;
    private CameraController _cameraController;

    private void OnEnable()
    {
        TargetLockController.OnTargetLockStateChanged += UpdateTargetLockState;
    }

    private void OnDisable()
    {
        TargetLockController.OnTargetLockStateChanged -= UpdateTargetLockState;
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
        _cameraController = CameraController.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        bool noInput = inputDirection == Vector2.zero;
        
        if (noInput)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        
        if (IsLockedOnTarget) 
            FocusedMovement(ref inputDirection);
        
        else if (!noInput) DefaultMovement(ref inputDirection);
    }

    private void DefaultMovement(ref Vector2 inputDirection)
    {
        var moveDirection = (inputDirection.x * _cameraController.Right + inputDirection.y * _cameraController.Forward).normalized;
        _rigidbody.linearVelocity = moveDirection * speed;
        _transform.rotation = Quaternion.Slerp(_transform.rotation, Quaternion.LookRotation(moveDirection.normalized), RotationSpeed * Time.fixedDeltaTime);
    }

    private void FocusedMovement(ref Vector2 inputDirection)
    {
        var movementForward = _cameraController.Right * inputDirection.x + _cameraController.Forward * inputDirection.y;
        movementForward.Normalize();
        var movementRight = Vector3.Cross(movementForward, Vector3.up);

        transform.position += movementForward.normalized * (speed * Time.deltaTime);
        
        var targetDirection = target.position - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        var fw = Vector3.Dot(targetDirection, movementForward);
        var sd = Vector3.Dot(targetDirection, movementRight);

        animator.SetFloat("fw", fw, dampTime, Time.deltaTime);
        animator.SetFloat("sd", sd, dampTime, Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), RotationSpeed * Time.fixedDeltaTime);
            
    }
    
    private void UpdateTargetLockState(bool isLocked, Transform target)
    {
        IsLockedOnTarget = isLocked;
        this.target = target;
    }

}