using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YgoDownloader.View {
    public class FeedbackPanel : MonoBehaviour
    {
        [SerializeField] private GameObject myPanel;
        [SerializeField] private Text myText;

        public void Activate(string text, bool timed = false, float time = 2f) {
            myText.text = text;
            myPanel.SetActive(true);
            if (timed) {
                StartCoroutine(DeactivateCo(time));
            }
        }

        public void Deactivate() {
            myPanel.SetActive(false);
        }

        private IEnumerator DeactivateCo(float time) {
            yield return new WaitForSeconds(time);
            Deactivate();
        }
    }
}
