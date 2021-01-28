using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Infrastructure.Settings
{
    public interface IImagesSetting
    {
	    string RegularImagePath { get; }
	    string MobileImagePath { get; }
	    string AltText { get; }
	}

    public class ImagesSetting : IImagesSetting
    {
	    public string RegularImagePath { get; set; }
	    public string MobileImagePath { get; set; }
	    public string AltText { get; set; }
    }
}
