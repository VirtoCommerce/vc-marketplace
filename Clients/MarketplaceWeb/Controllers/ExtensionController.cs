﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MarketplaceWeb.Converters;
using MarketplaceWeb.Models;
using VirtoCommerce.ApiClient;

namespace MarketplaceWeb.Controllers
{
    [RoutePrefix("extension")]
    public class ExtensionController : ControllerBase
    {

        [Route("{id}")]
        public async Task<ActionResult> DisplayItem(string id)
        {
            var item = await SearchClient.GetProductAsync(id);

            if (ReferenceEquals(item, null))
            {
                throw new HttpException(404, "Item not found");
            }

            var model = new FullExtension
            {
                Extension = item.ToWebModel()
            };

            return View(model);
        }

        /// <summary>
        /// This method used only for dynamic content. It cannot be exectuted async due to request limitations
        /// </summary>
        [ChildActionOnly]
        public ActionResult DisplayDynamic(string itemCode)
        {
            try
            {
                var result = Task.Run(() => SearchClient.GetProductByCodeAsync(itemCode)).Result;
                var model = result.ToWebModel();
                return PartialView("DisplayTemplates/Item", model);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}