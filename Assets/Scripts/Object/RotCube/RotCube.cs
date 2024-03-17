using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotCube : MonoBehaviour
{
    public float speed; // 用于存放方块的旋转速度
    public List<GameObject> cubeList = new List<GameObject>();
    public List<float> rotAngel = new List<float>();
    public int index;
    public int finishIndex;
    public bool canRot; // 用于存放方块是否可以旋转
    public bool isRot;
    public float curRot = 0;
    public Vector3 tempRot = new Vector3();
    public bool allStop = false;
    public GameObject awardBox;

    private void Update()
    {
        if (!allStop)
        {
            Rot();
            Check();
            checkAllCube();
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "HitBox" && !isRot)
        {
            canRot = true;
        }
    }

    private void Rot()
    {
        tempRot = transform.rotation.eulerAngles;
        if (canRot)
        {
            curRot += Time.deltaTime;
            this.transform.rotation = Quaternion.Euler(0.0f, 90*(index-1)+(curRot * speed), 0.0f);
            isRot = true;
        }
        if(tempRot.y >= rotAngel[index] - 0.6 && canRot)
        {
            canRot = false;
            isRot = false;
            curRot = 0;
            index++;
        }
    }

    private void Check()
    {
        if(index > 4)
        {
            index = 1;
        }
        if(tempRot.y <= 0)
        {
            tempRot.y += 180;
        }
    }

    private void checkAllCube()
    {
        if(cubeList.Count > 0)
        {
            if (index == finishIndex)
            {
                int num = 0;
                foreach (GameObject cube in cubeList)
                {
                    if (cube.GetComponent<RotCube>().finishIndex == cube.GetComponent<RotCube>().index)
                    {
                        num++;
                    }
                }
                if (cubeList.Count == num)
                {
                    allStop = true;
                    awardBox.SetActive(true);
                }
            }
        }
    }
}
