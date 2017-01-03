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
        public ActionResult Details(int? id, int? idWizyty)
        {
            if (idWizyty.HasValue)
            {
                ViewData["idWizyty"] = idWizyty.Value;
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Apartamenty apartament =
                db.Apartamenty
                .Include("Wizyty")
                .Include("UdogodnieniaApartamenty.Udogodnienie")
                .First(x => x.IdApartamentu == id);

            if (apartament == null)
            {
                return HttpNotFound();
            }

            ApartamentyDisplayViewModel ap = new ApartamentyDisplayViewModel(apartament);

            return View(ap);
        }

        // GET: Apartamenty/Create
        public ActionResult Create()
        {
            ApartamentyEditViewModel model = new ApartamentyEditViewModel()
            {
                Apartament = new Apartamenty(),
                WszystkieUdogodnienia = db.Udogodnienia.ToList(),
                WybraneUdogodeniniaIds = new int[] { },
                WybraneUdogodnienia = new List<Udogodnienia>()
            };

            return View(model);
        }

        // POST: Apartamenty/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApartamentyEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Apartamenty ap = model.Apartament;

                ap.IdWlasciciel = User.Identity.GetUserId();
                db.Apartamenty.Add(ap);
                db.SaveChanges();

                List<UdogodnieniaApartamenty> udogodnieniaApartamenty = new List<UdogodnieniaApartamenty>();

                foreach (var u in model.WybraneUdogodeniniaIds)
                {
                    db.UdogodnieniaApartamenty.Add(new UdogodnieniaApartamenty()
                    {
                        IdApartamentu = ap.IdApartamentu,
                        IdUdogodnienia = u
                    });
                }

                db.SaveChanges();
                return RedirectToAction("MojeApartamenty");
            }

            model.WszystkieUdogodnienia = db.Udogodnienia.ToList();
            model.WybraneUdogodnienia = db.Udogodnienia.ToList().Where(x => model.WybraneUdogodeniniaIds.Contains(x.IdUdogodnienia)).ToList();
            return View(model);
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

            ApartamentyEditViewModel model = new ApartamentyEditViewModel()
            {
                Apartament = apartamenty,
                WszystkieUdogodnienia = db.Udogodnienia.ToList(),
                WybraneUdogodnienia = db.UdogodnieniaApartamenty.Where(x => x.IdApartamentu == id).Select(u => u.Udogodnienie).ToList(),
            };

            model.WybraneUdogodeniniaIds = model.WybraneUdogodnienia.Select(x => x.IdUdogodnienia).ToArray();

            return View(model);
        }

        // POST: Apartamenty/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApartamentyEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Apartamenty ap_new = model.Apartament;

                Apartamenty ap_old = db.Apartamenty.Find(ap_new.IdApartamentu);

                if (ap_old.IdWlasciciel != User.Identity.GetUserId())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

                ap_old.Cena = ap_new.Cena;
                ap_old.IloscOsob = ap_new.IloscOsob;
                ap_old.KodPocztowy = ap_new.KodPocztowy;
                ap_old.Miasto = ap_new.Miasto;
                ap_old.Nazwa = ap_new.Nazwa;
                ap_old.Opis = ap_new.Opis;
                ap_old.Ulica = ap_new.Ulica;
                ap_old.IdWlasciciel = User.Identity.GetUserId();
                db.SaveChanges();

                List<UdogodnieniaApartamenty> udogodnieniaApartamentyOld = db.UdogodnieniaApartamenty
                    .Where(x => x.IdApartamentu == ap_new.IdApartamentu).ToList();
                db.UdogodnieniaApartamenty.RemoveRange(udogodnieniaApartamentyOld);

                List<UdogodnieniaApartamenty> udogodnieniaApartamenty = new List<UdogodnieniaApartamenty>();

                foreach (var u in model.WybraneUdogodeniniaIds)
                {
                    db.UdogodnieniaApartamenty.Add(new UdogodnieniaApartamenty()
                    {
                        IdApartamentu = ap_new.IdApartamentu,
                        IdUdogodnienia = u
                    });
                }

                db.SaveChanges();
                return RedirectToAction("MojeApartamenty");
            }

            model.WszystkieUdogodnienia = db.Udogodnienia.ToList();

            if (model.WybraneUdogodeniniaIds == null)
                model.WybraneUdogodeniniaIds = new int[] { };
            else
                model.WybraneUdogodnienia = db.Udogodnienia.ToList()
                    .Where(x => model.WybraneUdogodeniniaIds.Contains(x.IdUdogodnienia)).ToList();

            return View(model);
        }

        // GET: Apartamenty/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Apartamenty apartament =
                db.Apartamenty
                .Include("UdogodnieniaApartamenty.Udogodnienie")
                .First(x => x.IdApartamentu == id);

            if (apartament == null)
            {
                return HttpNotFound();
            }

            ApartamentyDisplayViewModel ap = new ApartamentyDisplayViewModel(apartament);

            return View(ap);
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
