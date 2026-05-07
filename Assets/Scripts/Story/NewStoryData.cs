using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[Serializable]
public class NewStoryData
{
    public int Story_ID;
    public string Speaker;
    public string Dialogue;
    public bool Is_Image;
    public bool Audio;
    public bool EndPoint;
    public string TargetImageName;
    public string TargetAudio;
    public string TargetMusic;
}
