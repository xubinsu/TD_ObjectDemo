using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Player : MonoBehaviour
{
    public PlayerData date;
    public DemoManager demoManager;
    public CharacterController characterController;
    public Animator anim;
    public PlayerInput input;
    public IPlayerState current;
    public IPlayerState last;
    public int particleType = 1;
    public GameObject hitBox;
    public int combo = 0;
    public enum ParticleType
    {
        red,
        green,
        blue,
    }
    public Dictionary<PlayerState.PlayerStateType, IPlayerState> state = new Dictionary<PlayerState.PlayerStateType, IPlayerState>();

    public Vector3 interia = Vector3.zero;

    private void Awake()
    {
        input = new PlayerInput();
        input.Enable();
        demoManager = GameObject.FindWithTag("DemoManager").GetComponent<DemoManager>();
        anim = this.GetComponent<Animator>();
        characterController = this.GetComponent<CharacterController>(); 

        //Ìí¼Ó×´Ì¬
        state.Add(PlayerState.PlayerStateType.Idle, new PlayerState.Idle(this));
        state.Add(PlayerState.PlayerStateType.Walk, new PlayerState.Walk(this));
        state.Add(PlayerState.PlayerStateType.Jump, new PlayerState.Jump(this));
        state.Add(PlayerState.PlayerStateType.Attack, new PlayerState.Attack(this));
        TransState(PlayerState.PlayerStateType.Idle);
    }

    private void Update()
    {
        current.OnUpdate();
        if (input.PlayerBasic.Red.WasPerformedThisFrame())
        {
            particleType = 1;
        }
        else if (input.PlayerBasic.Green.WasPerformedThisFrame())
        {
            particleType = 2;
        }
        else if (input.PlayerBasic.Blue.WasPerformedThisFrame())
        {
            particleType = 3;
        }
    }

    public void TransState(PlayerState.PlayerStateType type)
    {
        if(current != null)
        {
            current.OnExit();
            last = current;
        }
        current = state[type];
        current.OnEnter();
    }

    public Vector3 GetRelativeDirection(Vector3 input)
    {
        Quaternion rot = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        Vector3 dir = rot * Vector3.forward * input.y + rot * Vector3.right * input.x;
        return new Vector3(dir.x, 0, dir.z).normalized;
    }
}
