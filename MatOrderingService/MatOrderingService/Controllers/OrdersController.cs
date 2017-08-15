using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MatOrderingService.Models;
using MatOrderingService.Services.Storage;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MatOrderingService.Domain;

namespace MatOrderingService.Controllers
{
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
                return NotFound();
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
            var newOrder = _mapper.Map<Order>(order);
            newOrder.IsDeleted = false;
            newOrder.Status = OrderStatus.New;
            newOrder.CreateDate = DateTime.Now;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<OrderInfo>(newOrder));
        }

        /// <summary>
        /// Updates an Order's Details by unique id
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OrderInfo), 200)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> Put(int id, [FromBody]EditOrder value)
        {
            var order = await _context
                .Orders
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (order == null)
            {
                return NotFound();
            }
            order.OrderDetails = value.OrderDetails;
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<OrderInfo>(order));
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
            if(order == null)
            {
                return NotFound();
            }
            order.IsDeleted = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
