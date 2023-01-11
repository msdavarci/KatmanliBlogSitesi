using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KatmanliBlogSitesi.Data;
using KatmanliBlogSitesi.Entites;
using KatmanliBlogSitesi.Service.Abstract;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace KatmanliBlogSitesi.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize]
	public class PostsController : Controller
	{
		private readonly DatabaseContext _context;
		private readonly IService<Post> _service;
		private readonly IService<Category> _serviceCategory;

		public PostsController(DatabaseContext context, IService<Post> service, IService<Category> serviceCategory)
		{
			_service = service;
			_serviceCategory = serviceCategory;
			_context = context;
		}

		// GET: Admin/Posts
		public async Task<IActionResult> Index()
		{
			var databaseContext = _context.Posts.Include(p => p.Category);
			return View(await databaseContext.ToListAsync());
		}

		// GET: Admin/Posts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.Posts == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.Include(p => p.Category)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// GET: Admin/Posts/Create
		public async Task<IActionResult> CreateAsync()
		{
			ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name");
			//ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
			return View();
		}

		// POST: Admin/Posts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateAsync(Post post, IFormFile? Image)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (Image is not null)
					{
						string klasor = Directory.GetCurrentDirectory() + "/wwwroot/Img/" + Image.FileName;
						using var stream = new FileStream(klasor, FileMode.Create); //Idisposable - GC
						await Image.CopyToAsync(stream);
						post.Image = Image.FileName;

					}
					await _service.AddAsync(post);
					await _service.SaveChangesAsync();
					//_context.Add(post);
					//await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}

			}
			ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name");
			return View(post);
		}

		// GET: Admin/Posts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.Posts == null)
			{
				return NotFound();
			}

			var post = await _context.Posts.FindAsync(id);
			if (post == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
			return View(post);
		}

		// POST: Admin/Posts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Post post, IFormFile? Image)
		{
			if (id != post.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{

					if (Image is not null)
					{
						string klasor = Directory.GetCurrentDirectory() + "/wwwroot/Img/" + Image.FileName;
						using var stream = new FileStream(klasor, FileMode.Create); //Idisposable - GC
						Image.CopyTo(stream);
						post.Image = Image.FileName;

					}
					_context.Update(post);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PostExists(post.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
			return View(post);
		}

		// GET: Admin/Posts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.Posts == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.Include(p => p.Category)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// POST: Admin/Posts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.Posts == null)
			{
				return Problem("Entity set 'DatabaseContext.Posts'  is null.");
			}
			var post = await _context.Posts.FindAsync(id);
			if (post != null)
			{
				_context.Posts.Remove(post);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool PostExists(int id)
		{
			return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
