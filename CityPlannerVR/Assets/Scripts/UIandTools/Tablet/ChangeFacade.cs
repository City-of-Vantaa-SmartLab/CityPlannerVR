using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class ChangeFacade : MonoBehaviour {

    MeshRenderer meshRenderer;

    /// <summary>
    /// List of all the possible facades for an object
    /// (if wanted to make to work with multiple buildings, remove comments marked with xxx (some other adjustments might have to be also done)) 
    /// </summary>
    public /*static [xxx]*/ GameObject[] facades;
    public static int facadeIndex = 0;
    public static bool canChangeFacade = true;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

	public void NextFacadeOverNetwork()
	{
		int newIndex = ScrollFacades ();
		this.gameObject.GetComponent<PhotonView> ().RPC ("CheckIndex", PhotonTargets.AllBuffered, newIndex);
		this.gameObject.GetComponent<PhotonView> ().RPC ("FacadeChange", PhotonTargets.AllBuffered);
	}

	[PunRPC]
	public void CheckIndex(int index)
	{
		if (facadeIndex != index) {
			facadeIndex = index;
		}
	}
    /// <summary>
    /// Used to scroll facades
    /// </summary>
    public int ScrollFacades()
    {
        Debug.Log("Scroll Facades");
        if (canChangeFacade)
        {
            if (facades.Length > 0)
            {
                if (facadeIndex == facades.Length - 1)
                {
                    facadeIndex = 0;
                }
                else
                {
                    ++facadeIndex;
                }
            }
        }

		return facadeIndex;
        //FacadeChange(); [xxx]
    }

    /// <summary>
    /// Change the facade of the building selected
    /// </summary>
	[PunRPC]
    public void FacadeChange()
    {
        Debug.Log("Change facades");
        if (canChangeFacade) {
            for (int i = 0; i < facades.Length; i++)
            {
                if (i == facadeIndex)
                {
                    facades[i].SetActive(true);
                }
                else
                {
                    facades[i].SetActive(false);
                }
            }
        }
    }
}
