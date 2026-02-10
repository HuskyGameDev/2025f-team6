using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurboPursuit.UI 
{

    [CreateAssetMenu(fileName = "New Carousel Entry", menuName = "UI/Carousel Entry", order = 0)]

    public class CarouselEntry : ScriptableObject
    {
        [field:SerializeField] public Sprite EntryGraphic {get; private set;}
        [field:SerializeField] public string Headline {get; private set;}
    
        [Header("Interaction")]
        [SerializeField] private string levelNameToLoad;

        public void Interact()
        {
            SceneManager.LoadScene(levelNameToLoad);
        }


    }

}