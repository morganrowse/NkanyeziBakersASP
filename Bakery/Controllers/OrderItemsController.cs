using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bakery.Data;
using Bakery.Models;
using System.Security.Claims;

namespace Bakery.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderItem.Include(o => o.Order).Include(o => o.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.Order)
                .Include(o => o.Product)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID");
            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ID");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrderId,ProductId,Price,Quantity,Comments,CreatedAt,DeletedAt")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderItem);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ID", orderItem.ProductId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem.SingleOrDefaultAsync(m => m.ID == id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ID", orderItem.ProductId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrderId,ProductId,Price,Quantity,Comments,CreatedAt,DeletedAt")] OrderItem orderItem)
        {
            if (id != orderItem.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID", orderItem.OrderId);
            ViewData["ProductId"] = new SelectList(_context.Set<Product>(), "ID", "ID", orderItem.ProductId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItem
                .Include(o => o.Order)
                .Include(o => o.Product)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItem.SingleOrDefaultAsync(m => m.ID == id);
            _context.OrderItem.Remove(orderItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItem.Any(e => e.ID == id);
        }


        public async Task<IActionResult> AddToCart(int id, int quantity = 1, string comments = "N/A")
        {
            var orders = _context.Order.Where(m => m.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier) && m.Status == "Created");

            var order = new Order { };

            if (orders.Count() == 0)
            {
                 order = new Order {
                    UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Status = "Created",
                    CreatedAt = DateTime.Now
                };

                _context.Add(order);
                await _context.SaveChangesAsync();

            } else
            {
                order = orders.First();
            }

            var product = await _context.Product.FindAsync(id);

            var orderitem = new OrderItem();
            orderitem.OrderId = order.ID;
            orderitem.ProductId = product.ID;
            orderitem.Price = product.Price;
            orderitem.Quantity = quantity;
            orderitem.Comments = comments;
            orderitem.CreatedAt = DateTime.Now;

            _context.Add(orderitem);

            await _context.SaveChangesAsync();

            TempData["success"] = "Item added to cart.";
            return RedirectToAction("Details/" + order.ID.ToString(), "Orders");
        }
    }
}
