using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.Infrastructure.Settings
{
	public interface IGoogleSettings
	{
		bool IsTagManagerEnabled { get; }
		string TagManagerUrl { get; }
		string TagManagerCode { get; }
	}
}
