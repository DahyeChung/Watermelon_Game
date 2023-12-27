using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManager : MonoBehaviour
{
    public GameObject board; //움직일 오브젝트
    public Transform destination; //목표 지점

    private void Start()
    {
        LeanTween.move(board, destination.position, 2f); //2초에 걸쳐 이동
    }
}
