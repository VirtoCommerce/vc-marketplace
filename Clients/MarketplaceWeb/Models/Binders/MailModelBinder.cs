﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MarketplaceWeb.Models.Binders
{
    public class MailModelBinder : IModelBinder
    {
        private readonly string[] _removedKeys = { "to", "subject" };

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext");

            var retVal = new MailModel();

            var form = controllerContext.RequestContext.HttpContext.Request.Form;
			var allKeys = form.AllKeys.ToList();
			allKeys.Remove("returnUrl");
			allKeys.Remove("vendore");

            CheckAndRemoveKeys(allKeys);

            if (allKeys.Contains("fullname"))
                retVal.FullName = form["fullname"];

            retVal.To = form["to"];
            retVal.Subject = form["subject"];

            var builder = new StringBuilder();
            foreach (var key in allKeys)
            {
                builder.AppendLine(string.Format("{0}: {1} <br>", key, form[key]));
            }

            retVal.MailBody = builder.ToString();

            builder.AppendLine(string.Format("EmailTo: {0}", form["to"]));
            retVal.FullMailBody = builder.ToString();

            return retVal;
        }

        public void CheckAndRemoveKeys(List<string> allKeys)
        {
            foreach (var removedKey in _removedKeys)
            {
                if (!allKeys.Contains(removedKey))
                {
                    throw new NullReferenceException(string.Format("No {0} email", removedKey));
                }
                allKeys.Remove(removedKey);
            }
        }
    }
}