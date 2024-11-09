using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{

    private NavMeshAgent _monster;
    private GameObject _target;
    private Vector3 _lastknownposition;
    private bool _lockOn;

    private int iters = 0;
    // Start is called before the first frame update
    void Start()
    {
        _monster = GetComponent<NavMeshAgent>();
        _target = GameObject.FindWithTag("Player");
        _lastknownposition = transform.position;
        _lockOn = false;
    }


    // Update is called once per frame
    void Update()
    {
        if(iters%10==0)
        {
            Ray ray = new Ray(transform.position, _target.transform.position - transform.position);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.CompareTag("Player"))
                {
                    _lockOn=true;
                    _lastknownposition = _target.transform.position;
                    _monster.destination=_lastknownposition;
                }
            }
        }

        if(!_lockOn && Vector3.Distance(transform.position, _lastknownposition)<0.5f)
        {
            _monster.destination = transform.position;
        }
        iters++;
    }
    
}
