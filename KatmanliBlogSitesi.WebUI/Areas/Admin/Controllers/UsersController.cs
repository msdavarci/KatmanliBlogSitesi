using KatmanliBlogSitesi.Data;
using KatmanliBlogSitesi.Entites;
using KatmanliBlogSitesi.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KatmanliBlogSitesi.Areas.Admin.Controllers
{

    [Area("Admin"), Authorize]
    public class UsersController : Controller
    {
        DatabaseContext context = new DatabaseContext();
        private readonly IService<User> _service; // veritabanındaki users tablosuna ulaşmak için DatabaseContext sınıfından context isminde bir nesne oluşturduk.

		public UsersController(IService<User> service)
		{
			_service = service;
		}

		// GET: UsersController
		public ActionResult Index()
        {
            var kullaniciListesi = context.Users.ToList(); // context nesnesi üzerinden users tablosuna ulaşıp veritabanındaki kayıtları çektik.

            return View(kullaniciListesi); // veritabanından çektiğimiz listeyi ekrana yolladık.
        }

        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        public async Task<ActionResult> CreateAsync(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.AddAsync(user);
                    await _service.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }
            
            return View(user);
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            try
            {
                context.Users.Add(user);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Hata Oluştu!");
            }
            return View(user);
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            var user = context.Users.Find(id); // Edit sayfası bizden model olarak içi dolu bir kullanıcı bekliyor.
            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(int id, User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _service.Update(user);
                    await _service.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch
                {
                    ModelState.AddModelError("", "Hata Oluştu!");
                }
            }

            return View(user);
        }

        // GET: UsersController/Delete/5
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var model = await _service.FindAsync(id);
            return View(model);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, User user)
        {
            try
            {
                _service.Delete(user);
               _service.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
