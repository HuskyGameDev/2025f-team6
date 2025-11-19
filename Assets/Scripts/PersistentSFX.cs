//using UnityEngine;

//public class PersistentSFX : MonoBehaviour
//{
//    private const string TagName = "UIAudioSource";

//    void Awake()
//    {
//        // If another instance already exists, destroy this one
//        var existing = GameObject.FindWithTag(TagName);

//        if (existing != null && existing != gameObject)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Mark this one as the persistent SFX object
//        gameObject.tag = TagName;
//        DontDestroyOnLoad(gameObject);
//    }
//}
