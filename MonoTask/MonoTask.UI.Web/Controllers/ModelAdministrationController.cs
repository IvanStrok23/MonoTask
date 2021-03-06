﻿using AutoMapper;
using MonoTask.Common.Interfaces.ServiceInterfaces;
using MonoTask.Core.Entities.Extensions;
using MonoTask.Core.Entities.Helpers;
using MonoTask.UI.Web.Helper;
using MonoTask.UI.Web.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using POCO = MonoTask.Core.Entities;

namespace MonoTask.UI.Web.Controllers
{
    public class ModelAdministrationController : Controller
    {
        private readonly IVehicleModelService _vehicleModelService;
        private readonly IVehicleMakeService _vehicleMakeService;
        private readonly IMapper _mapper;
        public ModelAdministrationController(IVehicleModelService vehicleModelService, IVehicleMakeService vehicleMakeService, IMapper mapper)
        {
            _vehicleModelService = vehicleModelService;
            _vehicleMakeService = vehicleMakeService;
            _mapper = mapper;

        }
        public async Task<ActionResult> Index()
        {
            var items = await _vehicleModelService.GetModels();
            var count = await _vehicleModelService.GetModelCount();

            List<VehicleModelView> viewItems = _mapper.Map<List<VehicleModelView>>(items);
            ViewBag.Items = viewItems;
            ViewBag.SortOrder = "asc";
            ViewBag.CurrentPage = 1;
            ViewBag.PageMax = count / 10 + (count % 10 == 0 ? 0 : 1);
            return View();
        }


        public async Task<ActionResult> SoryByColumn(TableFilterData sortingData)
        {
            SortParams param = new SortParams(sortingData.SortBy.ToSortByEnum(), sortingData.SortOrder.ToSortOrderEnum(), sortingData.SearchValue);

            var items = await _vehicleModelService.GetModelsSortedByColumn(param);
            var count = await _vehicleModelService.GetModelCount(param.SearchValue);

            List<VehicleModelView> viewItems = _mapper.Map<List<VehicleModelView>>(items);
            ViewBag.Items = viewItems;
            setViewBagFilterData(sortingData, count);
            return View("Index");          
        }

        public async Task<ActionResult> GetByPage(TableFilterData sortingData)
        {
            PagingParams param = new PagingParams(sortingData.Page, sortingData.SortBy.ToSortByEnum(), sortingData.SortOrder.ToSortOrderEnum(), sortingData.SearchValue);

            var items = await _vehicleModelService.GetModelsByPage(param);
            var count = await _vehicleModelService.GetModelCount(param.SortParams.SearchValue);

            List<VehicleModelView> viewItems = _mapper.Map<List<VehicleModelView>>(items);
            ViewBag.Items = viewItems;
            setViewBagFilterData(sortingData, count);
            return View("Index");
        }

        public async Task<ActionResult> SearchByName(string searchValue)
        {
            searchValue = searchValue == null ? "" : searchValue;
            var items = await _vehicleModelService.GetModelsByName(searchValue);
            var count = await _vehicleModelService.GetModelCount(searchValue);
            List<VehicleModelView> viewItems = _mapper.Map<List<VehicleModelView>>(items);
            ViewBag.Items = viewItems;
            setViewBagFilterData(new TableFilterData(searchValue), count);
            return View("Index");
        }
        private void setViewBagFilterData(TableFilterData sortingData, int setCount)
        {
            ViewBag.SortOrder = sortingData.SortOrder;
            ViewBag.SearchValue = sortingData.SearchValue;
            ViewBag.CurrentPage = sortingData.Page;
            ViewBag.PageMax = setCount / 10 + (setCount % 10 == 0 ? 0 : 1);
        }

        [HttpPost]
        public async Task<ActionResult> Insert(VehicleModelView model)
        {
            POCO.VehicleModel pocoModel = _mapper.Map<POCO.VehicleModel>(model);
            int id = await _vehicleModelService.InsertModel(pocoModel);
            if (id != 0)
            {
                return getResult(HttpStatusCode.OK, "Insert success!");
            }
            return getResult(HttpStatusCode.BadRequest, "BadRequest");
        }

        [HttpPost]
        public async Task<ActionResult> GetMakeDropdown()
        {
            //TODO: Limit data
            var makeDropdown = await _vehicleMakeService.GetMakeDropdown();
            Response.StatusCode = (int)HttpStatusCode.OK;
            return Json(new { success = false, message = "Success!",data = Newtonsoft.Json.JsonConvert.SerializeObject(makeDropdown) }, JsonRequestBehavior.AllowGet);

        }
        
        [HttpPost]
        public async Task<ActionResult> Edit(POCO.VehicleModel model)
        {
            bool response = await _vehicleModelService.EditModel(model);
            if (response)
            {
                return getResult(HttpStatusCode.OK, "Edit success!");
            }
            return getResult(HttpStatusCode.BadRequest, "BadRequest");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            bool response = await _vehicleModelService.DeleteModel(id);
            if (response)
            {
                return getResult(HttpStatusCode.OK, "Deletion success!");
            }
            return getResult(HttpStatusCode.BadRequest, "BadRequest");
        }

        private JsonResult getResult(HttpStatusCode code , string text)
        {
            Response.StatusCode = (int)code;
            return Json(new { success = false, message = text }, JsonRequestBehavior.AllowGet);
        }


    }
}