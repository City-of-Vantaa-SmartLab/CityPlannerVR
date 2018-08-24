using UnityEngine;

/// <summary>
/// Responsible for checking if there are objects nearby that can be drawn on
/// </summary>
public class PencilHit : MonoBehaviour {

    [SerializeField]
    private float penDistance = 0.05f;

    private TexturePainter texPainter;

	void Start () {
        texPainter = GameObject.FindGameObjectWithTag("TexturePainter").GetComponent<TexturePainter>();
        if (texPainter == null)
        {
            Debug.LogError("PencilHit::Start: Can't find texturepainter");
        }
		Debug.LogWarning ("TexturePainter found!");
	}

	void Update () {
        RaycastHit hit;

        Ray rayBehindPlayer = new Ray(transform.position, transform.up);

        // If hit comething
        if (Physics.Raycast(rayBehindPlayer, out hit, penDistance))
        {
            // Hit something we can draw on
            if(hit.collider.gameObject.tag == "Drawable")
            {
				Debug.LogWarning ("Ray hit board");
                // Call texturepainter to draw
                texPainter.Draw(transform, hit);
            }
        }
        
    }
}
