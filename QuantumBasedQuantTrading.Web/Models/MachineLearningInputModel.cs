namespace QuantumBasedQuantTrading.Web.Models
{
    public class MachineLearningInputModel
    {
        public string Symbol { get; set; }
        public float TitleSentiment { get; set; }
        public float ContSentiment { get; set; }
        public float DescSentiment { get; set; }
        public float OpenPrice { get; set; }
        public float CurrentHighPrice { get; set; }
        public float CurrentLowPrice { get; set; }
        public float CurrentVolume { get; set; }
    }

}
