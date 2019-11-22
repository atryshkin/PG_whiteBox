using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PieGame
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();

        StateManager states;

        public void Init(StateManager st)
        {
            states = st;

            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.curWeapon;

            for (int i = 0; i < w.actions.Count; i++)
            {
                Action a = GetAction(w.actions[i].input);
                a.targetAnim = w.actions[i].targetAnim;
            }
        }


        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.curWeapon;

            for (int i = 0; i < w.th_actions.Count; i++)
            {
                Action a = GetAction(w.th_actions[i].input);
                a.targetAnim = w.th_actions[i].targetAnim;
            }
        }

        void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnim = null;
            }
        }

        ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();

                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }

        public Action GetActionSlot(StateManager st)
        {
            ActionInput a_input = GetActionInput(st);

            return GetAction(a_input);
        }

        Action GetAction(ActionInput inp)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }

            return null;
        }

        public ActionInput GetActionInput(StateManager st)
        {
            if (st.rb)
                return ActionInput.rb_LeftClick;
            if (st.rt)
                return ActionInput.rt_Rkey;
            if (st.lb)
                return ActionInput.lb_Zkey;
            if (st.lt)
                return ActionInput.lt_RightClick;

            return ActionInput.rb_LeftClick;
        }

	}

    public enum ActionInput
    {
        rb_LeftClick,lb_Zkey,rt_Rkey,lt_RightClick
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnim;

    }
}

