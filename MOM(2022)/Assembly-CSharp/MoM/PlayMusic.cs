using System;
using MHUtils;
using MHUtils.UI;
using UnityEngine;

namespace MOM
{
    public class PlayMusic
    {
        public string libraryName;

        public AudioClip currentTrack;

        private WeakReference owner;

        public static void Play(string[] libraryName, object owner)
        {
            if (libraryName != null)
            {
                int num = global::UnityEngine.Random.Range(0, libraryName.Length);
                PlayMusic.Play(libraryName[num], owner);
            }
        }

        public static void Play(string libraryName, object owner)
        {
            PlayMusic obj = new PlayMusic
            {
                libraryName = libraryName,
                owner = new WeakReference(owner)
            };
            if (!obj.IsValid())
            {
                Debug.LogError("PlayMusic for invalid owner! do not guess, its dangerous to assign by guess and leads to memory leaks! ask KHASH");
            }
            AudioLibrary.RequestMusic(obj);
        }

        public void SetTrack(AudioClip ac)
        {
            this.Clear();
            this.currentTrack = ac;
        }

        public void Clear()
        {
            if (this.currentTrack != null)
            {
                this.currentTrack = null;
            }
        }

        public bool IsValid()
        {
            if (this.owner == null || this.owner.Target == null)
            {
                return false;
            }
            if (this.owner.Target is ScreenBase screenBase)
            {
                if (screenBase.stateStatus <= State.StateStatus.Closing)
                {
                    return true;
                }
                return false;
            }
            if (this.owner.Target is GameManager)
            {
                return true;
            }
            if (this.owner.Target is Battle battle)
            {
                return !battle.battleEnd;
            }
            return false;
        }

        public object GetOwner()
        {
            return this.owner?.Target;
        }
    }
}
