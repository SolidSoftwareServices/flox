using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using Ei.Rp.Mvc.Core.Authx;
using Ei.Rp.Mvc.Core.Controllers;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.Models.LogView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Ei.Rp.Mvc.Core.Controllers.ControllerBase;

namespace EI.RP.WebApp.Controllers
{
	[AuthorizedOnlyDuringDevelopment]
	[AllowAnonymous]
	public class LogViewController :Controller
	{
		private readonly bool _enabled;
		private readonly string _logDir;

		private static readonly string PathPrefix =
			Uri.UnescapeDataString(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);

		public LogViewController(IUiAppSettings settings,IHostingEnvironment hostingEnvironment)
		{
			_enabled = settings.LogViewerEnabled && !hostingEnvironment.IsProductionEnvironment();
			if (_enabled)
			{
				_logDir = settings.LogsRoot;

				if (_logDir.StartsWith("~/") || _logDir.StartsWith("/"))
				{
					_logDir = $"{PathPrefix}{_logDir.Replace("~", string.Empty)}";
				}
			}
		}
		[Authorize]
		public async Task<ActionResult> SessionId()
		{
			if (!_enabled) return Forbid();
			return new JsonResult(new{ SessionId=HttpContext?.Session?.Id});
		}

		
		public async Task<ActionResult> SessionLog(string sessionId=null,string category="all" )
		{
			
			if (!_enabled) return Forbid();
			sessionId = sessionId ?? HttpContext?.Session?.Id;
			var file = GetLogFiles().OrderByDescending(x=>x.Name.Length).FirstOrDefault(x =>
				x.Name.Contains(category, StringComparison.InvariantCultureIgnoreCase) &&
				x.Name.Contains(sessionId, StringComparison.InvariantCultureIgnoreCase));
			if (file == null) return NotFound();
			return await Index(file.Name);
		}

		public async Task<ActionResult> Index(string log)
		{
			if (!_enabled) return Forbid();
			if (!string.IsNullOrEmpty(log))
			{
				var path = Path.Combine(_logDir, log);

				var memory = new MemoryStream();
				using (var stream = new FileStream(path, FileMode.Open))
				{
					await stream.CopyToAsync(memory);
				}

				memory.Position = 0;
				return File(memory, "text/plain", Path.GetFileName(path));
			}
			var logs = GetLogFiles();

			return View(new LogViewModel { LogFiles = logs });
		}

		private LogFileModel[] GetLogFiles()
		{
			var pathPrefix = $"/{GetType().GetNameWithoutSuffix()}/{nameof(Index)}";

			var logs = Directory.GetFiles(_logDir)
				.Where(path => path.EndsWith(".log"))
				.Select(path => new FileInfo(path))
				.Select(file => new LogFileModel
				{
					Name = file.Name,
					Url = pathPrefix + $"?log={Uri.EscapeUriString(file.Name)}",
					Size = GetFriendlyFileSize(file.Length),
					LastChanged = file.LastWriteTimeUtc
				}).OrderByDescending(x => x.LastChanged)
				.ToArray();
			return logs;
		}

		public ActionResult Delete(string log)
		{
			if (!_enabled) return Forbid();
			if (!string.IsNullOrWhiteSpace(log))
			{
				var logPath = Path.Combine(_logDir, log);
				System.IO.File.Delete(logPath);
			}
			else
			{
				var logs = Directory.GetFiles(_logDir)
					.Where(path => path.EndsWith(".log")).ToArray();
				foreach (var logFile in logs)
				{
					try
					{
						System.IO.File.Delete(logFile);
					}
					catch
					{
						//swallow exception on purpose. This is intended as we dont log this tool
					}
				}
			}
			return Redirect($"~/{GetType().GetNameWithoutSuffix()}");
		}

		private string GetFriendlyFileSize(long lengthInBytes)
		{
			var kb = Math.Round(lengthInBytes / 1024d);
			var groupSeparator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
			var friendly = kb.ToString("N0").Replace(groupSeparator, " ") + " KB";

			return friendly;
		}

	}
}