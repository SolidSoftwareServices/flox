using System.Security.Cryptography;
using System.Threading.Tasks;

namespace S3.CoreServices.Encryption
{
	internal interface ICryptoTransformFactory
	{
		Task<ICryptoTransform> Create();
		bool Return(Task<ICryptoTransform> obj);
	}
}