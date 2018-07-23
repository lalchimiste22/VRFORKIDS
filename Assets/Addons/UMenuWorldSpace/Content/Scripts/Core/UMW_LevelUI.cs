using UnityEngine;
using UnityEngine.UI;


public class UMW_LevelUI : MonoBehaviour
{
    [SerializeField]private Image ImagePreview;
    [SerializeField]private Text TitleText;
    [SerializeField]private Text DescriptionText;
    [SerializeField]private Animator Anim;

    private UMW_LevelInfo cacheInfo;
    private UMW_UIReferences UIReference;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    public void Init(UMW_LevelInfo info)
    {
        UIReference = FindObjectOfType<UMW_UIReferences>();
        cacheInfo = info;
        ImagePreview.sprite = info.PreviewImage;
        TitleText.text = info.DisplayLevelName;
        DescriptionText.text = info.Description;
    }

    /// <summary>
    /// 
    /// </summary>
    public void LoadLevel()
    {
        UIReference.SetSelectLevel(cacheInfo);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enter"></param>
    public void OnMouseEvent(bool enter)
    {
        if (Anim == null)
            return;

        Anim.SetBool("show", enter);
    }

}