using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[Serializable] //attributes for json
public class CommentData
{
    //CommentMetaData metaData;
    public string dataString;
    //public int type;
    public Comment.CommentType commentType;

    public string userName; //player
    public string commentedObjectName;
    public string SHPath;
    
    public string submittedShortDate;
    //public string submittedShortTime;
    //public Vector3 commentatorPosition;
    public int quickcheck;
    public string commentName;
    public float[] positions;
}

[Serializable]
public class Comment {

    public enum CommentType { None, Text, Thumb, Voice };

    //public CommentType CurrentType { get; set; }
    public CommentData data; //used for storing and loading data

    //default constructor
    public Comment()
    {
        data = null;
        OnEnable();
        //Debug.Log("Comment created");
    }

    //Not a monobehaviour!
    private void OnEnable()
    {
        //SaveData.OnLoaded += LoadData;
        //SaveData.OnBeforeSave += StoreData;
        SaveData.OnBeforeSaveComments += ApplyDataToContainer;
    }

    //Not a monobehaviour!
    private void OnDisable()
    {
        //SaveData.OnLoaded -= LoadData;
        //SaveData.OnBeforeSave -= StoreData;
        SaveData.OnBeforeSaveComments -= ApplyDataToContainer;
    }

    public void ApplyDataToContainer()
    {
        //SaveData<CommentData>.AddCommentData(data);
        //SaveData.AddData(data);
        AddCommentDataToSavedata();
        OnDisable();
    }

    public void SortAndAddToLocalList()
    {
        switch (data.commentType)
        {
            case Comment.CommentType.Text:
                //case 1:
                if (!IsCommentInList(SaveData.commentLists.textComments))
                    SaveData.commentLists.textComments.Add(this);
                break;

            case Comment.CommentType.Voice:
                //case 3:
                if (!IsCommentInList(SaveData.commentLists.voiceComments))
                    SaveData.commentLists.voiceComments.Add(this);
                break;

            case Comment.CommentType.Thumb:
                //case 2:
                //if (!IsCommentInList(SaveData.commentLists.thumbComments))
                    SaveData.commentLists.thumbComments.Add(this);
                break;

            default:
                Debug.Log("Type not set for comment (quickcheck: " + data.quickcheck + ") by user " + this.data.userName + " while being sorted!");
                break;
        }
    }

    public bool IsCommentInList(List<Comment> testList)
    {
        Comment temp;
        for (int i = 0; i < testList.Count; i++)
        {
            //Debug.Log("Checking if comment already exists...");
            temp = testList[i];
            if (IsTheSameComment(temp))
            {
                //Debug.Log("Found the same comment!");
                    return true;
            }
        }
        return false;
    }

    public bool IsTheSameComment(Comment testComment)
    {
        if (data.quickcheck == testComment.data.quickcheck &&  //comparing ints is quicker than strings
            data.userName == testComment.data.userName &&
            data.commentedObjectName == testComment.data.commentedObjectName &&
            data.dataString == testComment.data.dataString
            )
            return true;
        else
            return false;
    }

    public static int ConvertFirstCharsToInt(string str, int maxLength)
    {
        //Debug.Log("Starting conversion process...");
        string newStr = TruncateString(str, maxLength);
        //Debug.Log("String truncated...");
        byte[] bytes = Encoding.Default.GetBytes(newStr);
        //Debug.Log("Encoding done...");
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        if (bytes.Length < 4)
        {
            byte[] temp = new byte[4];
            bytes.CopyTo(temp, 0);
            bytes = temp;
        }
        //Debug.Log("Bytes are in order and proper size...");
        int magic = BitConverter.ToInt32(bytes, 0);
        //Debug.Log("Conversion done!");
        return magic;
    }

    public void GenerateQuickCheck(int subStringMaxLength)
    {
        string userName = TruncateString(data.userName, subStringMaxLength);
        string objectName = TruncateString(data.commentedObjectName, subStringMaxLength);
        string date = TruncateString(data.submittedShortDate, subStringMaxLength);
        string uberString = userName + objectName + date;
        //Debug.Log("Joining strings: " + userName + " " + objectName + " " + date);
        int magic = ConvertFirstCharsToInt(uberString, subStringMaxLength * 4);
        //Debug.Log("QuickCheck: " + magic);
        data.quickcheck = magic;
    }

    //public void ConvertToQuickCheck(int maxLength, CommentData data)
    //{
    //    string userName = TruncateString(data.userName, maxLength);
    //    string objectName = TruncateString(data.commentedObjectName, maxLength);
    //    string date = TruncateString(data.submittedShortDate, maxLength);
    //    string uberString = userName + objectName + date;
    //    Debug.Log("Joining strings: " + userName + " " + objectName + " " + date);
    //    int magic = ConvertFirstCharsToInt(uberString, maxLength * 4);
    //    Debug.Log("Adding QuickCheck to data: " + magic);
    //    data.quickcheck = magic;
    //}

