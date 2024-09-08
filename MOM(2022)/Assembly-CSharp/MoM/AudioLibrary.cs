// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.AudioLibrary
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DBDef;
using MHUtils;
using MOM;
using UnityEngine;
using UnityEngine.Networking;

public class AudioLibrary : MonoBehaviour
{
    private static AudioLibrary instance;

    private string sourceFolder;

    private Stack<AudioSource> unusedAudioSources = new Stack<AudioSource>();

    private Dictionary<string, AudioClip> sfx = new Dictionary<string, AudioClip>();

    private Dictionary<string, AudioClip> music = new Dictionary<string, AudioClip>();

    private Dictionary<object, float> remainingLoopSfx = new Dictionary<object, float>();

    private Stack<PlayMusic> musicStack = new Stack<PlayMusic>();

    private List<PlayMusic> managedRequests = new List<PlayMusic>();

    private Coroutine musicPlaying;

    private AudioSource musicAS;

    private PlayMusic musicASOwner;

    private string curentMusic;

    private bool musicLoading;

    private Coroutine reloadingSFX;

    private void Start()
    {
        AudioLibrary.instance = this;
        this.musicLoading = true;
    }

    public static AudioLibrary Get()
    {
        return AudioLibrary.instance;
    }

    private string GetSourceFolder()
    {
        if (string.IsNullOrEmpty(this.sourceFolder))
        {
            this.sourceFolder = Path.Combine(MHApplication.EXTERNAL_ASSETS, "Sound");
        }
        return this.sourceFolder;
    }

    public void ForceReloadSFX()
    {
        this.sfx.Clear();
        this.reloadingSFX = base.StartCoroutine(this.LoadingSFX());
    }

