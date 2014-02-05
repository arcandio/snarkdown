﻿using System.Globalization;
using MarkdownSharp;

namespace Kiwi.Markdown
{
	public class MarkdownService : IMarkdownService
    {
        private readonly MarkdownSharp.Markdown _markdown;
        private readonly TextInfo _invariantTextInfo;

		public ITranformers Tranformers { get; set; }

		public IContentProvider ContentProvider { get; set; }

		public MarkdownService(IContentProvider contentProvider)
        {
            ContentProvider = contentProvider;

            _markdown = new MarkdownSharp.Markdown(CreateMarkdownOptions());

            _invariantTextInfo = CultureInfo.InvariantCulture.TextInfo;

			Tranformers = new Tranformers();
        }

		private MarkdownOptions CreateMarkdownOptions()
		{
			return new MarkdownOptions
			{
				AutoHyperlink = true,
				AutoNewLines = false,
				EncodeProblemUrlCharacters = true,
				LinkEmails = true
			};
		}

        public virtual Document GetDocument(string docId)
        {
            return new Document
            {
                Title = _invariantTextInfo.ToTitleCase(docId.Replace("-", " ")),
                Content = ToHtml(ContentProvider.GetContent(docId))
            };
        }

		public string ToHtml(string markdown)
		{
			return ApplyTransformation(markdown);
		}

		protected virtual string ApplyTransformation(string markdownContent)
        {
            foreach (var preTransformation in Tranformers.GetTransformers())
                markdownContent = preTransformation.Invoke(markdownContent);

			return _markdown.Transform(markdownContent).Replace("\n", "\r\n");
        }
    }
}