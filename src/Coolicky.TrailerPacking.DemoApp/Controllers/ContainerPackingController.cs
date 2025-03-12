using Coolicky.TrailerPacking.DemoApp.Models;
using Coolicky.TrailerPacking.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Coolicky.TrailerPacking.DemoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerPackingController : ControllerBase
    {
        // POST api/values
        [HttpPost]
        public ActionResult<List<ContainerPackingResult>> Post([FromBody]ContainerPackingRequest request)
        {
            var items = new List<Item>();
            foreach (var requestItem in request.ItemsToPack)
            {
                items.Add(new Item(Guid.NewGuid(), requestItem.Dim1, requestItem.Dim2, requestItem.Dim3, requestItem.Quantity, requestItem.IsStackable, requestItem.Weight, requestItem.MaxStack));
            }
            return new List<ContainerPackingResult>()
            {
                new ContainerPackingResult()
                {
                    ContainerID = request.Containers.First().ID,
                    AlgorithmPackingResults = [GeneticPackingSolver.Hello(request.Containers.FirstOrDefault(), items)]
                }
            };
            
            return PackingService.Pack(request.Containers, items, request.AlgorithmTypeIDs);
        }
    }
}