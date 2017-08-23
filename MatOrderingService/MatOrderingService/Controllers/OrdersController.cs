using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MatOrderingService.Models;
using MatOrderingService.Services.Storage;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Dapper;
using MatOrderingService.Domain;
using Microsoft.AspNetCore.Authorization;
using MatOrderingService.Exceptions;

namespace MatOrderingService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly OrdersDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(OrdersDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves an array of all orders
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(OrderInfo[]), 200)]
        public async Task<IActionResult> Get()
        {
            var orders = await _context
                .Orders
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .ToArrayAsync();
            return Ok(orders.Select(order => _mapper.Map<OrderInfo>(order)).ToArray());
        }

        /// <summary>
        /// Retrieves a specific Order by unique id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderInfo), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _context
                .Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (order == null)
            {
                throw new EntityNotFoundException();
            }
            return Ok(_mapper.Map<OrderInfo>(order));
        }

        /// <summary>
        /// Creates a new Order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderInfo), 200)]
        public async Task<IActionResult> Post([FromBody]NewOrder order)
        {
            if (ModelState.IsValid)
            {
                var newOrder = _mapper.Map<Order>(order);
                newOrder.IsDeleted = false;
                newOrder.Status = OrderStatus.New;
                newOrder.CreateDate = DateTime.Now;

                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<OrderInfo>(newOrder));
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Updates an Order's Details by unique id
        /// </summary>
        /// <param name="id">ID of the updated order</param>
        /// <param name="value">New properties of the order</param>
        /// <response code="200">Order created</response>
        /// <response code="404">Order is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderInfo), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> Put(int id, [FromBody]EditOrder value)
        {
            if (ModelState.IsValid)
            {
                var order = await _context
                    .Orders
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
                if (order == null)
                {
                    throw new EntityNotFoundException();
                }
                order.OrderDetails = value.OrderDetails;
                await _context.SaveChangesAsync();
                return Ok(_mapper.Map<OrderInfo>(order));
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Deletes an Order by unique id
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context
                .Orders
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (order == null)
            {
                throw new EntityNotFoundException();
            }
            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Retrieves Order's statistic
        /// </summary>
        [HttpGet("statistic")]
        [ProducesResponseType(typeof(OrdersStatisticItem[]), 200)]
        public async Task<IActionResult> GetStatistic()
        {
            var ordersStatisticItems = await _context
                .Orders
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .GroupBy(g => g.CreatorId)
                .Select(p => new OrdersStatisticItem { CreatorId = p.Key, NumberOfOrders = p.Count() })
                .ToArrayAsync();
            return Ok(ordersStatisticItems);
        }

        /// <summary>
        /// Retrieves Order's statistic with dapper
        /// </summary>
        [HttpGet("statistic/dapper")]
        [ProducesResponseType(typeof(OrdersStatisticItem[]), 200)]
        public async Task<IActionResult> GetStatisticDapper()
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                var ordersStatisticItems = await connection.QueryAsync<OrdersStatisticItem>(@"
                    SELECT CreatorId, COUNT(*) AS NumberOfOrders
                    FROM Orders
                    GROUP BY CreatorId;
                ");
                return Ok(ordersStatisticItems);
            }
        }
    }
}
