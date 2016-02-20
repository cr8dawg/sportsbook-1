﻿namespace SportsBook.Web.Areas.RegisteredUsers.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Data.Models;
    using Infrastructure.Mapping;
    using Microsoft.AspNet.Identity;
    using SportsBook.Services.Data.Contracts;
    using ViewModels.Facilities;
    using Web.Controllers;

    public class FacilitiesController : BaseController
    {
        private readonly IFacilitiesService facilities;
        private readonly IUsersService users;
        private readonly ICitiesService cities;
        private readonly ISportCategoriesService sportCategories;

        public FacilitiesController(IFacilitiesService facilitiesService, IUsersService usersService, ICitiesService citiesService, ISportCategoriesService sportCategories)
        {
            this.facilities = facilitiesService;
            this.users = usersService;
            this.cities = citiesService;
            this.sportCategories = sportCategories;
        }

        public ActionResult FacilityDetails(int id)
        {
            Facility foundFacility = this.facilities.GetFacilityDetails(id);
            var facilityForView = AutoMapperConfig.Configuration.CreateMapper().Map<FacilityDetailedViewModel>(foundFacility);
            return this.View(facilityForView);
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddFacility()
        {
            FacilityCreateOrChangeViewModel model = new FacilityCreateOrChangeViewModel();
            var cities = this.cities.All();
            model.CitiesDropDown = this.GetSelectListCities(cities);
            var sportCategories = this.sportCategories.All();
            model.SportCategoriesDropDown = this.GetSelectListSportCategories(sportCategories);
            return this.View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddFacility(FacilityCreateOrChangeViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                Facility mappedFacility = AutoMapperConfig.Configuration.CreateMapper().Map<Facility>(model);
                foreach (var categoryId in model.SportCategoriesIds)
                {
                    SportCategory currentCategory = this.sportCategories.GetById(categoryId);
                    mappedFacility.SportCategories.Add(currentCategory);
                }

                mappedFacility.AuthorId = this.User.Identity.GetUserId();

                this.facilities.Add(mappedFacility);
                return this.RedirectToAction("FacilityDetails", new { id = mappedFacility.Id });
            }

            return this.View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditFacility(int Id)
        {
            Facility editedFacilty = this.facilities.GetFacilityDetails(Id);
            FacilityCreateOrChangeViewModel mappedFacility = AutoMapperConfig.Configuration.CreateMapper().Map<FacilityCreateOrChangeViewModel>(editedFacilty);
            var cities = this.cities.All();
            mappedFacility.CitiesDropDown = this.GetSelectListCities(cities);
            var sportCategories = this.sportCategories.All();
            mappedFacility.SportCategoriesDropDown = this.GetSelectListSportCategories(sportCategories);
            return this.View(mappedFacility);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditFacility(int id, FacilityCreateOrChangeViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                Facility mappedFacility = AutoMapperConfig.Configuration.CreateMapper().Map<Facility>(model);

                Facility currentFacility = this.facilities.GetFacilityDetails(id);
                foreach (var category in currentFacility.SportCategories)
                {
                    category.Facilities.Remove(currentFacility);
                }

                currentFacility.SportCategories.Clear();

                this.facilities.Save();
                foreach (var categoryId in model.SportCategoriesIds)
                {
                    SportCategory currentCategory = this.sportCategories.GetById(categoryId);
                    mappedFacility.SportCategories.Add(currentCategory);
                }

                this.facilities.UpdateFacility(id, mappedFacility);
                return this.RedirectToAction("FacilityDetails", new { id = id, area = string.Empty });
            }

            return this.View(model);
        }

        private IEnumerable<SelectListItem> GetSelectListCities(IEnumerable<City> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.Id.ToString(),
                    Text = element.Name
                });
            }

            return selectList;
        }

        private IEnumerable<SelectListItem> GetSelectListSportCategories(IEnumerable<SportCategory> elements)
        {
            // Create an empty list to hold result of the operation
            var selectList = new List<SelectListItem>();
            foreach (var element in elements)
            {
                selectList.Add(new SelectListItem
                {
                    Value = element.Id.ToString(),
                    Text = element.Name
                });
            }

            return selectList;
        }
    }
}