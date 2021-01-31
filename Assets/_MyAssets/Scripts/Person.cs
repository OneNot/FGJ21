using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public GameObject SpeechBubblePrefab;

    private string ownedItem;
    private SpeechBubble currentSpeechBubble;
    [SerializeField]
    private RectTransform rectTransform;

    private float lifeTime, lifeStart;

    [SerializeField]
    private AudioSource audioSource;

    private string voiceUsed;

    public Queue<AudioClip> AudioQueue = new Queue<AudioClip>();


    private void Start() {
        lifeStart = Time.time;
    }

    private void Update()
    {
        lifeTime = Time.time - lifeStart;
        if (lifeTime > PersonSpawner.Instance.PeopleStartGeneratingAngerAt)
            ScoreManager.Instance.IncreaseCrowdAnger(Time.deltaTime * PersonSpawner.Instance.PeopleAngerAmountPerSecond);
    }


    public void TakeOwnershipOfItem(Item item)
    {
        ownedItem = item.Name;
    }

    public void AskForItem()
    {
        Speak(SpeechBuild());
    }


    private void NewSpeechBubble()
    {
        RemoveSpeechBubble();
        currentSpeechBubble = Instantiate(SpeechBubblePrefab, transform).GetComponent<SpeechBubble>();
    }

    private void RemoveSpeechBubble()
    {
        if (currentSpeechBubble != null)
            Destroy(currentSpeechBubble.gameObject); 
    }

    public void Speak(string text)
    {
        if (currentSpeechBubble == null)
            NewSpeechBubble();

        currentSpeechBubble.SetText(text);
    }


    private string SpeechBuild()
    {
        ItemPrefab itemPref = ItemHandler.Instance.ItemPrefs.Find(x => x.Name == ownedItem);
        ItemPrefabVoice itemPrefVoice = itemPref.Voices[Random.Range(0, itemPref.Voices.Count)];
        voiceUsed = itemPrefVoice.VoiceName;
        AudioClip itemAudio = itemPrefVoice.Clip;

        List<Phrase> PhrasesWithRightVoice = PersonSpawner.Instance.Phrases.FindAll(x => x.Voice == voiceUsed);
        Phrase phrase = PhrasesWithRightVoice[Random.Range(0, PhrasesWithRightVoice.Count)];
        string phraseString = phrase.Body;

        audioSource.pitch = Random.Range(0.65f, 1.55f);

        AudioQueue.Clear();
        AudioQueue.Enqueue(phrase.Clips[0]);
        AudioQueue.Enqueue(itemAudio);
        if (phrase.Clips.Count > 1)
            AudioQueue.Enqueue(phrase.Clips[1]);

        StartCoroutine(PlayAudioQueue());

        return phraseString.Replace("{item}", ownedItem);
    }

    private IEnumerator PlayAudioQueue()
    {
        while (AudioQueue.Count > 0)
        {
            audioSource.clip = AudioQueue.Dequeue();
            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);
        }
    }

    public bool IsMouseHoveringOnPersonWithRightItem(Item item)
    {
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return (rectTransform.rect.Contains(localMousePosition) && ownedItem == item.Name);
    }

    public void GivePersonItem(Item item)
    {
        PersonSpawner.Instance.ActivePersons.Remove(this);
        ItemHandler.Instance.RemoveItemFromActiveItems(item);
        Thank();
        StartCoroutine(LeaveAfterSeconds(PersonSpawner.Instance.AfterItemGetWaitTimeBeforeLeave));
    }

    private void Thank()
    {
        List<Phrase> PhrasesWithRightVoice = PersonSpawner.Instance.ThanksPhrases.FindAll(x => x.Voice == voiceUsed);
        Phrase phrase = PhrasesWithRightVoice[Random.Range(0, PhrasesWithRightVoice.Count)];
        string phraseString = phrase.Body;

        AudioQueue.Clear();
        AudioQueue.Enqueue(phrase.Clips[0]);
        StartCoroutine(PlayAudioQueue());

        Speak(phraseString);
    } 

    private IEnumerator LeaveAfterSeconds(float timeToLeave)
    {
        yield return new WaitForSeconds(timeToLeave);
        RemoveSpeechBubble();
        PersonSpawner.Instance.AnimatePersonLeaveThenDestroy(this);
    }
}
