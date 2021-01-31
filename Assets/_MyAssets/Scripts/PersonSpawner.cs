using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonSpawner : MonoBehaviour
{
    #region Static Instance Handling
    private static PersonSpawner instance;
    public static PersonSpawner Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<PersonSpawner>();
            if (instance == null)
                Debug.LogError("Could not find ItemHandler");

            return instance;
        }
    }
    void OnDisable()
    {
        instance = null;
    }
    #endregion



    public GameObject PersonPrefab;

    [Range(0f, 1f)]
    public float SpawnXMinNormalized, SpawnXMaxNormalized, SpawnYMinNormalized, SpawnYMaxNormalized, SpawnYPopupAnimStartNormalized;

    public AnimationCurve PersonPopupAnimationCurve;

    [Range(0.01f, 5f)]
    public float AnimationDuration;

    public float AfterItemGetWaitTimeBeforeLeave;

    [Range(0.1f, 120f)]
    public float MaxSpawnIntervalAverage, MinSpawnIntervalAverage;

    [Range(0f, 3600)]
    public float SpawnIntervalDecreaseDuration;

    [Range(0f, 1f)]
    public float SpawnIntervalLowerVariationMultiplier;

    [Range(1f, 2f)]
    public float SpawnIntervalHigherVariationMultiplier;

    public AnimationCurve SpawnIntevalDecreaseCurve;
    public float PeopleStartGeneratingAngerAt = 30f;
    public float PeopleAngerAmountPerSecond = 0.01f;

    [SerializeField]
    public List<Phrase> Phrases = new List<Phrase>();

    [SerializeField]
    public List<Phrase> ThanksPhrases = new List<Phrase>();


    [Header("=== Don't edit ===")]
    public List<Person> ActivePersons = new List<Person>();


    private float spawnXMin, spawnXMax, spawnYMin, spawnYMax, spawnYPopupAnimStart;
    private float currentSpawnInterval, currentSpawnIntervalAverage, lastSpawnTime;

    private void Awake() {
        spawnXMin = Screen.width * SpawnXMinNormalized;
        spawnXMax = Screen.width * SpawnXMaxNormalized;
        currentSpawnIntervalAverage = MaxSpawnIntervalAverage;
        currentSpawnInterval = Random.Range(currentSpawnIntervalAverage * SpawnIntervalLowerVariationMultiplier, currentSpawnIntervalAverage * SpawnIntervalHigherVariationMultiplier);
        lastSpawnTime = 0;
    }

    private void Update()
    {
        //if current spawn interval average is bigger than the smallest allowed, decrease it according to the SpawnIntevalDecreaseCurve
        if (currentSpawnIntervalAverage > MinSpawnIntervalAverage)
        {
            currentSpawnIntervalAverage = Mathf.Lerp(currentSpawnIntervalAverage, MinSpawnIntervalAverage, SpawnIntevalDecreaseCurve.Evaluate(Time.time / SpawnIntervalDecreaseDuration));
        }

        //if it has been longer than current spawn interval, spawn a new person
        if (Time.time - lastSpawnTime >= currentSpawnInterval)
        {
            SpawnPerson();
            lastSpawnTime = Time.time;

            //randomize next spawn interval from current spawn interval average
            currentSpawnInterval = Random.Range(currentSpawnIntervalAverage * SpawnIntervalLowerVariationMultiplier, currentSpawnIntervalAverage * SpawnIntervalHigherVariationMultiplier);
        }

        //Debug.Log("currentSpawnInterval: " + currentSpawnInterval);
        

        // if (Input.GetMouseButtonDown(0))
        // {
        //     SpawnPerson();
        // }
    }

    private void SpawnPerson()
    {
        UpdateSpawnPositionsByScreenSize();
        Person newPerson = Instantiate(PersonPrefab, new Vector3(Random.Range(spawnXMin, spawnXMax), spawnYPopupAnimStart), Quaternion.identity, transform).GetComponent<Person>();
        newPerson.transform.SetAsFirstSibling();
        ActivePersons.Add(newPerson);

        Item spawnedItem = ItemHandler.Instance.SpawnRandomItem();
        newPerson.TakeOwnershipOfItem(spawnedItem);

        StartCoroutine(AnimatedPersonPopup(newPerson, Random.Range(spawnYMin, spawnYMax)));
    }

    private IEnumerator AnimatedPersonPopup(Person person, float endYPos)
    {
        yield return AnimatePersonMove(person.transform, endYPos);
        person.AskForItem();
    }
    private IEnumerator AnimatePersonMove(Transform person, float endYPos)
    {
        float timer = 0f;
        Vector3 startPos = person.transform.position;
        Vector3 endPos = new Vector3(person.position.x, endYPos, person.position.z);
        while (timer <= AnimationDuration)
        {
            person.position = Vector3.Lerp(startPos, endPos, PersonPopupAnimationCurve.Evaluate(timer / AnimationDuration));
            timer += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator AnimatePersonLeaveThenDestroyIE(Person person)
    {
        yield return AnimatePersonMove(person.transform, Screen.height * SpawnYPopupAnimStartNormalized);
        Destroy(person.gameObject);
    }

    public void AnimatePersonLeaveThenDestroy(Person person)
    {
        StartCoroutine(AnimatePersonLeaveThenDestroyIE(person));
    }

    private void UpdateSpawnPositionsByScreenSize()
    {
        spawnXMin = Screen.width * SpawnXMinNormalized;
        spawnXMax = Screen.width * SpawnXMaxNormalized;
        spawnYMin = Screen.height * SpawnYMinNormalized;
        spawnYMax = Screen.height * SpawnYMaxNormalized;
        spawnYPopupAnimStart = Screen.height * SpawnYPopupAnimStartNormalized;
    }

}
