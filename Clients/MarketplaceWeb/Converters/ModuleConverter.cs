﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;
using MarketplaceWeb.Helpers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using clientModels = VirtoCommerce.Client.Model;
using webModels = MarketplaceWeb.Models;

namespace MarketplaceWeb.Converters
{
	public static class ModuleConverter
	{
		public const string OverviewProperty = "Overview";
		public const string LocaleProperty = "Locale";
		public const string UserIdProperty = "VendorId";
		public const string DescriptionProperty = "Description";
		public const string LicenseProperty = "License";

		//Variation properties
		public const string CompatibilityProperty = "Compatibility";
		public const string ReleaseDateProperty = "ReleaseDate";
		public const string ReleaseStatusProperty = "ReleaseStatus";
		public const string LinkProperty = "DowloadLink";
		public const string NoteProperty = "ReleaseNote";
		public const string VersionProperty = "ReleaseVersion";


        public static webModels.Module ToWebModel(this clientModels.VirtoCommerceCatalogModuleWebModelProduct item)
        {
            var retVal = new webModels.Module
            {
                Code = item.Code,
                Id = item.Id,
                Title = item.Name,
                CatalogId = item.CatalogId,
                ReviewsTotal = 0,
                Price = webModels.PriceModel.Parse(item.Properties),
                Keyword = item.SeoInfos?.FirstOrDefault()?.SemanticUrl ?? item.Id,
            };

            if (item.Images != null && item.Images.Any())
            {
                retVal.Images.AddRange(item.Images.Select(i => i.ToWebModel()));
            }

            if (item.Reviews != null)
            {
                var reviews = item.Reviews.Where(x => !string.IsNullOrWhiteSpace(x.ReviewType)).ToArray();
                var shortReview = reviews.FirstOrDefault(
                    x => x.ReviewType.Equals("QuickReview", StringComparison.OrdinalIgnoreCase));

                var fullReview = reviews.FirstOrDefault(
                    x => x.ReviewType.Equals("FullReview", StringComparison.OrdinalIgnoreCase));

                if (fullReview != null)
                {
                    retVal.FullDescription = fullReview.Content;
                }
            }

            if (item.Properties != null)
            {
                retVal.Overview = item.Properties.ParsePropertyToString(OverviewProperty);
                retVal.Locale = item.Properties.ParseProperty(LocaleProperty).ToList();
                retVal.UserId = item.Properties.ParsePropertyToString(UserIdProperty);
                retVal.Description = item.Properties.ParsePropertyToString(DescriptionProperty);
                retVal.License = item.Properties.ParsePropertyToString(LicenseProperty);
            }

            if (item.Variations != null)
            {
                retVal.Releases = item.Variations.Select(x => x.ToVariationWebModel(retVal)).ToList();
            }

            if (retVal.Releases.Count > 0)
            {
                retVal.DownloadLink = retVal.Releases.OrderByDescending(r => r.ReleaseDate).First().DownloadLink;
            }

            return retVal;
        }

        //public static webModels.Release ToWebModel(this clientModels.VirtoCommerceCatalogModuleWebModelProduct variation, webModels.Module parent)
        //{
        //    var retVal = new webModels.Release
        //    {
        //        Id = variation.Id,
        //        Compatibility = variation.Properties.ParseProperty(CompatibilityProperty).ToList(),
        //        DownloadLink = variation.Properties.ParsePropertyToString(LinkProperty),
        //        ReleaseDate = variation.Properties.ParseProperty<DateTime>(ReleaseDateProperty).FirstOrDefault(),
        //        Note = variation.Properties.ParsePropertyToString(NoteProperty),
        //        Version = variation.Properties.ParsePropertyToString(VersionProperty),
        //        ReleaseStatus = variation.Properties.ParseProperty<webModels.ReleaseStatus>(ReleaseStatusProperty).FirstOrDefault(),
        //        ParentExtension = parent
        //    };

        //    if (variation.Assets.Any())
        //    {
        //        retVal.DownloadLink = variation.Assets.First().Url;
        //    }

        //    return retVal;
        //}

        //public static webModels.Module ToWebModel(this clientModels.VirtoCommerceCatalogModuleWebModelProduct item)
        //{
        //    var retVal = new webModels.Module
        //    {
        //        Code = item.Code,
        //        Id = item.Id,
        //        Title = item.Name,
        //        CatalogId = item.CatalogId,
        //        ReviewsTotal = 0,
        //        Price = webModels.PriceModel.Parse(item.Properties),
        //        Keyword = item.SeoInfos.Count > 0 ? item.SeoInfos.First().SemanticUrl : string.Empty,
        //    };

        //    //if (item.PrimaryImage != null)
        //    //{
        //    //    retVal.Images.Add(item.PrimaryImage.ToWebModel());
        //    //}

        //    if (item.Images != null && item.Images.Any())
        //    {
        //        retVal.Images.AddRange(item.Images.Select(i => i.ToWebModel()));
        //    }

