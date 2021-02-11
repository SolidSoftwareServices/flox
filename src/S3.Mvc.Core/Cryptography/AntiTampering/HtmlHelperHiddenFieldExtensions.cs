using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using S3.CoreServices.Encryption;
using S3.Mvc.Core.System;
using S3.Mvc.Core.Views;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace S3.Mvc.Core.Cryptography.AntiTampering
{

	public static class HtmlHelperHiddenFieldExtensions
	{
		

		
		public static async Task<HtmlString> SecureHiddenAsync(this IHtmlHelper htmlHelper, string name,
			bool isVisibleAsDecryptedByClient, string value=null,object htmlAttributes=null)
		{
			return await htmlHelper.InputHelperAsync( isVisibleAsDecryptedByClient, name,  value, htmlAttributes:(IDictionary<string, object>)  htmlAttributes);
		}
		
		public static async Task <HtmlString> SecureHiddenForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression, bool isVisibleAsDecryptedByClient)
		{
			return await htmlHelper.SecureHiddenForAsync( expression, null, isVisibleAsDecryptedByClient);
		}
		public static async Task<HtmlString> SecureHiddenForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			object htmlAttributes, bool isVisibleAsDecryptedByClient)
		{
			return await htmlHelper.SecureHiddenForAsync( expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), isVisibleAsDecryptedByClient);
		}
		public static async Task<HtmlString> SecureHiddenForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression,
			IDictionary<string, object> htmlAttributes, bool isVisibleAsDecryptedByClient)
		{
			var expressionProvider = new ModelExpressionProvider(htmlHelper.MetadataProvider);

			var metadata = expressionProvider.CreateModelExpression(htmlHelper.ViewData, expression);

			string name = expressionProvider.GetExpressionText(expression);
			return await htmlHelper.InputHelperAsync(isVisibleAsDecryptedByClient, name, metadata.Model?.ToString(), htmlAttributes: htmlAttributes);
		}

		
		private static readonly string HiddenTypeString = InputType.Hidden.ToString().ToLowerInvariant();

		private static async Task<HtmlString> InputHelperAsync(
			this IHtmlHelper htmlHelper,
			bool isVisibleAsDecryptedByClient,
			string name,
			string value,
			IDictionary<string, object> htmlAttributes)
		{
			string fullName;

			fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

			if (string.IsNullOrEmpty(fullName))
			{
				throw new ArgumentException("name");
			}

			var inputItemBuilder = new StringBuilder();
			if (isVisibleAsDecryptedByClient)
			{
				inputItemBuilder.Append(BuildHiddenWithoutEncryption(value, htmlAttributes, fullName));
			}

			var appendHashedValueAsync = htmlHelper.AppendHashedValueAsync(value, fullName, inputItemBuilder,
				isVisibleAsDecryptedByClient);
			await appendHashedValueAsync;


			return new HtmlString(inputItemBuilder.ToString());
		}

		private static async Task AppendHashedValueAsync(this IHtmlHelper htmlHelper, string value, string fullName,
			StringBuilder inputItemBuilder, bool isVisibleAsDecryptedByClient)
		{
			var encryptionService = htmlHelper.ViewContext.HttpContext.Resolve<IEncryptionService>();
			var encryptValueAsync = encryptionService.EncryptAsync(value, true);

			var hiddenInputHash = new TagBuilder("input")
			{
				TagRenderMode = TagRenderMode.SelfClosing
			};
			hiddenInputHash.MergeAttribute("type", HiddenTypeString);
			var prefix = isVisibleAsDecryptedByClient
				? EncryptionConstants.TokenPrefixAntiTampering
				: EncryptionConstants.TokenPrefixEncrypted;
			hiddenInputHash.MergeAttribute("name", $"{prefix}{fullName}", true);

			hiddenInputHash.MergeAttribute(
				"value",
				await encryptValueAsync);

			inputItemBuilder.Append(hiddenInputHash.GetString());
		}

		public static async Task<HtmlString> HoneypotForAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TProperty>> expression)
		{
			var expressionProvider = new ModelExpressionProvider(htmlHelper.MetadataProvider);
			string name = expressionProvider.GetExpressionText(expression);
			return await htmlHelper.InputHelperAsync(true, name, string.Empty, htmlAttributes: null);

		}
		internal static string BuildHiddenWithoutEncryption(string value, IDictionary<string, object> htmlAttributes, string fullName)
		{
			var hiddenInput = new TagBuilder("input")
			{
				TagRenderMode = TagRenderMode.SelfClosing
			};
			hiddenInput.MergeAttributes(htmlAttributes);

			hiddenInput.MergeAttribute("type", HiddenTypeString);
			hiddenInput.MergeAttribute("name", fullName, true);
			hiddenInput.MergeAttribute("value", value);
			return hiddenInput.GetString();
		}
	}
}
