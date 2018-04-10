using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pd;

namespace pd.manager
{
    public enum SSActionEventType : int { Started, Competeted }

    public interface ISSActionCallback
    {
        void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
            int intParam = 0, string strParam = null, Object objectParam = null);
    }

    public class SSAction : ScriptableObject
    {
        public bool enable = true;
        public bool destroy = false;

        public GameObject gameobject { get; set; }
        public Transform transform { get; set; }
        public ISSActionCallback callback { get; set; }

        protected SSAction() { }

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }



    public class CCMoveToAction : SSAction 
    {
        public Vector3 target;
        public float speed;

        private CCMoveToAction() { }
        public static CCMoveToAction GetSSAction(Vector3 target, float speed)
        {
            CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
            action.target = target;
            action.speed = speed;
            return action;
        }

        public override void Update()
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
            if (this.transform.position == target)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
        }

        public override void Start()
        {
            
        }
    }

    public class CCSequenceAction : SSAction, ISSActionCallback
    {
        public List<SSAction> sequence;
        public int repeat = -1;
        public int start = 0;

        public static CCSequenceAction GetSSAcition(int repeat, int start, List<SSAction> sequence)
        {
            CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
            action.repeat = repeat;
            action.sequence = sequence;
            action.start = start;
            return action;
        }

        public override void Update()
        {
            if (sequence.Count == 0) return;
            if (start < sequence.Count)
            {
                sequence[start].Update(); 
            }
        }

        public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
            int intParam = 0, string strParam = null, Object objectParam = null)
        {
            source.destroy = false;
            this.start++;
            if (this.start >= sequence.Count)
            {
                this.start = 0;
                if (repeat > 0) repeat--;
                if (repeat == 0)
                {
                    this.destroy = true;              
                    this.callback.SSActionEvent(this);
                }
            }
        }

        public override void Start()
        {
            foreach (SSAction action in sequence)
            {
                action.gameobject = this.gameobject;
                action.transform = this.transform;
                action.callback = this; 
                action.Start();
            }
        }

        void OnDestroy()
        {
            
        }
    }


    

    public class SSActionManager : MonoBehaviour, ISSActionCallback
    {

        private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
        private List<SSAction> waitingAdd = new List<SSAction>();
        private List<int> waitingDelete = new List<int>();             

        protected void Update()
        {
            foreach (SSAction ac in waitingAdd)
            {
                actions[ac.GetInstanceID()] = ac;
            }
            waitingAdd.Clear();

            foreach (KeyValuePair<int, SSAction> kv in actions)
            {
                SSAction ac = kv.Value;
                if (ac.destroy)
                {
                    waitingDelete.Add(ac.GetInstanceID());
                }
                else if (ac.enable)
                {
                    ac.Update();
                }
            }

            foreach (int key in waitingDelete)
            {
                SSAction ac = actions[key];
                actions.Remove(key);
                DestroyObject(ac);
            }
            waitingDelete.Clear();
        }

        public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
        {
            action.gameobject = gameobject;
            action.transform = gameobject.transform;
            action.callback = manager;
            waitingAdd.Add(action);
            action.Start();
        }

        public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
            int intParam = 0, string strParam = null, Object objectParam = null)
        {
            
        }
    }

    public class CCActionManager : SSActionManager 
    {

        private CCMoveToAction boat_action;     
        private CCSequenceAction pd_action;     
        public FirstController sceneController;
        float speed = 10;

        protected void Start()
        {
            sceneController = (FirstController)Director.getInstance().currentSceneController;
            sceneController.actionManager = this;
        }
        public void moveBoat(GameObject boat, Vector3 target)
        {
            boat_action = CCMoveToAction.GetSSAction(target, speed);
            this.RunAction(boat, boat_action, this);
        }

        public void movePD(GameObject pORd, Vector3 middle_pos, Vector3 end_pos)
        {
            SSAction first_action = CCMoveToAction.GetSSAction(middle_pos, speed);
            SSAction second_action = CCMoveToAction.GetSSAction(end_pos, speed);
            pd_action= CCSequenceAction.GetSSAcition(1, 0, new List<SSAction> { first_action, second_action });
            this.RunAction(pORd, pd_action, this);
        }
    }
}