using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace Raven.Abstractions.Data
{
	public class GetRequest
	{
		/// <summary>
		/// Request url (relative).
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Request headers.
		/// </summary>
		public IDictionary<string, string> Headers { get; set; }

		/// <summary>
		/// Query information e.g. "?pageStart=10&pageSize=20".
		/// </summary>
		public string Query { get; set; }
		
		public string Method { get; set; }

		/// <summary>
		/// Concatenated Url and Query.
		/// </summary>
		[JsonIgnore]
		public string UrlAndQuery
		{
			get
			{
				if (Method != null && Method != "Get")
					return Url;

				if (Query == null)
					return Url;
				
				if (Query.StartsWith("?"))
					return Url + Query;
				return Url + "?" + Query;
			}
		}

		public string Content { get; set; }

		public GetRequest()
		{
			Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}
	}
}