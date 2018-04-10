using System.Collections;
using System.Collections.Generic;
using pd;
using UnityEngine;


public class user_gui : MonoBehaviour {
    IUserAction uac;
    public int result { get; set; }
    
	// Use this for initialization
	void Start () {
        uac = Director.getInstance().currentSceneController as IUserAction;

	}
	
	// Update is called once per frame
	void OnGUI () {
        
       
        GUIStyle fontstyle = new GUIStyle();
        fontstyle.fontSize = 25;
        fontstyle.fontStyle = FontStyle.Bold;
		if(result == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 4 , 100, 100), "You win!", fontstyle);
            if (GUI.Button(new Rect(Screen.width / 2 -35, 3 * Screen.height / 4, 100, 100), "Restart"))
            {
                result = 0;
                uac.reset();
            }
        }
        if(result == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 4, 100, 100), "You loose!", fontstyle);
            if (GUI.Button(new Rect(Screen.width / 2 -35, 3 * Screen.height / 4, 100, 50), "Restart"))
            {
                result = 0;
                uac.reset();
            }
        }
	}
}
