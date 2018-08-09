using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPauseButtons : MonoBehaviour {

    PlayComment playComment;

    private void Start()
    {
        playComment = GetComponentInParent<PlayComment>();
    }

    public void PlayComment()
    {
        playComment.PlayCommentInPosition();
    }

    public void PauseComment()
    {
        playComment.PauseComment();
    }
}
