using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace menu
{
    [RequireComponent(typeof(UIDocument))]
    public class BackManager : MonoBehaviour
    {
        public List<Sprite> gifFrames;
        public float frameDelay = 0.2f;

        private Image gifImage;
        private int currentFrame;

        void OnEnable()
        {
            gifImage = GetComponent<UIDocument>().rootVisualElement.Q<Image>("Image");

            Debug.Log($"LEO - git frame count {gifFrames.Count}");

            if (gifFrames.Count > 0)
            {
                StartCoroutine(PlayGif());
            }
        }

        IEnumerator PlayGif()
        {
            while (true)
            {
                Debug.Log($"LEO - switching frame {currentFrame}");

                // Set the current frame as the background image
                gifImage.style.backgroundImage = new StyleBackground(gifFrames[currentFrame]);

                // Move to next frame
                currentFrame = (currentFrame + 1) % gifFrames.Count;

                yield return new WaitForSeconds(frameDelay);
            }
        }
    }
}