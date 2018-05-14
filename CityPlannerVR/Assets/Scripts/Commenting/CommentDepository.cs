using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores comments locally to lists (by type).
/// </summary>

public class CommentDepository : MonoBehaviour {

    public List<Comment> texts, voices, thumbs = new List<Comment>();


    void Start () {
        InitializeCommentLists();
    }


    void InitializeCommentLists()
    {
        texts = new List<Comment>();
        voices = new List<Comment>();
        thumbs = new List<Comment>();

    }


}