    public IEnumerator LoadingMusic()
    {
        List<SoundList> type = DataBase.GetType<SoundList>();
        string path = Path.Combine(this.GetSourceFolder(), "Music");
        float time = Time.timeSinceLevelLoad;
        foreach (SoundList item in type)
        {
            if (item.IsSFX)
            {
                continue;
            }
            Sound[] sound = item.sound;
            foreach (Sound v in sound)
            {
                if (time - Time.timeSinceLevelLoad > 0.015f)
                {
                    yield return null;
                    time = Time.timeSinceLevelLoad;
                }
                string file2 = AssetManager.GetAssetPath<AudioClip>(v.file);
                file2 = (string.IsNullOrEmpty(file2) ? Path.Combine(path, v.file) : file2);
                string text = "file:///" + UnityWebRequest.EscapeURL(file2);
                AudioType audioType = AudioType.WAV;
                if (text.EndsWith(".ogg"))
                {
                    audioType = AudioType.OGGVORBIS;
                }
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(text, audioType))
                {
                    DownloadHandlerAudioClip handler = www.downloadHandler as DownloadHandlerAudioClip;
                    handler.streamAudio = true;
                    www.SendWebRequest();
                    while (!www.isDone || !handler.isDone)
                    {
                        yield return null;
                    }
                    if (!string.IsNullOrEmpty(www.error) || www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log(www.error + "\nMusic " + file2 + " failed to load");
                    }
                    else
                    {
                        this.music[v.name] = DownloadHandlerAudioClip.GetContent(www);
                    }
                }
            }
        }
        Debug.Log("Reloading Music finished!");
        this.musicLoading = false;
    }

    public IEnumerator LoadingSFX()
    {
        _ = Time.realtimeSinceStartup;
        List<SoundList> list2 = DataBase.GetType<SoundList>().FindAll((SoundList o) => o.IsSFX);
        string path = Path.Combine(this.GetSourceFolder(), "SFX");
        int index = 0;
        float time = Time.timeSinceLevelLoad;
        foreach (SoundList list in list2)
        {
            Sound[] sound = list.sound;
            foreach (Sound v in sound)
            {
                if (time - Time.timeSinceLevelLoad > 0.015f)
                {
                    yield return null;
                    time = Time.timeSinceLevelLoad;
                }
                index++;
                MHEventSystem.TriggerEvent<GameLoader>(this, (float)index / (float)list.sound.Length);
                string file2 = AssetManager.GetAssetPath<AudioClip>(v.file);
                file2 = (string.IsNullOrEmpty(file2) ? Path.Combine(path, v.file) : file2);
                string text = "file:///" + UnityWebRequest.EscapeURL(file2);
                AudioType audioType = AudioType.WAV;
                if (text.EndsWith(".ogg"))
                {
                    audioType = AudioType.OGGVORBIS;
                }
                _ = v.name == "Click";
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(text, audioType))
                {
                    yield return www.SendWebRequest();
                    if (!string.IsNullOrEmpty(www.error) || www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.Log(www.error + "\nSFX " + file2 + " failed to load");
                    }
                    else
                    {
                        this.sfx[v.name] = DownloadHandlerAudioClip.GetContent(www);
                    }
                }
            }
        }
        Debug.Log("Reloading SFX finished!");
        this.reloadingSFX = null;
    }

    private void Update()
    {
    }

    public static void RequestMusic(PlayMusic pm, bool repeat = false)
    {
        MHTimer.StartNew();
        if (pm == null)
        {
            return;
        }
        if (!pm.IsValid())
        {
            if (pm == AudioLibrary.Get().musicASOwner)
            {
                AudioLibrary.Get().musicASOwner = null;
            }
            return;
        }
        if (AudioLibrary.Get().managedRequests.Find((PlayMusic o) => o.libraryName == pm.libraryName) == null)
        {
            AudioLibrary.Get().managedRequests.Add(pm);
        }
        if (AudioLibrary.Get().musicPlaying != null)
        {
            if (!repeat && AudioLibrary.Get().curentMusic != null && pm.libraryName == AudioLibrary.Get().curentMusic)
            {
                return;
            }
            AudioLibrary.Get().StopCoroutine(AudioLibrary.Get().musicPlaying);
            AudioLibrary.Get().musicPlaying = null;
        }
        AudioLibrary.Get().musicPlaying = AudioLibrary.Get().StartCoroutine(AudioLibrary.Get().PlayingMusic(pm));
    }

    public static void RequestSFX(string playSFX, float randomSize = 0f, float randomMiliseconds = 0f, float volume = 1f)
    {
        if (AudioLibrary.Get() == null)
        {
            Debug.LogWarning("Missing audio library!");
        }
        else if (!string.IsNullOrEmpty(playSFX) && AudioLibrary.Get().reloadingSFX == null && AudioLibrary.Get().sfx.ContainsKey(playSFX))
        {
            AudioLibrary.Get().StartCoroutine(AudioLibrary.Get().InternalPlaySFX(AudioLibrary.Get().sfx[playSFX], randomSize, randomMiliseconds, volume));
        }
    }

    public static void RequestLoopingSFX(string playSFX, float expectedLength, object owner)
    {
        if (!string.IsNullOrEmpty(playSFX) && AudioLibrary.Get().reloadingSFX == null && AudioLibrary.Get().sfx.ContainsKey(playSFX))
        {
            if (AudioLibrary.Get().remainingLoopSfx.ContainsKey(owner))
            {
                AudioLibrary.Get().remainingLoopSfx[owner] = expectedLength;
                return;
            }
            AudioLibrary.Get().remainingLoopSfx[owner] = expectedLength;
            AudioLibrary.Get().StartCoroutine(AudioLibrary.Get().PlayingLoopSFX(AudioLibrary.Get().sfx[playSFX], owner));
        }
    }

    public static void StopLoopingSFX(object owner)
    {
        if (AudioLibrary.Get().remainingLoopSfx.ContainsKey(owner))
        {
            AudioLibrary.Get().remainingLoopSfx.Remove(owner);
        }
    }

    private void ValidateStack()
    {
        if (this.musicStack != null && this.musicStack.Count != 0)
        {
            if (this.musicStack.Count == 1)
            {
                if (!this.musicStack.Peek().IsValid())
                {
                    this.musicStack.Clear();
                }
            }
            else
            {
                List<PlayMusic> toRemove = null;
                foreach (PlayMusic item in this.musicStack)
                {
                    if (!item.IsValid())
                    {
                        if (toRemove == null)
                        {
                            toRemove = new List<PlayMusic>();
                        }
                        toRemove.Add(item);
                    }
                }
                if (toRemove != null)
                {
                    List<PlayMusic> list = new List<PlayMusic>(this.musicStack);
                    list = list.FindAll((PlayMusic o) => !toRemove.Contains(o));
                    this.musicStack = new Stack<PlayMusic>(list);
                }
            }
        }
        for (int num = this.managedRequests.Count - 1; num >= 0; num--)
        {
            PlayMusic playMusic = this.managedRequests[num];
            if (playMusic == null)
            {
                this.managedRequests.RemoveAt(num);
            }
            else if (!playMusic.IsValid() && this.musicAS?.clip != playMusic.currentTrack && this.managedRequests.Count > 1)
            {
                playMusic.Clear();
                this.managedRequests.RemoveAt(num);
            }
        }
    }

    private void ClearStackBy(object obj)
    {
        if (this.musicStack == null)
        {
            return;
        }
        List<PlayMusic> toRemove = null;
        foreach (PlayMusic item in this.musicStack)
        {
            if (item.GetOwner() == obj && toRemove == null)
            {
                toRemove = new List<PlayMusic>();
            }
            toRemove?.Add(item);
        }
        if (toRemove == null)
        {
            return;
        }
        List<PlayMusic> list = new List<PlayMusic>(this.musicStack);
        list = list.FindAll((PlayMusic o) => !toRemove.Contains(o));
        this.musicStack = new Stack<PlayMusic>(list);
        foreach (PlayMusic item2 in toRemove)
        {
            item2.Clear();
        }
    }

    private void AddToStack(PlayMusic pm)
    {
        this.ValidateStack();
        this.ClearStackBy(pm.GetOwner());
        if (this.musicStack == null)
        {
            this.musicStack = new Stack<PlayMusic>();
        }
        this.musicStack.Push(pm);
        Debug.Log("music stack " + this.musicStack.Count);
    }

    private PlayMusic GetFromStack()
    {
        if (this.musicStack == null)
        {
            return null;
        }
        this.ValidateStack();
        if (this.musicStack.Count > 0)
        {
            return this.musicStack.Pop();
        }
        return null;
    }

    private IEnumerator PlayingMusic(PlayMusic pm)
    {
        if (pm == null || string.IsNullOrEmpty(pm.libraryName))
        {
            Debug.LogWarning("Missing music setting while called to play music!");
            yield break;
        }
        SoundList soundList = DataBase.Get<SoundList>(pm.libraryName, reportMissing: true);
        int num = Random.Range(0, soundList.sound.Length);
        Sound soundTrack = soundList.sound[num];
        if (this.musicAS != null)
        {
            if (this.musicASOwner != null)
            {
                if (this.musicASOwner.IsValid())
                {
                    if (this.musicASOwner.GetOwner() != pm.GetOwner())
                    {
                        this.AddToStack(this.musicASOwner);
                    }
                }
                else
                {
                    this.musicASOwner.Clear();
                    int num2 = this.managedRequests.IndexOf(this.musicASOwner);
                    if (num2 > -1)
                    {
                        this.managedRequests.RemoveAt(num2);
                    }
                }
            }
            this.musicASOwner = null;
            while (this.musicAS.volume > 0f && this.musicAS.isPlaying)
            {
                this.musicAS.volume = Mathf.Max(0f, this.musicAS.volume - 0.02f);
                yield return null;
            }
            if (this.musicAS.isPlaying)
            {
                this.musicAS.Stop();
            }
        }
        if (this.musicAS == null)
        {
            this.musicAS = base.gameObject.AddComponent<AudioSource>();
        }
        while (!this.music.ContainsKey(soundTrack.name) && this.musicLoading)
        {
            yield return null;
        }
        AudioClip ac = (this.music.ContainsKey(soundTrack.name) ? this.music[soundTrack.name] : null);
        pm.SetTrack(ac);
        this.musicAS.volume = 0f;
        this.musicAS.PlayOneShot(ac);
        this.musicASOwner = pm;
        this.curentMusic = pm.libraryName;
        while (this.musicAS.volume < this.GetMusicVolume())
        {
            this.musicAS.volume = Mathf.Min(this.GetMusicVolume(), this.musicAS.volume + 0.02f);
            yield return null;
        }
        while (this.musicAS != null)
        {
            if (!this.musicAS.isPlaying)
            {
                if (this.musicASOwner != null || !(ac != null))
                {
                    break;
                }
                this.musicAS.PlayOneShot(ac);
            }
            float num3 = this.GetMusicVolume() - this.musicAS.volume;
            if (num3 != 0f)
            {
                if (Mathf.Abs(num3) < 0.02f)
                {
                    this.musicAS.volume = this.GetMusicVolume();
                }
                else
                {
                    this.musicAS.volume += Mathf.Sign(num3) * 0.02f;
                }
            }
            yield return null;
            if (this.musicASOwner != null && this.musicASOwner.IsValid())
            {
                continue;
            }
            this.musicASOwner = null;
            PlayMusic fromStack = this.GetFromStack();
            if (fromStack != null)
            {
                AudioLibrary.RequestMusic(fromStack);
                yield return null;
                continue;
            }
            while (this.musicAS != null && this.musicAS.isPlaying)
            {
                num3 = this.GetMusicVolume() - this.musicAS.volume;
                if (num3 != 0f)
                {
                    if (Mathf.Abs(num3) < 0.02f)
                    {
                        this.musicAS.volume = this.GetMusicVolume();
                    }
                    else
                    {
                        this.musicAS.volume += Mathf.Sign(num3) * 0.02f;
                    }
                }
                yield return null;
            }
            break;
        }
        if (this.musicASOwner != null && this.musicASOwner.IsValid())
        {
            AudioLibrary.Get().musicPlaying = null;
            AudioLibrary.RequestMusic(this.musicASOwner, repeat: true);
        }
    }

    private IEnumerator InternalPlaySFX(AudioClip ac, float randomSize, float randomMilisecondDelay, float volume)
    {
        AudioSource aSource = ((this.unusedAudioSources.Count <= 0) ? base.gameObject.AddComponent<AudioSource>() : this.unusedAudioSources.Pop());
        if (randomMilisecondDelay > 0f)
        {
            yield return new WaitForSeconds(Random.Range(0.001f, randomMilisecondDelay * 0.001f));
        }
        aSource.clip = ac;
        aSource.volume = this.GetSfxVolume() * volume;
        aSource.pitch = 1f + Random.Range(0f - randomSize, randomSize);
        aSource.loop = false;
        aSource.PlayOneShot(ac);
        while (aSource.isPlaying)
        {
            yield return null;
        }
        this.unusedAudioSources.Push(aSource);
    }

    private IEnumerator PlayingLoopSFX(AudioClip ac, object owner)
    {
        AudioSource aSource = ((this.unusedAudioSources.Count <= 0) ? base.gameObject.AddComponent<AudioSource>() : this.unusedAudioSources.Pop());
        float initialStrength = this.GetSfxVolume();
        aSource.clip = ac;
        aSource.volume = initialStrength;
        aSource.pitch = 1f;
        aSource.loop = true;
        aSource.Play();
        float decayTime = 0.5f;
        while (aSource.isPlaying && this.remainingLoopSfx.ContainsKey(owner))
        {
            float num = this.remainingLoopSfx[owner] - Time.deltaTime;
            this.remainingLoopSfx[owner] = num;
            aSource.volume = Mathf.Clamp01(num / decayTime) * initialStrength;
            if (num <= 0f)
            {
                break;
            }
            yield return null;
        }
        aSource.Stop();
        this.unusedAudioSources.Push(aSource);
        if (this.remainingLoopSfx.ContainsKey(owner))
        {
            this.remainingLoopSfx.Remove(owner);
        }
    }

    private string GetMusicPath(string musicFileName)
    {
        string path = Path.Combine(this.GetSourceFolder(), "Music");
        string assetPath = AssetManager.GetAssetPath<AudioClip>(musicFileName);
        assetPath = (string.IsNullOrEmpty(assetPath) ? Path.Combine(path, musicFileName) : assetPath);
        return "file:///" + UnityWebRequest.EscapeURL(assetPath);
    }

    private float GetMusicVolume()
    {
        return (float)Settings.GetData().Get<int>(Settings.Name.musicVolume) * 0.01f;
    }

    private float GetSfxVolume()
    {
        return (float)Settings.GetData().Get<int>(Settings.Name.sfxVolume) * 0.01f;
    }
}
