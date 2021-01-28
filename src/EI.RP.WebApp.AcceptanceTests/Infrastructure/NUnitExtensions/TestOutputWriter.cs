using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions
{
	public static class TestOutputWriter
	{
		public static async Task WriteOutputLineAsync(this TextWriter contextOutWriter,string txt)
		{
			txt = $"{DateTime.UtcNow.TimeOfDay} - ({TestContext.CurrentContext.Test.Name}) - {txt}";
			await TestContext.Progress.WriteLineAsync(txt);
		}
		
		public static async Task WriteOutputLineAsync(this TextWriter contextOutWriter,string format,params object[]args)
		{
			await contextOutWriter.WriteOutputLineAsync(string.Format(format,args));
		}
		public static void WriteOutputLine(this TextWriter contextOutWriter,string txt)
		{
			contextOutWriter.WriteOutputLineAsync(txt).GetAwaiter().GetResult();
		}
		public static void WriteOutputLine(this TextWriter contextOutWriter,string format,params object[]args)
		{
			contextOutWriter.WriteOutputLineAsync(format,args).GetAwaiter().GetResult();
		}
		public static void WriteOutputLine(this TextWriter contextOutWriter,Exception ex)
		{
			contextOutWriter.WriteOutputLine(ex.ToString());
		}
	}
}