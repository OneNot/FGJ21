using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemPrefab
{
    public string Name;
    public GameObject Prefab;
    public List<ItemPrefabVoice> Voices;
}

[Serializable]
public class ItemPrefabVoice
{
    public string VoiceName;
    public AudioClip Clip;
}