        //    //if (item.EditorialReviews != null)
        //    //{
        //    //    var reviews = item.EditorialReviews.Where(x => !string.IsNullOrWhiteSpace(x.ReviewType)).ToArray();
        //    //    var shortReview = reviews.FirstOrDefault(
        //    //        x => x.ReviewType.Equals("QuickReview", StringComparison.OrdinalIgnoreCase));

        //    //    var fullReview = reviews.FirstOrDefault(
        //    //        x => x.ReviewType.Equals("FullReview", StringComparison.OrdinalIgnoreCase));

        //    //    if (fullReview != null)
        //    //    {
        //    //        retVal.FullDescription = fullReview.Content;
        //    //    }
        //    //}

        //    if (item.Properties != null)
        //    {
        //        retVal.Overview = item.Properties.ParsePropertyToString(OverviewProperty);
        //        retVal.Locale = item.Properties.ParseProperty(LocaleProperty).ToList();
        //        retVal.UserId = item.Properties.ParsePropertyToString(UserIdProperty);
        //        retVal.Description = item.Properties.ParsePropertyToString(DescriptionProperty);
        //        retVal.License = item.Properties.ParsePropertyToString(LicenseProperty);
        //    }

        //    if (item.Variations != null && item.Variations.Count > 0)
        //    {
        //        retVal.Releases = item.Variations.Select(x => x.ToVariationWebModel(retVal)).ToList();
        //    }

        //    if (retVal.Releases.Count > 0)
        //    {
        //        retVal.DownloadLink = retVal.Releases.OrderBy(r => r.ReleaseDate).Last().DownloadLink;
        //    }

        //    return retVal;
        //}

        public static webModels.Release ToVariationWebModel(this clientModels.VirtoCommerceCatalogModuleWebModelProduct variation, webModels.Module parent)
        {
            var retVal = new webModels.Release
            {
                Id = variation.Id,
                Compatibility = variation.Properties.ParseProperty(CompatibilityProperty).ToList(),
                DownloadLink = variation.Properties.ParsePropertyToString(LinkProperty),
                ReleaseDate = variation.Properties.ParseProperty<DateTime>(ReleaseDateProperty).FirstOrDefault(),
                Note = variation.Properties.ParsePropertyToString(NoteProperty),
                Version = variation.Properties.ParsePropertyToString(VersionProperty),
                ReleaseStatus = variation.Properties.ParseProperty<webModels.ReleaseStatus>(ReleaseStatusProperty).FirstOrDefault(),
                ParentExtension = parent
            };

            if (variation.Assets.Any())
            {
                retVal.DownloadLink = variation.Assets.First().Url;
            }

            return retVal;
        }

        public static string[] ParseProperty(this List<clientModels.VirtoCommerceCatalogModuleWebModelProperty> properties, string name)
        {
            var property = properties.FirstOrDefault(p => p.Name == name);
            if(property != null)
            {
                return property.Values.Select(v => v.Value.ToString()).ToArray();
            }

            return new string[0];
        }

        public static string[] ParseProperty(this IDictionary<string, object> properties, string name)
		{
			var key = properties.Keys.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(key))
			{
				if (properties[key] is string[])
				{
					return (string[])properties[key];
				}
				else if (properties[key] is JArray)
				{
					return JsonConvert.DeserializeObject<string[]>(properties[key].ToString());
				}
				else
				{
					return new string[] { properties[key].ToString() };
				}
			}
			else
			{
				return new string[0];
			}
		}

		public static T[] ParseProperty<T>(this IDictionary<string, object> properties, string name)
		{
			var retVal = new List<T>();
			var values = ParseProperty(properties, name);
			if (values != null && values.Any())
			{
				var type = typeof(T);


				var tc = TypeDescriptor.GetConverter(type);

				foreach (var value in values)
				{
					try
					{
						var result = (T)tc.ConvertFromString(null, CultureInfo.InvariantCulture, value);
						retVal.Add(result);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}

			return retVal.ToArray();
		}

        public static T[] ParseProperty<T>(this List<clientModels.VirtoCommerceCatalogModuleWebModelProperty> properties, string name)
        {
            var retVal = new List<T>();
            var values = ParseProperty(properties, name);
            if (values != null && values.Any())
            {
                var type = typeof(T);


                var tc = TypeDescriptor.GetConverter(type);

                foreach (var value in values)
                {
                    try
                    {
                        var result = (T)tc.ConvertFromString(null, CultureInfo.InvariantCulture, value);
                        retVal.Add(result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return retVal.ToArray();
        }

        public static string ParsePropertyToString(this IDictionary<string, object> properties, string name,
			string separator = ", ")
		{
			var values = ParseProperty(properties, name);

			if (values != null)
			{
				return values.Length > 1 ? string.Join(separator, values) : values.FirstOrDefault();
			}

			return null;
		}

        public static string ParsePropertyToString(this List<clientModels.VirtoCommerceCatalogModuleWebModelProperty> properties, string name, string separator = ", ")
        {
            var values = ParseProperty(properties, name);

            if (values != null)
            {
                return values.Length > 1 ? string.Join(separator, values) : values.FirstOrDefault();
            }

            return null;
        }

	}
}