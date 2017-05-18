using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bakery.Data;
using Bakery.Models;

namespace Bakery.Controllers
{
    public class OrderCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderCommentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: OrderComments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderComment.Include(o => o.Order);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrderComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderComment = await _context.OrderComment
                .Include(o => o.Order)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (orderComment == null)
            {
                return NotFound();
            }

            return View(orderComment);
        }

        // GET: OrderComments/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID");
            return View();
        }

        // POST: OrderComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,OrderId,Comment,CreatedAt,DeletedAt")] OrderComment orderComment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderComment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID", orderComment.OrderId);
            return View(orderComment);
        }

        // GET: OrderComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderComment = await _context.OrderComment.SingleOrDefaultAsync(m => m.ID == id);
            if (orderComment == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID", orderComment.OrderId);
            return View(orderComment);
        }

        // POST: OrderComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,OrderId,Comment,CreatedAt,DeletedAt")] OrderComment orderComment)
        {
            if (id != orderComment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderCommentExists(orderComment.ID))
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
            ViewData["OrderId"] = new SelectList(_context.Order, "ID", "ID", orderComment.OrderId);
            return View(orderComment);
        }

        // GET: OrderComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderComment = await _context.OrderComment
                .Include(o => o.Order)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (orderComment == null)
            {
                return NotFound();
            }

            return View(orderComment);
        }

        // POST: OrderComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderComment = await _context.OrderComment.SingleOrDefaultAsync(m => m.ID == id);
            _context.OrderComment.Remove(orderComment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OrderCommentExists(int id)
        {
            return _context.OrderComment.Any(e => e.ID == id);
        }
    }
}
