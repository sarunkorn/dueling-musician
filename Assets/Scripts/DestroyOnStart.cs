using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    [SerializeField] GameObject _root;
    

    void Start()
    {
        if (_root != null)
        {
            Destroy(_root);
        }
        else
        {
            Destroy(this);
        }
    }
}
