using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores comments locally to lists (by type). For debugging only!
/// Use SaveData singleton for easy access to similar lists (like in this script)
/// </summary>

public class CommentDepository : MonoBehaviour {

    [SerializeField]
    private List<Comment> texts, voices, thumbs = new List<Comment>();


    void Start () {
        //InitializeCommentLists();
        InvokeRepeating("UpdateLists", 1, 3);
    }


    void InitializeCommentLists()
    {
        texts = new List<Comment>();
        voices = new List<Comment>();
        thumbs = new List<Comment>();
    }

    private void UpdateLists()
    {
        texts = SaveData.commentLists.textComments; //copy content or reference?
        voices = SaveData.commentLists.voiceComments;
        thumbs = SaveData.commentLists.thumbComments;
    }
}
