using HotelMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelMVC.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            TestModel model = new TestModel()
            {
                nazwa = "wpisz nazwę",
                ilosc = 0,
                data = DateTime.Now,
                waga = 0.0
            };

            return View(model);
        }

        //POST: Test
        [HttpPost]
        public ActionResult Index(TestModel model)
        {
            //tu można wykonać różne operacje na modelu,
            //np. zapisać go w bazie danych

            return RedirectToAction("Index", "Home");
        }
    }
}