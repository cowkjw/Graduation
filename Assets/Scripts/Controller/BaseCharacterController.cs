using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{

    protected Vector3 DestPos;
    protected GameObject Target;
    protected Stat Stat;
    protected Define.State _state = Define.State.Idle;

   public virtual Define.State State
    {
        get { return _state; }

        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();

            switch (_state)
            {
                case Define.State.Idle:
                    anim.CrossFade("Idle", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("Run", 0.1f);
                    break;
                case Define.State.Attack:
                    anim.CrossFade("Attack", 0.1f);
                    break;
                case Define.State.Die:
                    anim.CrossFade("Die", 0.005f);
                    break;
            }
        }
    }

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Update()
    {
        switch (State)
        {
            case Define.State.Die:
                Dying();
                break;
            case Define.State.Moving:
                Moving();
                break;
            case Define.State.Attack:
                Attacking();
                break;
        }
    }

    protected virtual void Dying() { }
    protected virtual void Moving() { }
    protected virtual void Attacking() { }
    protected virtual void Init() { }
}
