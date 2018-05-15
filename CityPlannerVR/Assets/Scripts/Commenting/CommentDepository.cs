using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores comments locally to lists (by type).
/// For debugging only! Use SaveData singleton for easy access to similar lists
/// </summary>

public class CommentDepository : MonoBehaviour {

    public List<Comment> texts, voices, thumbs = new List<Comment>();


    void Start () {
        InitializeCommentLists();
        InvokeRepeating("UpdateLists", 1, 5);
    }


    void InitializeCommentLists()
    {
        texts = new List<Comment>();
        voices = new List<Comment>();
        thumbs = new List<Comment>();
    }

    private void UpdateLists()
    {
        texts = SaveData.commentLists.textComments; //copy or reference?
        voices = SaveData.commentLists.voiceComments;
        thumbs = SaveData.commentLists.thumbComments;
    }
}
