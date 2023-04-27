using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public CharacterController controller;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", controller.velocity.magnitude);
    }
}
