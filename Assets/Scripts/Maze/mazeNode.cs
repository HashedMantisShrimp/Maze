using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class mazeNode : MonoBehaviour
{
    [SerializeField]
    private GameObject topWall;

    [SerializeField]
    private GameObject bottomWall;

    [SerializeField]
    private GameObject leftWall;

    [SerializeField]
    private GameObject rightWall;

    [SerializeField]
    private GameObject unvisitedNode;

    public bool isVisited { get; private set;}

    public void Visit(){
        isVisited = true;
        unvisitedNode.SetActive(false);
    }

    public void ClearTopWall(){
        topWall.SetActive(false);
    }

    public void ClearBottomWall(){
        bottomWall.SetActive(false);
    }

    public void ClearLeftWall(){
        leftWall.SetActive(false);
    }

    public void ClearRightWall(){
        rightWall.SetActive(false);
    }

}
