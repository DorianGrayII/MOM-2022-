namespace MOM
{
    using MHUtils;
    using MHUtils.UI;
    using System;
    using UnityEngine;

    public class PlayMusic
    {
        public string libraryName;
        public AudioClip currentTrack;
        private WeakReference owner;

        public void Clear()
        {
            if (this.currentTrack != null)
            {
                this.currentTrack = null;
            }
        }

        public object GetOwner()
        {
            if (this.owner != null)
            {
                return this.owner.Target;
            }
            WeakReference owner = this.owner;
            return null;
        }

        public bool IsValid()
        {
            if ((this.owner == null) || (this.owner.Target == null))
            {
                return false;
            }
            ScreenBase target = this.owner.Target as ScreenBase;
            if (target != null)
            {
                return (target.stateStatus <= MHUtils.State.StateStatus.Closing);
            }
            if (this.owner.Target is GameManager)
            {
                return true;
            }
            Battle battle = this.owner.Target as Battle;
            return ((battle != null) && !battle.battleEnd);
        }

        public static void Play(string[] libraryName, object owner)
        {
            if (libraryName != null)
            {
                int index = UnityEngine.Random.Range(0, libraryName.Length);
                Play(libraryName[index], owner);
            }
        }

        public static void Play(string libraryName, object owner)
        {
            PlayMusic pm = new PlayMusic();
            pm.libraryName = libraryName;
            pm.owner = new WeakReference(owner);
            if (!pm.IsValid())
            {
                Debug.LogError("PlayMusic for invalid owner! do not guess, its dangerous to assign by guess and leads to memory leaks! ask KHASH");
            }
            AudioLibrary.RequestMusic(pm, false);
        }

        public void SetTrack(AudioClip ac)
        {
            this.Clear();
            this.currentTrack = ac;
        }
    }
}

