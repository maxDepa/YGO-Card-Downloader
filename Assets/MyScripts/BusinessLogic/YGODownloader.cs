using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using YgoDownloader.Model;
using YgoDownloader.View;

namespace YgoDownloader.BusinessLogic {
    public class YGODownloader : MonoBehaviour
    {
        private const string url = "https://db.ygoprodeck.com/api/v7/cardinfo.php";

        [SerializeField] private Dropdown myDropdown;
        [SerializeField] private FeedbackPanel myFeedbackPanel;
        private string currentArchetype => myDropdown.captionText.text;

        public List<Archetype> archetypes = new List<Archetype>();
        public Card[] cards;

        #region Path
        private string savePath => 
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)
            //+ "/YGO Card Downloader/" + currentArchetype + "/";
            Application.dataPath
            + "/Downloaded Cards/" + currentArchetype + "/";

        public string GetFullPath(string cardName, string imageID, string imageExt) {
            return savePath + imageID + "_" + cardName + imageExt;
        }
        #endregion

        void Start() {
            DeactivateFeedback();
            StartCoroutine(InitializeDropdown());
        }

        #region Initialization
        private IEnumerator InitializeDropdown() {
            UnityWebRequest uwr = UnityWebRequest.Get("https://db.ygoprodeck.com/api/v7/archetypes.php");
            yield return uwr.SendWebRequest();
            string data = uwr.downloadHandler.text;
            List<Archetype> archetypes = JsonConvert.DeserializeObject<List<Archetype>>(data);
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (Archetype archetype in archetypes) {
                options.Add(new Dropdown.OptionData(archetype.archetype_name));
            }
            myDropdown.AddOptions(options);
        }
        #endregion

        #region Download
        public void DownloadArchetype() {
            StartCoroutine(DownloadArchetype(currentArchetype));
        }

        private IEnumerator DownloadArchetype(string archetype) {
            string fullUrl = url + ArchetypeUrl(archetype);
            Debug.Log("Sending request: " + fullUrl);
            UnityWebRequest uwr = UnityWebRequest.Get(fullUrl);
            yield return uwr.SendWebRequest();
            string data = uwr.downloadHandler.text;
            uwr.Dispose();
            HandleResponse(data);
        }

        private string ArchetypeUrl(string archetype) {
            return "?archetype=" + archetype;
        }

        private void HandleResponse(string data) {
            Debug.Log(data);
            cards = JsonUtility.FromJson<CardResponse>(data).data;
            StartCoroutine(DownloadCardImages(cards));
        }

        private IEnumerator DownloadCardImages(Card[] cards) {
            if(!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            for(int i = 0; i < cards.Length; i++) {
                ActivateFeedback("Downloading cards: " + i + "/" + cards.Length.ToString());
                Card card = cards[i];
                string cardName = card.name;
                foreach (CardImage cardImage in card.card_images) {
                    string filePath = GetFullPath(cardName, cardImage.id, Path.GetExtension(cardImage.image_url));
                    if (File.Exists(filePath)) continue;
                    UnityWebRequest www = UnityWebRequest.Get(cardImage.image_url);
                    yield return www.SendWebRequest();
                    var data = www.downloadHandler.data;
                    File.WriteAllBytes(filePath, data);
                    while (!File.Exists(filePath)) {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            ActivateFeedback("Cards saved in " + savePath, true);
        }
        #endregion

        #region View
        private void ActivateFeedback(string feedback, bool timed = false, float time = 2f) {
            myFeedbackPanel.Activate(feedback, timed, time);

        }

        private void DeactivateFeedback() {
            myFeedbackPanel.Deactivate();
        }
        #endregion
    }
}
