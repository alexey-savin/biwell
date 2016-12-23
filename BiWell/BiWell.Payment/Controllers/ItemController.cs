using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        public ActionResult ExportExcel()
        {
            ExportExcelAsHtml();

            return RedirectToAction("Index");
        }

        private void ExportExcelAsHtml()
        {
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=InventoryItems.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    var gv = new GridView();
                    gv.DataSource = db.ItemWeights.ToList();
                    gv.DataBind();
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                }
            }
            Response.End();    
        }

        private void ExportExcelAsTsv()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=DemoExcel.xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = Encoding.UTF8.WebName;
            Response.ContentEncoding = Encoding.UTF8;

            foreach (var item in db.ItemWeights)
            {
                Response.Output.Write(item.ItemId);
                Response.Output.Write("\t");
                Response.Output.Write(item.Name);
                Response.Output.Write("\t");
                Response.Output.WriteLine();
            }

            Response.Flush();
            Response.End();
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
