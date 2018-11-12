using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    private CharacterAnimationManager _anim;
    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<CharacterAnimationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { _anim.Run(); }
    }
}
