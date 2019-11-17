using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;


#if true

public class ToroidNavigator : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite touchPlaceholder;
    Texture2D touchPlaceholderTex;


    Vector2 sizeDelta;
    Vector2 canvasScale;


    Vector3 centre;
    float realWidth, realHeight;
    float canvasWidth, canvasHeight; // they both must be equal , i.e. we deal with circle

    float radius;


    void Start()
    {
        centre = this.transform.position;

        realWidth = image.sprite.rect.width;
        realHeight = image.sprite.rect.height;

        var canvas = GetComponentInParent<Canvas>();
        /*
         * https://forum.unity.com/threads/get-the-true-size-in-pixels-of-image-that-uses-canvas-scaler.487930/
        Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        Vector2 canvasScale = new Vector2(canvas.transform.localScale.x, canvas.transform.localScale.y);
        Vector2 finalScale = new Vector2(sizeDelta.x * canvasScale.x, sizeDelta.y * canvasScale.y);
        */

        canvasWidth = canvas.scaleFactor * realWidth;
        canvasHeight = canvas.scaleFactor * realHeight;

        radius = canvasWidth / 2;
        touchPlaceholderTex = textureFromSprite(touchPlaceholder);
    }

    public static Texture2D textureFromSprite(Sprite sprite)
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


                for(int i=0;i<newColors.Length;i++)
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



    /*
     * Use the center of the circle as the origin. 

dir = org - pos;
Then you would do something like arctan2(dir.y,dir.x) * RAD_2PI; 
     */

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }


    Vector3 screenPosition;
    bool boPressed;
    private void Update()
    {
        bool is_active = false;

        bool clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position);
        if (clicked)
        {
            screenPosition = camera.WorldToScreenPoint(position);

            Vector3 diff = screenPosition - centre;
            var distance = diff.magnitude;

            //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}, screen_position = {2}, distance = {3}", centre, canvasWidth, screen_position, distance)); 
            if (distance <= radius)
            {
                is_active = true;
            }
        }

        boPressed = is_active;
    }

    void OnGUI()
    {
        if (!boPressed) return;

        if (Event.current.type.Equals(EventType.Repaint))
        {
            float width = touchPlaceholderTex.width;
            float height = touchPlaceholderTex.height;

            float x = screenPosition.x - width/2;
            float y = Screen.height - screenPosition.y - height/2;

            Graphics.DrawTexture(new Rect(x, y, width, height), touchPlaceholderTex);
        }  
    }
}

#else

public class ToroidNavigator : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private Image image;

    [SerializeField]
    private Sprite touchPlaceholder;


    Vector2 sizeDelta;
    Vector2 canvasScale;


    Vector3 centre;
    float realWidth, realHeight;
    float canvasWidth, canvasHeight; // they both must be equal , i.e. we deal with circle

    float radius;

    GameObject overlayObject;
    bool isOverlayActive = false;

    SpriteRenderer spriteRend;

    void Start()
    {
        centre = this.transform.position;

        realWidth = image.sprite.rect.width;
        realHeight = image.sprite.rect.height;

        var canvas = GetComponentInParent<Canvas>();
        /*
         * https://forum.unity.com/threads/get-the-true-size-in-pixels-of-image-that-uses-canvas-scaler.487930/
        Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        Vector2 canvasScale = new Vector2(canvas.transform.localScale.x, canvas.transform.localScale.y);
        Vector2 finalScale = new Vector2(sizeDelta.x * canvasScale.x, sizeDelta.y * canvasScale.y);
        */

        canvasWidth = canvas.scaleFactor * realWidth;
        canvasHeight = canvas.scaleFactor * realHeight;

        radius = canvasWidth / 2;

        overlayObject = new GameObject("TouchOverlay");
        overlayObject.transform.parent = transform; // make 'TouchOverlay' as child of this object

        spriteRend = overlayObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        spriteRend.drawMode = SpriteDrawMode.Sliced;
        spriteRend.sprite = touchPlaceholder;
        overlayObject.SetActive(isOverlayActive);

        //color cannot be changed directly
        var color = spriteRend.color;
        color.a = 0.333f;
        spriteRend.color = color;

        spriteRend.size = new Vector2(4, 4);
        spriteRend.sortingOrder = 0;
    }

    /*
     * Use the center of the circle as the origin. 

dir = org - pos;
Then you would do something like arctan2(dir.y,dir.x) * RAD_2PI; 
     */

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }

    
    private void Update()
    {
        bool bo_active = false;

        Vector3 screen_position = new Vector3(0, 0, 0);

        bool clicked = TouchOrMouseClick.TrackMouseOrTouchCoordGUI(camera, out Vector3 position);
        if(clicked)
        {
            screen_position = camera.WorldToScreenPoint(position);

            Vector3 diff = screen_position - centre;
            var distance = diff.magnitude;

            //Debug.Log(string.Format("centre = {0}, canvasWidth = {1}, screen_position = {2}, distance = {3}", centre, canvasWidth, screen_position, distance)); 
            if (distance <= radius)
            {
                Debug.Log("inside circle");

                bo_active = true;

            }
        }

        switch(isOverlayActive)
        {
            case true:
                if(!bo_active)
                {
                    overlayObject.SetActive(false);
                }
                break;

            case false:
                if (bo_active)
                {
                    overlayObject.SetActive(true);
                }
                break;
        }

        if(bo_active)
        {
            overlayObject.transform.position =  position;
            overlayObject.transform.LookAt(camera.transform);
        }
    }
}

#endif