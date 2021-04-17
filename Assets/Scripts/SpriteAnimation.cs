using UnityEngine;

[CreateAssetMenu(fileName = "Animation", menuName = "Sprite Animation")]
public class SpriteAnimation : ScriptableObject
{
    public Sprite[] frames;
    public float fps = 12f;
}
