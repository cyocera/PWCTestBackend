using Newtonsoft.Json;
using PWCExamService.Data;
using PWCExamService.Data.Entities;
using PWCExamService.Data.UnitOfWork;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace PWCExamService.Managers
{
    public interface ISubtesManager 
    {
        Task<BaseResponseEntity<List<LineEntityResponse>>> GetLines();
        Task<BaseResponseEntity<List<StationEntityResponse>>> GetStations(string lineId);
        Task<BaseResponseEntity<SingleResponseEntity<int>>> GetTimeOfArrival(string lineId, string stationFromId, string stationToId);
        Task<BaseResponseEntity<List<LineStatus>>> GetLineStatus();
        Task<BaseResponseEntity<List<LineStatus>>> GetLineStatusByDate(DateTime dateFrom, DateTime dateTo);

    }
    public class SubtesManager : ISubtesManager
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork UoW;
        public SubtesManager(IHttpClientFactory clientFactory, IConfiguration configuration, IUnitOfWork UoW)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.UoW = UoW;
        }

        public async Task<BaseResponseEntity<List<LineEntityResponse>>> GetLines() 
        {
            var result = new BaseResponseEntity<List<LineEntityResponse>>();
            try
            {
                var forecastGTFSResponse = await CallForecastGTFS();
                result.Data = forecastGTFSResponse.Entity.Select(x => new LineEntityResponse { lineId = x.ID, lineName = string.Format("{0} {1} ({2} - {3}))",
                                                                          x.Linea.Route_Id.Substring(0,5),
                                                                          x.Linea.Route_Id.Substring(5),
                                                                          x.Linea.Estaciones.FirstOrDefault().stop_name,
                                                                          x.Linea.Estaciones.LastOrDefault().stop_name)}).ToList();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<BaseResponseEntity<List<StationEntityResponse>>> GetStations(string lineId) 
        {
            var result = new BaseResponseEntity<List<StationEntityResponse>>();
            try
            {
                var forecastGTFSResponse = await CallForecastGTFS();
                result.Data = forecastGTFSResponse.Entity.FirstOrDefault(x => x.ID.Equals(lineId)).Linea.Estaciones.Select(y => new StationEntityResponse { stationId = y.stop_id, stationName = y.stop_name }).ToList();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<BaseResponseEntity<SingleResponseEntity<int>>> GetTimeOfArrival(string lineId, string stationFromId, string stationToId) 
        {
            var result = new BaseResponseEntity<SingleResponseEntity<int>>();
            try
            {
                var forecastGTFSResponse = await CallForecastGTFS();
                result.Data = new SingleResponseEntity<int> { result = Convert.ToInt32(forecastGTFSResponse.Entity.FirstOrDefault(x => x.ID.Equals(lineId)).Linea.Estaciones.FirstOrDefault(x => x.stop_id.Equals(stationFromId)).arrival.delay / 60) };
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<BaseResponseEntity<List<LineStatus>>> GetLineStatus() 
        {
            var result = new BaseResponseEntity<List<LineStatus>>();
            try
            {
                var servicesAlertResponse = await CallServicesAlerts();
                var listTest = new List<LineStatus>();
                listTest.Add(new LineStatus { linename = "Line A", status = "Funcionando correctamente", lastUpdate = DateTime.Now });
                listTest.Add(new LineStatus { linename = "Line B", status = "Funcionando correctamente", lastUpdate = DateTime.Now });
                listTest.Add(new LineStatus { linename = "Line C", status = "Funcionando correctamente", lastUpdate = DateTime.Now });
                result.Data = servicesAlertResponse.Entity.Count != 0 ? servicesAlertResponse.Entity.Select(x => new LineStatus { linename = "test", status = "test", lastUpdate = DateTime.Now }).ToList() : listTest;

                await UpdateHistoricalIncident(result.Data);
            
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<BaseResponseEntity<List<LineStatus>>> GetLineStatusByDate(DateTime dateFrom, DateTime dateTo) 
        {
            var result = new BaseResponseEntity<List<LineStatus>>();
            try
            {
                result.Data = UoW.lineIncidentHistorical.Find(x => x.date >= dateFrom && x.date <= dateTo).Result
                                                        .Select(y => new LineStatus { linename = y.lineName, status = y.status, lastUpdate = y.date }).ToList();
            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }
            return result;
        }

        #region PrivateMethod
        private async Task<ApiTransporteEntityResponse<HeaderGorecastGTFS, List<GorecastGTFSResponse>>> CallForecastGTFS() 
        {
            var result = new ApiTransporteEntityResponse<HeaderGorecastGTFS, List<GorecastGTFSResponse>>();
            using (var serv = clientFactory.CreateClient("apiTransporte"))
            {
                var response = await serv.GetAsync(string.Format("subtes/forecastGTFS?client_id={0}&client_secret={1}", configuration["apitransporte:clientId"], configuration["apitransporte:clientSecret"]));
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<ApiTransporteEntityResponse<HeaderGorecastGTFS, List<GorecastGTFSResponse>>>(data);
                }
            }
            return result;
        }
        private async Task<ApiTransporteEntityResponse<ServicesAlertHeader, List<ServicesAlertResponse>>> CallServicesAlerts() 
        {
            var result = new ApiTransporteEntityResponse<ServicesAlertHeader, List<ServicesAlertResponse>>();
            using (var serv = clientFactory.CreateClient("apiTransporte"))
            {
                var response = await serv.GetAsync(string.Format("subtes/serviceAlerts?json=1&client_id={0}&client_secret={1}", configuration["apitransporte:clientId"], configuration["apitransporte:clientSecret"]));
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    result = JsonConvert.DeserializeObject<ApiTransporteEntityResponse<ServicesAlertHeader, List<ServicesAlertResponse>>>(data);
                }
            }
            return result;
        }
        private async Task UpdateHistoricalIncident(List<LineStatus> data)
        {
            var insert = new List<LineIncidentHistorical>();
            insert.AddRange(UoW.lineIncidentHistorical.GetAll().Result.Where(x => x.date.ToShortDateString() == DateTime.Now.ToShortDateString()));

            if (insert.ToList().Count != 0) 
            {
                //simulación del servicio de status
                insert.Select(y => { y.date = DateTime.Now; return y;}).ToList();
                await UoW.lineIncidentHistorical.UpdateRange(insert);
            }
            else
            {
                data.ForEach(x => insert.ToList().Add(new LineIncidentHistorical {  }));

                foreach (var item in data)
                {
                    var entiy = new LineIncidentHistorical { lineName = item.linename, status = item.status, date = item.lastUpdate };
                    insert.Add(entiy);
                }

                await UoW.lineIncidentHistorical.InsertRange(insert);
            }
            UoW.Save();
        }
        #endregion
    }
}
