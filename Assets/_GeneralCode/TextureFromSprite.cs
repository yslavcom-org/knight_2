using UnityEngine;

public class TextureFromSprite : MonoBehaviour
{
    public static Texture2D Convert(Sprite sprite)
    {
        try
        {
            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.ARGB32, false);
                Color[] colors = newText.GetPixels();
                Color[] newColors = sprite.texture.GetPixels(Mathf.CeilToInt(sprite.textureRect.x),
                                                             Mathf.CeilToInt(sprite.textureRect.y),
                                                             Mathf.CeilToInt(sprite.textureRect.width),
                                                             Mathf.CeilToInt(sprite.textureRect.height));


                for (int i = 0; i < newColors.Length; i++)
                {
                    newColors[i].a = newColors[i].grayscale;
                    newColors[i].a = 0.333f;
                }

                //Debug.Log(colors.Length + "_" + newColors.Length);

                newText.SetPixels(newColors);
                newText.Apply();
                return newText;
            }
            else
                return sprite.texture;
        }
        catch
        {
            return sprite.texture;
        }
    }
}
