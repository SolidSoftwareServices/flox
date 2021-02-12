namespace S3.CoreServices.Encryption
{
	class DecryptTransformFactory : CryptoTransformFactory,IDecryptTransformFactory
	{
		public DecryptTransformFactory(IEncryptionSettings settings) : base(settings,false)
		{
		}
	}
}