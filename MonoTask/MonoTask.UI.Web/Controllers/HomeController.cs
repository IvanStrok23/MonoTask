﻿using MonoTask.Core.Services;
using MonoTask.Infrastructure.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POCO = MonoTask.Core.Entities;


namespace MonoTask.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleService _vehicleModelService;
        public HomeController(IVehicleService vehicleModelService)
        {
            _vehicleModelService = vehicleModelService;
        }

        public ActionResult Index()
        {
            POCO.VehicleModel item = _vehicleModelService.GetTest();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}