using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightingFly : MonoBehaviour
{
    public List<Vector3> posList = new List<Vector3>(); // 用于存放小球将要移动的位置
    public bool isWait = false; // 用于记录小球是否在原地等待
    public bool isMove = false; // 用于记录小球是否正在移动
    public bool canMove = false; // 用于记录小球此时是否可以移动
    public float waitTime = 1; // 小球的原地等待时间
    public float elapsedTime = 0;
    public float speed = 0.05f; // 小球的移动速度，匀速
    public int count = 0; // 记录小球目前处于列表中的第几个位置
    private ParticleSystem pal;

    private void Awake()
    {
        pal = GetComponent<ParticleSystem>();
        this.transform.position = posList[0];
    }
    private void Update()
    {
        if (!isWait && count<posList.Count && canMove)
        {
            Move();
        }
        if(this.transform.position == posList[count] && count<posList.Count)
        {
            isMove = false;
            canMove = false;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(!isMove && !isWait && collision.gameObject.tag == "Player" && count < posList.Count-1 && !canMove)
        {
            isWait = true;
            StartCoroutine("flyWait");
            count++;
            canMove = true;
        }
    }

    private void Move()
    {
        // 获取当前位置和目标位置  
        Vector3 currentPos = this.transform.position;
        Vector3 targetPos = posList[count];

        Vector3 direction = targetPos - currentPos;
        float distance = direction.magnitude;

        // 如果距离为零，则无需移动  
        if (distance == 0f)
        {
            return;
        }

        // 归一化方向向量，这样它只包含方向信息，没有长度  
        direction.Normalize();

        // 计算物体以speed速度移动需要的时间  
        float timeToReachTarget = distance / speed;

        // 需要一个方法来获取已经过去的时间（比如使用Time.deltaTime累加）  
        // 一个变量叫做 elapsedTime 来跟踪已经过去的时间  
        elapsedTime += Time.deltaTime;  

        // 检查是否已到达目标位置  
        if (elapsedTime >= timeToReachTarget)
        {
            // 到达目标位置，更新位置并可能进行其他逻辑处理（比如设置isMove为false，增加count等）  
            this.transform.position = targetPos;
            isMove = false; // 想在到达目标后停止移动  
            elapsedTime = 0f; // 重置计时器  
        }
        else
        {
            // 根据已经过去的时间和速度来更新位置  
            float distanceCovered = speed * elapsedTime;
            this.transform.position = currentPos + direction * distanceCovered;
        }
    }

    IEnumerator flyWait()
    {
        pal.Play();
        yield return new WaitForSeconds(1);
        isWait = false;
        yield return null;
    }
}
