/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;


public class TexturePainter : MonoBehaviour {
	public GameObject brushContainer; //The cursor that overlaps the model and our container for the brushes painted
	public Camera sceneCamera,canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
   
	public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
	public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)

    [SerializeField]
	private float brushSize=0.01f; //The size of our brush
    [SerializeField]
    private float penDistance = 10f;

    public Color brushColor; //The selected color

	bool saving=false; //Flag to check if we are saving the texture

    public BrushPooler brushPool;

    public Transform penTip;

    private void Start()
    {
        brushPool = brushContainer.GetComponent<BrushPooler>();
        brushColor = Color.black;    

        if (brushPool == null)
        {
            Debug.LogError("TexturePainter::Start: Brush Pool is null!");
        }
    }

    void Update () {

        //DoAction();
	}

    // Each individual pen could call this function if they hit the correct object (exam paper)
    // Pen would give its own "pentip" as an argument

    void DoAction()
    {
        if (saving)
        {
            return;
        }

        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPositionObject(ref uvWorldPosition, penTip))
        {
            GameObject brushObj = brushPool.GetPooledObject();

            if (brushObj != null)
            {
                brushObj.GetComponent<SpriteRenderer>().color = brushColor;

                brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)

                brushObj.transform.localScale = Vector3.one * brushSize;//The size of the brush

                brushObj.SetActive(true);
            }
            else
            { 
                saving = true;
                SaveTexture();
                brushPool.ResetPool();
            }
        }    
    }

    bool HitTestUVPositionObject(ref Vector3 uvWorldPosition, Transform penTip)
    {
        RaycastHit hit;
        
        Ray rayBehindPlayer = new Ray(penTip.position, penTip.up);

        if (Physics.Raycast(rayBehindPlayer, out hit, penDistance))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null|| meshCollider.sharedMesh == null)
                return false;
            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;
            uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;
            uvWorldPosition.z = 0.0f;

            return true;
        }
        else
        {
            return false;
        }
    }
    //Sets the base material with a our canvas texture, then removes all our brushes
    void SaveTexture(){		
		System.DateTime date = System.DateTime.Now;
		RenderTexture.active = canvasTexture;
		Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);		
		tex.ReadPixels (new Rect (0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
		tex.Apply ();
		RenderTexture.active = null;
		baseMaterial.mainTexture =tex;  
        saving = false;
	}

    public void Draw(Transform tip, RaycastHit hit)
    {
        Debug.Log("TexturePainter::Draw: Drawing");
        if (saving)
        {
            return;
        }

        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPositionObject(ref uvWorldPosition, tip))
        {
            GameObject brushObj = brushPool.GetPooledObject();

            if (brushObj != null)
            {
                brushObj.GetComponent<SpriteRenderer>().color = brushColor;

                brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)

                brushObj.transform.localScale = Vector3.one * brushSize;//The size of the brush

                brushObj.SetActive(true);
            }
            else
            {
                saving = true;
                SaveTexture();
                brushPool.ResetPool();
            }
        }
    }
}
