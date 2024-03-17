using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerState
{
    public enum PlayerStateType
    {
        Idle,
        Attack,
        Jump,
        Interact,
        Walk,
    }

    public class Idle : IPlayerState
    {
        public Player self;
        public Idle(Player self)
        {
            this.self = self;
        }

        public void OnEnter()
        {
            self.anim.Play("Idle");
            self.interia = Vector3.zero;
        }

        public void OnExit()
        {
            
        }

        public void OnUpdate()
        {
            if (self.input.PlayerBasic.Jump.WasPerformedThisFrame())
            {
                self.TransState(PlayerStateType.Jump);
            }
            else if (self.input.PlayerBasic.Attack.WasPerformedThisFrame())
            {
                self.TransState(PlayerStateType.Attack);
            }
            else if(self.input.PlayerBasic.Move.ReadValue<Vector2>().magnitude >= 0.05f)
            {
                self.TransState(PlayerStateType.Walk);
            }
        }
    }

    public class Walk : IPlayerState
    {
        public Player self;
        public Walk(Player self)
        {
            this.self = self;
        }

        public void OnEnter()
        {
            self.anim.Play("Walk");
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            if (self.input.PlayerBasic.Jump.WasPerformedThisFrame())
            {
                self.TransState(PlayerStateType.Jump);
            }
            else if (self.input.PlayerBasic.Attack.WasPerformedThisFrame())
            {
                self.TransState(PlayerStateType.Attack);
                // 在攻击状态下继续移动
            }
            else if (self.input.PlayerBasic.Move.ReadValue<Vector2>().magnitude <= 0.05f)
            {
                self.TransState(PlayerStateType.Idle);
            }
            else
            {
                Move();
            }
        }

        private void Move()
        {
            self.characterController.Move(Time.deltaTime * self.date.speed * self.GetRelativeDirection(self.input.PlayerBasic.Move.ReadValue<Vector2>()));
            self.transform.LookAt(self.transform.position + self.GetRelativeDirection(self.input.PlayerBasic.Move.ReadValue<Vector2>()));
            self.interia = self.GetRelativeDirection(self.input.PlayerBasic.Move.ReadValue<Vector2>());
        }
    }

    public class Jump : IPlayerState
    {
        public Player self;
        public float time;

        public float speed;
        public Jump(Player self)
        {
            this.self = self;
        }

        public void OnEnter()
        {
            time = 0;
            speed = 20;
            self.anim.Play("Jump");
        }

        public void OnExit()
        {

        }

        public void OnUpdate()
        {
            time += Time.deltaTime;
            speed -= Time.deltaTime * 60f;
            self.characterController.Move(speed * Vector3.up * Time.deltaTime + self.interia * Time.deltaTime * self.date.speed); 
            if (self.characterController.isGrounded)
            {
                self.TransState(PlayerStateType.Idle);
                self.characterController.Move(self.interia * Time.deltaTime * self.date.speed);
            }
        }
    }
    public class Attack : IPlayerState
    {
        public Player self;
        public float time;
        public bool isAttack = false;
        public bool Onparticle = false;
        public ParticleSystem particle;
        public GameObject hitBox;
        public Attack(Player self)
        {
            this.self = self;
        }

        public void OnEnter()
        {
            particle = self.GetComponentInChildren<ParticleSystem>();
            hitBox = self.hitBox;
            time = 0;
            if(self.particleType == 1)
            {   
                ParticleSystem.MainModule mainModule = particle.main;
                mainModule.startColor = Color.red;
            }
            else if (self.particleType == 2)
            {
                ParticleSystem.MainModule mainModule = particle.main;
                mainModule.startColor = Color.green;
            }
            else if (self.particleType == 3)
            {
                ParticleSystem.MainModule mainModule = particle.main;
                mainModule.startColor = Color.blue;
            }
            isAttack = true;
            hitBox.SetActive(true);
            if (self.combo == 0)
            {
                self.anim.Play("Attack_1");
                particle.Play();
            }
            else if(self.combo == 1 || self.combo == 2)
            {
                
            }
        }

        public void OnExit()
        {
            hitBox.SetActive(false);
        }

        public void OnUpdate()
        {
            time += Time.deltaTime;
            if(time > 1f)
            {
                self.TransState(PlayerStateType.Idle);
                self.combo = 0;
                self.anim.SetInteger("combo", self.combo);
            }
            else
            {
                if (self.input.PlayerBasic.Attack.WasPerformedThisFrame())
                {
                    if(self.combo < 2)
                    {
                        self.combo++;
                    }
                    self.anim.SetInteger("combo", self.combo);
                    time = 0;
                    self.TransState(PlayerStateType.Attack);
                }
            }
        }
    }
 }
