using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public LineOfSight los;
    public List<Transform> waypoints;

    FSM<StateEnum> _fsm;
    IAttack _entityAttack;
    ITreeNode _root;

    private void Start()
    {
        InitializedFSM();
        InitializedTree();
    }

    void InitializedFSM()
    {
        IMove entityMove = GetComponent<IMove>();
        _entityAttack = GetComponent<IAttack>();

        var idle = new EnemyIdleState();
        var patrol = new EnemyPatrolState(entityMove, transform, waypoints);
        var chase = new EnemyChaseState(entityMove, transform, target);
        var attack = new EnemyAttackState(_entityAttack);

        // Patrol
        patrol.AddTransition(StateEnum.Chase, chase);
        patrol.AddTransition(StateEnum.Attack, attack);
        patrol.AddTransition(StateEnum.Idle, idle);

        // Idle
        idle.AddTransition(StateEnum.Patrol, patrol);
        idle.AddTransition(StateEnum.Chase, chase);

        // Chase
        chase.AddTransition(StateEnum.Attack, attack);
        chase.AddTransition(StateEnum.Idle, idle);
        chase.AddTransition(StateEnum.Patrol, patrol);

        // Attack
        attack.AddTransition(StateEnum.Chase, chase);
        attack.AddTransition(StateEnum.Idle, idle);

        _fsm = new FSM<StateEnum>(patrol); // arrancamos patrullando
    }

    void InitializedTree()
    {
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));
        var patrol = new ActionTree(() => _fsm.Transition(StateEnum.Patrol));
        var chase = new ActionTree(() => _fsm.Transition(StateEnum.Chase));
        var attack = new ActionTree(() => _fsm.Transition(StateEnum.Attack));

        var qDistance = new QuestionTree(InAttackRange, attack, chase);
        var qInView = new QuestionTree(InView, qDistance, patrol);
        var qIsExist = new QuestionTree(() => target != null, qInView, idle);

        _root = qIsExist;
    }

    bool InView()
    {
        if (target == null || los == null) return false;

        float dist = Vector3.Distance(target.position, transform.position);

        bool r = los.CheckRange(target);
        bool a = los.CheckAngle(target);
        bool v = los.CheckView(target);


        return r && a && v;
    }

    bool InAttackRange()
    {
        return Vector3.Distance(target.position, transform.position) <= _entityAttack.GetAttackRange;
    }

    private void Update()
    {
        _root.Execute();   
        _fsm.OnUpdate();
     
    }

    private void FixedUpdate()
    {
        _fsm.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}