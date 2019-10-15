using UnityEngine;

// Source : http://unifycommunity.com/wiki/index.php?title=Animating_Tiled_texture_-_Extended
public class AnimatedTextureUV : MonoBehaviour
{
    //vars for the whole sheet
	public int colCount =  5;
	public int rowCount =  5;

    private int skip_phase_cnt = 0;

    //vars for animation
    public int  rowNumber  =  0; //Zero Indexed
	public int colNumber = 0; //Zero Indexed
	public int totalCells = 5;

    public Camera _look_at_cam;
    public bool _bo_track_look_at_cam = false;

    private bool _playInLoop;

    private Vector2 offset;

    private GameObject _pairedObject;

    public void Init(bool playInLoop)
    {
        _playInLoop = playInLoop;
        ResetTexAnim();
        DrawPhases();

        _pairedObject = null;
    }

    public void Init(bool playInLoop, GameObject paired_object)
    {
        _playInLoop = playInLoop;
        ResetTexAnim();
        DrawPhases();

        _pairedObject = paired_object;
    }

    private void Awake()
    {
        _playInLoop = true;
        ResetTexAnim();
    }

    private void Start()
    {
        ResetTexAnim();
        DrawPhases();
    }

    void FixedUpdate()
    {
        DrawPhases();

        if(_bo_track_look_at_cam)
        {
            TrackCamera(_look_at_cam);
        }
    }

    public void ResetTexAnim()
    {
        skip_phase_cnt = 0;
        colNumber = 0;
        rowNumber = 0;
    }

    void DrawPhases()
    {
        if (0 == skip_phase_cnt)
        {
            SetSpriteAnimation(colCount, rowCount, rowNumber, colNumber, totalCells);
        }

        if (1 == skip_phase_cnt++)
        {
            skip_phase_cnt = 0;
            colNumber++;
        }
        if (colNumber >= colCount)
        {
            colNumber = 0;
            rowNumber++;
            if (rowNumber >= rowCount)
            {
                rowNumber = 0;
                if(!_playInLoop)
                {
                    ResetTexAnim();
                    this.gameObject.SetActive(false);
                    _pairedObject.SetActive(false);
                }
            }
        }
    }

    //SetSpriteAnimation
    void SetSpriteAnimation(int colCount ,int rowCount ,int rowNumber ,int colNumber,int totalCells )
	{
        // An atlas is a single texture containing several smaller textures.
        // It's used for GUI to have not power of two textures and gain space, for example.
        // Here, we have an atlas with 16 faces
        // Calculate index
        int index = 0;// (int)(Time.time * fps);
		
	    // Repeat when exhausting all cells
	    index = index % totalCells; // => 0 1 2 3 / 0 1 2 3 / 0 1 2 3 ...
	    
	    // Size of every cell
	    float sizeX = 1.0f / colCount; // We split the texture in 4 rows and 4 cols
	    float sizeY = 1.0f / rowCount;
	    Vector2 size =  new Vector2(sizeX,sizeY);
	    
	    // split into horizontal and vertical index
	    var uIndex = index % colCount;
	    var vIndex = index / colCount;
	 
	    // build offset
	    // v coordinate is the bottom of the image in opengl so we need to invert.
	    float offsetX = (uIndex + colNumber) * size.x;
	    float offsetY = (1.0f - size.y) - (vIndex + rowNumber) * size.y;
	    Vector2 offset = new Vector2(offsetX,offsetY);
	    
		// We give the change to the material
		// This has the same effect as changing the offset value of the material in the editor.
	    GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", offset); // Which face should be displayed
	    GetComponent<Renderer>().material.SetTextureScale  ("_MainTex", size); // The size of a single face
	}


   public void TrackCamera(Camera look_at_cam)
    {
        if (look_at_cam)
        {
            transform.LookAt(look_at_cam.transform.position);
            transform.rotation = transform.rotation * Quaternion.Euler(90, 0, 0);
        }
    }
}