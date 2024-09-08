using System.Collections;
using HutongGames.PlayMaker;

namespace MOM
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class FSMAdventure : FSMStateBase
    {
        public static FSMAdventure instance;

        private bool advBusy;

        public override void OnEnter()
        {
            FSMAdventure.instance = this;
            this.advBusy = false;
        }

        public static bool IsRunning()
        {
            if (FSMAdventure.instance == null)
            {
                return false;
            }
            return FSMAdventure.instance.advBusy;
        }

        public static IEnumerator QueueOne(IEnumerator adventureHandler)
        {
            while (FSMAdventure.instance.advBusy)
            {
                yield return null;
            }
            FSMAdventure.instance.advBusy = true;
            yield return adventureHandler;
            FSMAdventure.instance.advBusy = false;
        }
    }
}
