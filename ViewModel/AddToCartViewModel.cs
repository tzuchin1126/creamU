using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Collections.Specialized.BitVector32;
using System.Drawing;
using System.Xml.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace products.ViewModel
{
    public class AddToCartViewModel
    {
        public int txtCount { get; set; } 
        public int txtFid { get; set; }


		public string? ProductImage { get; set; }
		public string? ProductName { get; set; }
		public string? Descript { get; set; }
		public int? Price { get; set; } 

	}
}