    public static string TruncateString(string str, int maxLength)
    {
        int length = str.Length;
        if (length > maxLength)
            length = maxLength;
        string newStr = str.Substring(0, length);
        return newStr;
    }

    public static Comment GenerateTestComment()
    {
        Comment newComment = SaveAndLoadComments.CreateComment();
        CommentData data = new CommentData
        {
            //commentatorPosition = Vector3.zero,
            commentedObjectName = "ObjectName",
            dataString = "Data",
            quickcheck = 123,
            SHPath = "Screenshot path",
            submittedShortDate = DateTime.Now.ToShortDateString(),
            //submittedShortTime = DateTime.Now.ToShortTimeString(),
            commentType = Comment.CommentType.Text,
            //type = 1,
            userName = "Username",
            positions = new float[] { 1f, 2f, 3f },
            commentName = "Comment name"
            
        };

        newComment.CombineAndProcess(data);

        //newComment.data = data;
        //newComment.AddCommentDataToSavedata();
        return newComment;
    }

    public void AddCommentDataToSavedata()
    {
        //Debug.Log("Adding comment data to savedata list");
        SaveData.AddData(this.data);
    }

    public static Comment GenerateTextComment(string commentString, GameObject targetObject, string screenshotPath)
    {
        CommentData tempData = GenerateMetaData(CommentType.Text, targetObject, screenshotPath, commentString);
        Comment comment = SaveAndLoadComments.CreateComment();
        comment.CombineAndProcess(tempData);
		List<string> info = new List<string> ();
		info.Add ("ObjectName");
		info.Add (targetObject.name);
		GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("TextCommentCreated", info);
        return comment;
    }

    public static Comment GenerateThumbComment(string thumbData, GameObject targetObject, string screenshotPath)
    {
        CommentData tempData = GenerateMetaData(CommentType.Thumb, targetObject, screenshotPath, thumbData);
        Comment comment = SaveAndLoadComments.CreateComment();
        comment.CombineAndProcess(tempData);
		List<string> info = new List<string> ();
		info.Add ("ObjectName");
		info.Add (targetObject.name);
		GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("ThumbCommentCreated", info);
        return comment;
    }

    public static Comment GenerateVoiceComment(string audioFilePath, string targetObjectName, string screenshotPath, string commentName, float[] positions)
    {
        CommentData tempData = GenerateMetaData(CommentType.Voice, targetObjectName, screenshotPath, audioFilePath, commentName, positions);
        Comment comment = SaveAndLoadComments.CreateComment();
        comment.CombineAndProcess(tempData);
		List<string> info = new List<string> ();
		info.Add ("ObjectName");
		info.Add (targetObjectName);
		GameObject.Find ("GameManager").GetComponent<Logger> ().LogActionLine ("VoiceCommentCreated", info);
        return comment;
    }

    private void CombineAndProcess(CommentData tempData)
    {
        data = tempData;
        GenerateQuickCheck(3);
        SortAndAddToLocalList();
        AddCommentDataToSavedata();
    }

    private static CommentData GenerateMetaData(CommentType commentType, GameObject targetObject, string screenshotPath, string dataString)
    {
        CommentData tempData;
        if (targetObject)
            tempData = GenerateMetaData(commentType, targetObject.name, screenshotPath, dataString, null, null);
        else
            tempData = GenerateMetaData(commentType, "Yleinen", screenshotPath, dataString, null, null);
        return tempData;
    }

    private static CommentData GenerateMetaData(CommentType commentType, string targetObjectName, string screenshotPath, string dataString, string commentName, float[] positions)
    {
        CommentData tempData = new CommentData();
        if (string.IsNullOrEmpty(PhotonNetwork.player.NickName))
            tempData.userName = "Anonymous";
        else
            tempData.userName = PhotonNetwork.player.NickName;

        if (string.IsNullOrEmpty(screenshotPath))
            tempData.SHPath = "No screenshots";
        else
            tempData.SHPath = screenshotPath;

        tempData.commentType = commentType;
        tempData.submittedShortDate = System.DateTime.Now.ToShortDateString();
        //tempData.submittedShortTime = System.DateTime.Now.ToShortTimeString();
        if (string.IsNullOrEmpty(dataString))
            tempData.dataString = "No data!";
        else
            tempData.dataString = dataString;

        tempData.quickcheck = 0; //should be created later

        tempData.commentedObjectName = targetObjectName;
        tempData.commentName = commentName;
        tempData.positions = positions;

        return tempData;
    }

}
