using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManager : MonoBehaviour
{
    public GameObject board; //������ ������Ʈ
    public Transform destination; //��ǥ ����

    private void Start()
    {
        LeanTween.move(board, destination.position, 2f); //2�ʿ� ���� �̵�
    }
}
