

namespace DO;

    public record Call
    {
        public int Id { get; init; }
        public string? Description { get; init; }
        public string FullAddress { get; set; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public DateTime OpeningTime { get; init; }
        public DateTime? MaximumTimeToFinish { get; init; }
        public Call() {
         Id = DalList.Config.NextCallId;
         OpeningTime = DalList.Config.Clock;

        }
    }
