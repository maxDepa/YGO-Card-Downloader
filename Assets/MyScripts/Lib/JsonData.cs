using UnityEngine;

namespace YgoDownloader.Depa.Dto
{
    [System.Serializable]
    public abstract class JsonData
    {
        //Utilities
        public string GetJSON() {
            Debug.Log("Generated JSON: " + JsonUtility.ToJson(this));
            return JsonUtility.ToJson(this);
        }

        public byte[] GetJSONBytes() {
            return System.Text.Encoding.UTF8.GetBytes(GetJSON());
        }

        //Save
        public void SaveJson(string path) {
            System.IO.File.WriteAllText(path, GetJSON());
        }

        public void SaveJsonBytes(string path) {
            System.IO.File.WriteAllBytes(path, GetJSONBytes());
        }
    }
}
