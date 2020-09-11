using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerController : MonoBehaviour {
    public float moveSpeed;
    public float jumpSpeed;

    public BoxCollider2D feet;

    public LayerMask groundLayer;

    public AudioClip jump;
    public AudioClip land;

    private Rigidbody2D _rb;
    private AudioSource _source;

    private float _moveDirection;
    private bool _doJump;

    private bool _wasGroundedBefore = true;

    private void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _source = GetComponent<AudioSource>();

        GetComponent<SpriteRenderer>().color = ColourPickup.GetColor(PickupColor.FrenchRaspberry);
    }

    private void Update() {
        if (_doJump) {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpSpeed);
            _doJump = false;
            _source.PlayOneShot(jump);
        }

        _rb.velocity = new Vector2(_moveDirection * moveSpeed, _rb.velocity.y);

        if (transform.position.y < -60) {
            GameController.instance.Died();
        }

        if (!IsGrounded()) {
            _wasGroundedBefore = false;
        } else if (!_wasGroundedBefore) {
            _wasGroundedBefore = true;
            _source.PlayOneShot(land);
        }
    }

    public void Move(InputAction.CallbackContext ctx) {
        _moveDirection = ctx.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext ctx) {
        if (ctx.performed && !_doJump && IsGrounded())
            _doJump = true;
    }

    public bool IsGrounded() {
        return Physics2D.OverlapBox(new Vector2(feet.transform.position.x, feet.transform.position.y) + feet.offset, feet.size, 0,
                                    groundLayer);
    }

    public void Color1() {
        if (GameController.instance.HasColor(PickupColor.Olivine))
            GameController.instance.currentActive = PickupColor.Olivine;
    }

    public void Color2() {
        if (GameController.instance.HasColor(PickupColor.PalatinatePurple))
            GameController.instance.currentActive = PickupColor.PalatinatePurple;
    }

    public void Color3() {
        if (GameController.instance.HasColor(PickupColor.RifleGreen))
            GameController.instance.currentActive = PickupColor.RifleGreen;
    }

    public void Color4() {
        if (GameController.instance.HasColor(PickupColor.MaximumBluePurple))
            GameController.instance.currentActive = PickupColor.MaximumBluePurple;
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.CompareTag("Hazard")) {
            GameController.instance.Died();
        }
    }
}