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

	//Maybe this should be a private var
	private Vector2 offset;



    private void Awake()
    {
        skip_phase_cnt = 0;
        DrawPhases();
    }

    void Update()
    {
        DrawPhases();
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