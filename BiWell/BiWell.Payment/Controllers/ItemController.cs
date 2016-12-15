using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BiWell.Payment.Controllers
{
    public class ItemController : Controller
    {
        private BiWellEntities db = new BiWellEntities();

        // GET: Items
        public ActionResult Index()
        {
            return View(db.ItemWeights.ToList());
        }

        // GET: Items/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemWeight itemWeight = db.ItemWeights.Find(id);
            if (itemWeight == null)
            {
                return HttpNotFound();
            }
            return View(itemWeight);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemId,Name,Weight")] ItemWeight itemWeight)
        {
            if (ModelState.IsValid)
            {
                db.ItemWeights.Add(itemWeight);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(itemWeight);
        }

        // GET: Items/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemWeight itemWeight = db.ItemWeights.Find(id);
            if (itemWeight == null)
            {
                return HttpNotFound();
            }
            return View(itemWeight);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemId,Name,Weight")] ItemWeight itemWeight)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemWeight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(itemWeight);
        }

        // GET: Items/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemWeight itemWeight = db.ItemWeights.Find(id);
            if (itemWeight == null)
            {
                return HttpNotFound();
            }
            return View(itemWeight);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ItemWeight itemWeight = db.ItemWeights.Find(id);
            db.ItemWeights.Remove(itemWeight);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
