﻿namespace SportsBook.Web.Areas.Facilities
{
    using System.Web.Mvc;

    public class FacilitiesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Facilities";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Facilities_default",
                "Facilities/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }
    }
}