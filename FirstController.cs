using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pd;
using pd.manager;

public class FirstController : MonoBehaviour, ISceneController, IUserAction {
    public coast_controller right_coast;
    public coast_controller left_coast;
    public boat_controller boat;
    public pd_controller[] pds;
    user_gui userGui;
    public CCActionManager actionManager;

    public void loadResources()
    {
        left_coast = new coast_controller(false);
        right_coast = new coast_controller(true);
        boat = new boat_controller();
        pds = new pd_controller[6];
        for(int i = 0; i < 3; i++)
        {
            pd_controller p = new pd_controller(true);
            Vector3 temp = right_coast.GetEmptyPos();
            p.set_pos(temp);
            if (temp.x < 0)
                p.pos_index = (int)-(temp.x + 6.5);
            else
                p.pos_index = (int)(temp.x - 6.5);
            p.set_coast(right_coast);
            right_coast.insert(p);
            p.set_name(i.ToString() + "priest");
            pds[i] = p;
            pd_controller d = new pd_controller(false);
            temp = right_coast.GetEmptyPos();
            d.set_pos(temp);
            if (temp.x < 0)
                d.pos_index = (int)-(temp.x + 6.5);
            else
                d.pos_index = (int)(temp.x - 6.5);
            d.set_coast(right_coast);
            right_coast.insert(d);
            d.set_name(i.ToString() + "devil");
            pds[i + 3] = d;
        }
        //GameObject river = Instantiate(Resources.Load("perfab/river", typeof(GameObject)), new Vector3(0, 0.4F, 0), Quaternion.identity, null) as GameObject;
        //GameObject bg = Instantiate(Resources.Load("perfab/bg", typeof(GameObject)), new Vector3(0, 0, 15), Quaternion.Euler(270,0,0), null) as GameObject;
    }

	// Use this for initialization
	void Awake () {
        Director director = Director.getInstance();
        director.currentSceneController = this;
        userGui = gameObject.AddComponent(typeof(user_gui)) as user_gui;   
        loadResources();

        actionManager = gameObject.AddComponent<CCActionManager>() as CCActionManager;
    }

    public void moveBoat()
    {
        int side_index;
        if (boat.get_side())
            side_index = 1;
        else
            side_index = -1;
        if (boat.empty())
            return;
        else if (boat.get_boat().transform.position != new Vector3(side_index * 4.5F, 1, 0))
            return;
        else
        {
            actionManager.moveBoat(boat.get_boat(), boat.get_to_pos());
            userGui.result = check_is_win();
        }
    }

    public void movePD(pd_controller pORd)
    {
        if(boat.is_on_boat(pORd))
        {
            coast_controller coast;
            if (boat.get_side())
                coast = right_coast;
            else
                coast = left_coast;

            Vector3 temp = coast.GetEmptyPos();



            Vector3 end_pos = coast.GetEmptyPos();                                         //动作分离版本改变
            Vector3 middle_pos = new Vector3(pORd.get_pd().transform.position.x, end_pos.y, end_pos.z);  //动作分离版本改变
            actionManager.movePD(pORd.get_pd(), middle_pos, end_pos);  //动作分离版本改变
            if (coast.GetEmptyPos().x < 0)
                pORd.pos_index = (int)-(temp.x + 6.5);
            else
                pORd.pos_index = (int)(temp.x - 6.5);
            pORd.set_coast(coast);
            coast.insert(pORd);
            boat.remove(pORd);
            
            
        }
        else
        {
            if(boat.is_full())
            {
                return;
            }
            
            if(left_coast.is_on_coast(pORd))
            {
                if (boat.get_side())
                    return;
                pORd.set_boat(boat);

                Vector3 temp = boat.GetEmptyPos();
                Vector3 end_pos = temp;                                             //动作分离版本改变
                Vector3 middle_pos = new Vector3(end_pos.x, pORd.get_pd().transform.position.y, end_pos.z); //动作分离版本改变
                actionManager.movePD(pORd.get_pd(), middle_pos, end_pos);  //动作分离版本改变
                if (temp.x < 0)
                    pORd.boat_pos_index = (int)-(temp.x + 3.5);
                else
                    pORd.boat_pos_index = (int)(temp.x - 3.5);
                left_coast.remove(pORd);
                boat.insert(pORd);
               
                
            }
            else
            {
                if (!boat.get_side())
                    return;
                
                right_coast.remove(pORd);
                Vector3 temp = boat.GetEmptyPos();
                Vector3 end_pos = temp;                                             //动作分离版本改变
                Vector3 middle_pos = new Vector3(end_pos.x, pORd.get_pd().transform.position.y, end_pos.z); //动作分离版本改变
                actionManager.movePD(pORd.get_pd(), middle_pos, end_pos);  //动作分离版本改变
               
                if (temp.x < 0)
                    pORd.boat_pos_index = (int)-(temp.x + 3.5);
                else
                    pORd.boat_pos_index = (int)(temp.x - 3.5);
                pORd.set_boat(boat);
                boat.insert(pORd);
               
                
                
            }
        }
        userGui.result = check_is_win();
    }

    int check_is_win()
    {
        int count = 0;
        for(int i =0; i <pds.Length; i++)
        {
            if (left_coast.is_on_coast(pds[i]))
                count++;
        }
        Debug.Log(count);
        if (count == 6) return 1;

        if(!boat.get_side())
        {
            if (left_coast.get_pcount() + boat.get_pcount() < left_coast.get_dcount() + boat.get_dcount() && left_coast.get_pcount() + boat.get_pcount() != 0)
                return 2;
            if (right_coast.get_pcount() < right_coast.get_dcount() && right_coast.get_pcount() != 0)
                return 2;
        }
        else
        {
            if (right_coast.get_pcount() + boat.get_pcount() < right_coast.get_dcount() + boat.get_dcount() && right_coast.get_pcount() + boat.get_pcount() != 0)
                return 2;
            if (left_coast.get_pcount() < left_coast.get_dcount() && left_coast.get_pcount() != 0)
                return 2;
        }
        

        return 0;
    }

    public void reset()
    {
        boat.reset();
        left_coast.reset();
        right_coast.reset();
        for (int i = 0; i < pds.Length; i++)
        {
            pds[i].reset();
        }
        for (int i = 0; i < pds.Length; i++)
        {
            Vector3 temp = right_coast.GetEmptyPos();



            pds[i].set_pos(right_coast.GetEmptyPos());  
            if (right_coast.GetEmptyPos().x < 0)
                pds[i].pos_index = (int)-(temp.x + 6.5);
            else
                pds[i].pos_index = (int)(temp.x - 6.5);
           
            right_coast.insert(pds[i]);
        }
    }

    
}
