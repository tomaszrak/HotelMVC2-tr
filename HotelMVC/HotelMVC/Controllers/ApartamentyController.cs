using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelMVC.Models;
using Microsoft.AspNet.Identity;

namespace HotelMVC.Controllers
{
    public class ApartamentyController : Controller
    {
        private EntityContext db = new EntityContext();

        // GET: Apartamenty
        public ActionResult Index()
        {
            return View(db.Apartamenty.ToList());
        }

        public ActionResult MojeApartamenty()
        {
            List<ApartamentyDisplayViewModel> ap =
                db.Apartamenty
                .Include("Wizyty")
                .Include("UdogodnieniaApartamenty.Udogodnienie").ToList()
                .Where(a => a.IdWlasciciel == User.Identity.GetUserId())
                .Select(a => new ApartamentyDisplayViewModel(a)).ToList();

            return View(ap);
        }

        // GET: Apartamenty/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartamenty apartamenty = db.Apartamenty.Find(id);
            if (apartamenty == null)
            {
                return HttpNotFound();
            }
            return View(apartamenty);
        }

        // GET: Apartamenty/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Apartamenty/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdApartamentu,Nazwa,Cena,IloscOsob,Opis,Ulica,Miasto,KodPocztowy,IdWlasciciel")] Apartamenty apartamenty)
        {
            if (ModelState.IsValid)
            {
                db.Apartamenty.Add(apartamenty);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(apartamenty);
        }

        // GET: Apartamenty/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartamenty apartamenty = db.Apartamenty.Find(id);
            if (apartamenty == null)
            {
                return HttpNotFound();
            }
            return View(apartamenty);
        }

        // POST: Apartamenty/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdApartamentu,Nazwa,Cena,IloscOsob,Opis,Ulica,Miasto,KodPocztowy,IdWlasciciel")] Apartamenty apartamenty)
        {
            if (ModelState.IsValid)
            {
                db.Entry(apartamenty).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(apartamenty);
        }

        // GET: Apartamenty/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Apartamenty apartamenty = db.Apartamenty.Find(id);
            if (apartamenty == null)
            {
                return HttpNotFound();
            }
            return View(apartamenty);
        }

        // POST: Apartamenty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Apartamenty apartamenty = db.Apartamenty.Find(id);
            db.Apartamenty.Remove(apartamenty);
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
