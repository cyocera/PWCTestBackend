namespace PWCExamService.Data.Entities
{
    public class CalculateArrivalSubteRequest
    {
        public string lineId { get; set; }
        public string stationFromId { get; set; }
        public string stationToId { get; set; }
    }
}
