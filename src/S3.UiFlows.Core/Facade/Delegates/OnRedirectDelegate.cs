using System.Threading.Tasks;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.UiFlows.Core.Facade.Delegates
{
	public delegate Task<TResult> OnRedirectDelegate<TResult>(ExitRedirect model);
}