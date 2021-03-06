﻿using System.Collections.Generic;
using System.Linq;

namespace SparkPost
{
    public class DataMapper
    {
        public DataMapper(string version)
        {
            // sticking with v1 for now
        }

        public virtual IDictionary<string, object> ToDictionary(Transmission transmission)
        {
            return RemoveNulls(new Dictionary<string, object>
            {
                ["content"] = ToDictionary(transmission.Content),
                ["campaign_id"] = transmission.CampaignId,
                ["description"] = transmission.Description,
                ["return_path"] = transmission.ReturnPath,
                ["metadata"] = transmission.Metadata.Count > 0 ? transmission.Metadata : null,
                ["options"] = ToDictionary(transmission.Options),
                ["substitution_data"] = transmission.SubstitutionData.Count > 0 ? transmission.SubstitutionData : null,
                ["recipients"] = BuildTheRecipientRequestFrom(transmission)
            });
        }

        public virtual IDictionary<string, object> ToDictionary(Recipient recipient)
        {
            return RemoveNulls(new Dictionary<string, object>
            {
                ["address"] = ToDictionary(recipient.Address),
                ["return_path"] = recipient.ReturnPath,
                ["tags"] = recipient.Tags.Count > 0 ? recipient.Tags : null,
                ["metadata"] = recipient.Metadata.Count > 0 ? recipient.Metadata : null,
                ["substitution_data"] = recipient.SubstitutionData.Count > 0 ? recipient.SubstitutionData : null
            });
        }

        public virtual IDictionary<string, object> ToDictionary(Address address)
        {
            return RemoveNulls(new Dictionary<string, object>
            {
                ["email"] = address.Email,
                ["name"] = address.Name,
                ["header_to"] = address.HeaderTo
            });
        }

        public virtual IDictionary<string, object> ToDictionary(Options options)
        {
            if (typeof(Options)
                .GetProperties()
                .Any(x => x.GetValue(options) != null))
                return RemoveNulls(new Dictionary<string, object>
                {
                    ["start_time"] =
                        options.StartTime.HasValue ? string.Format("{0:s}{0:zzz}", options.StartTime.Value) : null,
                    ["open_tracking"] = options.OpenTracking.HasValue && options.OpenTracking.Value,
                    ["click_tracking"] = options.ClickTracking.HasValue && options.ClickTracking.Value,
                    ["transactional"] = options.Transactional.HasValue && options.Transactional.Value,
                    ["sandbox"] = options.Sandbox.HasValue && options.Sandbox.Value,
                    ["skip_suppression"] = options.SkipSuppression.HasValue && options.SkipSuppression.Value
                });
            return null;
        }

        public virtual IDictionary<string, object> ToDictionary(Content content)
        {
            return RemoveNulls(new Dictionary<string, object>
            {
                ["from"] = content.From.Email,
                ["subject"] = content.Subject,
                ["text"] = content.Text,
                ["html"] = content.Html,
                ["reply_to"] = content.ReplyTo,
                ["template_id"] = content.TemplateId,
                ["attachments"] = content.Attachments.Any() ? content.Attachments.Select(ToDictionary) : null,
                ["inline_images"] = content.InlineImages.Any() ? content.InlineImages.Select(ToDictionary) : null,
                ["headers"] = content.Headers.Keys.Count > 0 ? content.Headers : null,
            });
        }

        public virtual IDictionary<string, object> ToDictionary(Attachment attachment)
        {
            return ToDictionary(attachment as File);
        }

        public virtual IDictionary<string, object> ToDictionary(InlineImage inlineImage)
        {
            return ToDictionary(inlineImage as File);
        }

        public virtual IDictionary<string, object> ToDictionary(File file)
        {
            return new Dictionary<string, object>()
            {
                ["name"] = file.Name,
                ["type"] = file.Type,
                ["data"] = file.Data
            };
        }

        private object BuildTheRecipientRequestFrom(Transmission transmission)
        {

            return transmission.ListId != null
                ? (object)new Dictionary<string, object> { ["list_id"] = transmission.ListId }
                : transmission.Recipients.Select(ToDictionary);
        }

        private static IDictionary<string, object> RemoveNulls(IDictionary<string, object> dictionary)
        {
            var newDictionary = new Dictionary<string, object>();
            foreach (var key in dictionary.Keys.Where(k => dictionary[k] != null))
                newDictionary[key] = dictionary[key];
            return newDictionary;
        }
    }
}
