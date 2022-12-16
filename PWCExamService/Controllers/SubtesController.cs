using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PWCExamService.Data;
using PWCExamService.Data.Entities;
using PWCExamService.Managers;

namespace PWCExamService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubtesController : ControllerBase
    {
        private readonly ISubtesManager services;
        public SubtesController(ISubtesManager services)
        {
            this.services = services;
        }

        [HttpGet, Route("getLines")]
        public async Task<ActionResult> GetLines()
        {
            var response = await services.GetLines();
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpGet, Route("getStation/{lineId}")]
        public async Task<ActionResult> GetStation(string lineId)
        {
            var response = await services.GetStations(lineId);
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }
        [HttpPost, Route("calculateTimeOfArrival")]
        public async Task<ActionResult> CalculateTimeOfArrival(CalculateArrivalSubteRequest request) 
        {
            var response = await services.GetTimeOfArrival(request.lineId, request.stationFromId, request.stationToId);
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpGet, Route("getLineStatus")]
        public async Task<ActionResult> GetLineStatus() 
        {
            var response = await services.GetLineStatus();
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        [HttpGet, Route("getLineStatusByDate/{dateFrom}/{dateTo}")]
        public async Task<ActionResult> GetLineStatusByDate(string dateFrom, string dateTo)
        {
            var response = await services.GetLineStatusByDate(Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateTo));
            return response.Code != 500 ? response.Code == 200 ? Ok(response) : BadRequest(response) : StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}
