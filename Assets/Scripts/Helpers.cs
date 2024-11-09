using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers
{
    public static float Map(float numberMap, float minMap, float maxMap, int minMapIndex, int maxMapIndex){
        return (numberMap - minMap) * (maxMapIndex - minMapIndex) / (maxMap - minMap) + minMapIndex;
    }


    public static Texture2D ConvertSpriteToTexture(Sprite sprite)
             {
                 try
                 {
                     if (sprite.rect.width != sprite.texture.width)
                     {
                         Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                         Color[] colors = newText.GetPixels();
                         Color[] newColors = sprite.texture.GetPixels((int)System.Math.Ceiling(sprite.textureRect.x),
                                                                      (int)System.Math.Ceiling(sprite.textureRect.y),
                                                                      (int)System.Math.Ceiling(sprite.textureRect.width),
                                                                      (int)System.Math.Ceiling(sprite.textureRect.height));
                         Debug.Log(colors.Length+"_"+ newColors.Length);
                         newText.SetPixels(newColors);
                         newText.Apply();
                         return newText;
                     }
                     else
                         return sprite.texture;
                 }catch
                 {
                     return sprite.texture;
                 }
             }
}
