using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pd;

public class click_gui : MonoBehaviour {
    IUserAction uac;
    pd_controller controling_pd;
	// Use this for initialization
	void Start () {
        uac = Director.getInstance().currentSceneController as IUserAction;
	}

    public void set_controling(pd_controller controling)
    {
        controling_pd = controling;
        
    }
	/*void Update()
    {

        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //定义一条射线，这条射线从摄像机屏幕射向鼠标所在位置
            RaycastHit hit; //声明一个碰撞的点(暂且理解为碰撞的交点)
            if (Physics.Raycast(ray, out hit)) //如果真的发生了碰撞，ray这条射线在hit点与别的物体碰撞了
            {
                OnClick();
            }
        }
    }
    // Update is called once per frame*/
    void OnMouseDown()
    {
        
        if (gameObject.name == "boat")
        {
            uac.moveBoat();
        }
        else
        {
            uac.movePD(controling_pd); ;
        }
            
    }
}
