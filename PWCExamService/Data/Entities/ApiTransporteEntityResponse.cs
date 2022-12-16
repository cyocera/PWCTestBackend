namespace PWCExamService.Data.Entities
{
    public class ApiTransporteEntityResponse<H, T>
    {
        public H? Header { get; set; }
        public T? Entity { get; set; }
    }

    public class HeaderGorecastGTFS
    {
        public int timestamp { get; set; }
    }

    public class GorecastGTFSResponse
    {
        public GorecastGTFSResponse()
        {
            Linea = new Line();
        }
        public string ID { get; set; }
        public Line Linea { get; set; }
    }

    public class Line
    {
        public Line()
        {
            Estaciones = new List<Station>();
        }
        public string Trip_Id { get; set; }
        public string Route_Id { get; set; }
        public int Direction_ID { get; set; }
        public string start_time { get; set; }
        public string start_date { get; set; }
        public List<Station> Estaciones { get; set; }

    }

    public class Station
    {
        public Station()
        {
            arrival = new Arrival();
            departure = new Departure();
        }
        public string stop_id { get; set; }
        public string stop_name { get; set; }
        public Arrival arrival { get; set; }
        public Departure departure { get; set; }
    }

    public class Arrival : ApiTransporteCommon
    {

    }
    public class Departure : ApiTransporteCommon
    {

    }

    public class ApiTransporteCommon
    {
        public int time { get; set; }
        public int delay { get; set; }
    }

    public class ServicesAlertHeader 
    {
        public string gtfs_realtime_version { get; set; }
        public int incrementality { get; set; }
        public int timestamp { get; set; }
    }

    public class ServicesAlertResponse 
    { 

    }
}
