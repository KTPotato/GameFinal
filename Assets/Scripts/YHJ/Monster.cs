using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterTurtleShell : MonoBehaviour
{
    private NavMeshAgent _monster;
    private GameObject _target;
    private Vector3 _lastKnownPosition;
    private bool _lockOn;

    private int iters = 0;
    private int _updateInterval = 10; // 10 �����Ӹ��� ������Ʈ

    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _target = GameObject.FindWithTag("Player");
        _lastKnownPosition = transform.position;
        _lockOn = false;
    }

    void Update()
    {
        // ���� �ֱ�� �þ� �˻�
        if (iters % _updateInterval == 0)
        {
            Ray ray = new Ray(transform.position, _target.transform.position - transform.position);
            RaycastHit hit;

            // �÷��̾ �þ߿� �ִ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    // �÷��̾ �þ߿� ������ ���� ����
                    _lockOn = true;
                    _lastKnownPosition = _target.transform.position;
                    _monster.destination = _lastKnownPosition;
                }
                else
                {
                    // �þ߿��� ����� ������ ��ġ�� �̵�
                    _lockOn = false;
                    _monster.destination = _lastKnownPosition;
                }
            }
        }

        // �÷��̾ �þ߿� ���� ���� ����ؼ� ����
        if (_lockOn)
        {
            _lastKnownPosition = _target.transform.position;
            _monster.destination = _lastKnownPosition;
        }

        // ������ ��ġ�� ���� �� ����
        if (!_lockOn && Vector3.Distance(transform.position, _lastKnownPosition) < 0.5f)
        {
            _monster.destination = transform.position;
        }

        iters++;
    }
}
