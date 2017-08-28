using AutoMapper;
using MatOrderingService.Domain;
using MatOrderingService.Models;
using MatOrderingService.Services.Generator;
using MatOrderingService.Services.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatOrderingService.Exceptions;

namespace MatOrderingService.Controllers
{ 
    [Route("api/[controller]")]
    public class OldOrdersController : Controller
    {
        private readonly IOrdersList _ordersList;
        private readonly IMapper _mapper;
        private readonly IGuidGenerator _generator;
        private readonly ILogger<OldOrdersController> _logger;

        public OldOrdersController(IOrdersList ordersList, IMapper mapper, IGuidGenerator generator, ILogger<OldOrdersController> logger)
        {
            _ordersList = ordersList;
            _mapper = mapper;
            _generator = generator;
            _logger = logger;
        }

        [HttpGet("codes")]
        public IActionResult Get([FromServices]IGuidGenerator methodGenerator)
        {
            return Ok($"OldOrdersController ID: {_generator.Generate()}. Method ID: {methodGenerator.Generate()}");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var orders = _ordersList.GetAllOrders()
                .Where(p => !p.IsDeleted)
                .ToArray();
            return Ok(orders.Select(order => _mapper.Map<OrderInfo>(order)).ToArray());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var order = _ordersList.GetAllOrders()
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (order == null)
            {
                throw new EntityNotFoundException();
            }
            return Ok(_mapper.Map<OrderInfo>(order));
        }

        [HttpPost]
        public IActionResult Post([FromBody]NewOrder order)
        {
            var newOrder = _mapper.Map<Order>(order);
            newOrder.IsDeleted = false;
            newOrder.Status = OrderStatus.New;
            newOrder.CreateDate = DateTime.Now;
            var allOrders = _ordersList.GetAllOrders();
            newOrder.Id = allOrders.Max(p => p.Id + 1);

            allOrders.Add(newOrder);

            return Ok(_mapper.Map<OrderInfo>(newOrder));
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]EditOrder value)
        {
            var order = _ordersList.GetAllOrders()
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (order == null)
            {
                return NotFound();
            }
            _mapper.Map<EditOrder, Order>(value, order);
            return Ok(_mapper.Map<OrderInfo>(order));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _ordersList.GetAllOrders()
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (order == null)
            {
                return NotFound();
            }
            order.IsDeleted = true;

            return NoContent();
        }
    }
}
