namespace YgoDownloader.Model {
    [System.Serializable]
    public class Card
    {
        public string id;
        public string name;
        public string type;
        public string desc;
        public int atk;
        public int def;
        public int level;
        public string race;
        public string attribute;
        public string archetype;
        public CardSet[] card_sets;
        public CardImage[] card_images;
        public CardPrice[] card_prices;
    }
}
