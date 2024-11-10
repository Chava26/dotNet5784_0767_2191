

namespace DO;



    public record Volunteer
    {
        public int Id { get; init; }
        public string FullName { get; init; }
        public string Cellphone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? FullAddress { get; set; }
        public double? Longitude { get; init; }
        public double? Latitude { get; init; }
        public double? MaximumDistance { get; init; }

        public Volunteer() { }

   
    }