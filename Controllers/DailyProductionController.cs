using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DailyProduction.Models;
using Azure.Data.Tables;
using Azure;

namespace IbasAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DailyProductionController : ControllerBase
    {
        static string connectionString = "DefaultEndpointsProtocol=https;AccountName=ibasproduction2020;AccountKey=DpjQAdhPUMDzQ+uFL1iAY7RifIhs2HV5VQTBJL1L77ACv1RHyKUTZmdatXD4E9XwGi+xBWURATG0+ASt53BHzA==;EndpointSuffix=core.windows.net";
        static string tableName = "IBASProduction2020";

        public TableClient tableClient = new TableClient(connectionString, tableName);

        private List<DailyProductionDTO> _productionRepo;
        private readonly ILogger<DailyProductionController> _logger;

        public DailyProductionController(ILogger<DailyProductionController> logger)
        {
            _logger = logger;

            Pageable<TableEntity> entities = tableClient.Query<TableEntity>();

            _productionRepo = new List<DailyProductionDTO>();

            foreach (TableEntity entity in entities)
            {
                _productionRepo.Add(new DailyProductionDTO
                {
                    Date = DateTime.Parse(entity.GetString("RowKey")),
                    Model = (BikeModel)Int32.Parse(entity.GetString("PartitionKey")),
                    ItemsProduced = Int32.Parse(entity.GetString("itemsProduced"))
                });
            }
        }

        [HttpGet]
        public IEnumerable<DailyProductionDTO> Get()
        {
            return _productionRepo;
        }
    }
}
