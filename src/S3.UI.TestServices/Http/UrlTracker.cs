using System;
using System.Collections.Generic;
using System.Net.Http;

namespace S3.UI.TestServices.Http
{

	class UrlTracker
	{
		private readonly List<string> _urls = new List<string>();
		private int _urlTrackerPosition = -1;
		private readonly object _syncLock=new object();
		public void Record(HttpResponseMessage responseMessage)
		{
			if (responseMessage.IsSuccessStatusCode && responseMessage.RequestMessage.Method == HttpMethod.Get)
			{
				lock (_syncLock)
				{
					var actual = responseMessage.RequestMessage.RequestUri.ToString();

					_urlTrackerPosition++;
					_urls.Insert(_urlTrackerPosition, actual);
					if (_urlTrackerPosition < _urls.Count - 1)
					{
						_urls.RemoveRange(_urlTrackerPosition + 1, _urls.Count - _urlTrackerPosition - 1);
					}
				}
			}
		}

		public string GetCurrent()
		{
			return _urls[_urlTrackerPosition];
		}

		public UrlTracker ToPrevious()
		{
			if(_urlTrackerPosition<=0) throw new InvalidOperationException();
			_urlTrackerPosition--;
			return this;
		}

		public UrlTracker ToNext()
		{
			if (_urlTrackerPosition >= _urls.Count-1) throw new InvalidOperationException();
			_urlTrackerPosition++;
			return this;
		}

		public void Reset()
		{
			_urls.Clear();
			_urlTrackerPosition = -1;
		}
	}
}