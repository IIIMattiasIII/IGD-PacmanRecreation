using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Vector3> validDirs;
    LevelManager levelManager;

    void Start() {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
        validDirs = levelManager.GetValidDirections(transform.position);
    }
}
