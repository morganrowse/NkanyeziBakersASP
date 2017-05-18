using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bakery.Data;
using Bakery.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Bakery.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public OrdersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.Order.Include(o => o.User);
                return View(await applicationDbContext.ToListAsync());
            } else
            {
                var applicationDbContext = _context.Order.Include(o => o.User).Where(m => m.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(await applicationDbContext.ToListAsync());
            }

            
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.Product)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,DeliveryPrice,DeliveryDate,UserId,Status,CreatedAt,DeletedAt")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.SingleOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,DeliveryPrice,DeliveryDate,UserId,Status,CreatedAt,DeletedAt")] Order order)
        {
            if (id != order.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.User)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.SingleOrDefaultAsync(m => m.ID == id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Checkout(int id)
        {
            var order = await _context.Order.Include(o => o.OrderItems).SingleOrDefaultAsync(x => x.ID == id);

            if (order.OrderItems.Count() == 0)
            {
                TempData["danger"] = "Please add items to your cart before checkout.";
                return RedirectToAction("Details/" + order.ID.ToString(), "Orders");
            }

            if (order.Status != "Created")
            {
                TempData["danger"] = "This cart has already been checked out.";
                return RedirectToAction("Details/" + order.ID.ToString(), "Orders");
            }

            order.Status = "Quoting";

            _context.Update(order);

            await _context.SaveChangesAsync();

            TempData["success"] = "Cart successfully checked out.";
            return RedirectToAction("Details/" + order.ID.ToString(), "Orders");
        }
    }
}
