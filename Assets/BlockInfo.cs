using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Test/Block information")]
public class BlockInfo : ScriptableObject
{
    public BlockEnum BlockType;
    public Vector2 PixelsOffsetX;
    public Vector2 PixelsOffsetY;
    public GameObject dropItem;

    public AudioClip StepSound;
    public float TimeToBreak = 1f;
}
