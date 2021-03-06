﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketplaceWeb.Models
{
	public class ReviewModel
	{
		public string Id { get; set; }
		public int Rating { get; set; }
		public string Title { get; set; }
		public string ReviewText { get; set; }
		public DateTime? Created { get; set; }
		public string ProductId { get; set; }

		public ReviewAuthor Author { get; set; }
	}

	public class ReviewAuthor
	{
		public string Name { get; set; }
		public string Icon { get; set; }
	}
}