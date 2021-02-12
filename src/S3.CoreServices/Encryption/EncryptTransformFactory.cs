namespace S3.CoreServices.Encryption
{
	class EncryptTransformFactory : CryptoTransformFactory,IEncryptTransformFactory
	{
		public EncryptTransformFactory(IEncryptionSettings settings) : base(settings,true)
		{
		}
	}
}