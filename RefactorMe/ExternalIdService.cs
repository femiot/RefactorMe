﻿using Microsoft.Extensions.Logging;
using Opsi.Architecture;
using Opsi.Cloud.Core.Interface;
using Opsi.Cloud.Core.Model;
using RefactorMe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Opsi.Cloud.Core
{
    public class ExternalIdService : IExternalIdGeneratorService
    {
        private readonly ILogger<ExternalIdService> _logger;
        private readonly string _regExMatch = @"({[a-z]*:[^:]*})";

        public ExternalIdService(
          ILogger<ExternalIdService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceActionResult> GenerateAsync(List<Dictionary<string, object>> entities, TypeMetadata typeMetadata)
        {
            var result = new ServiceActionResult();

            var namingPattern = typeMetadata.Name.GetNamingPattern();

            if (string.IsNullOrWhiteSpace(namingPattern))
            {
                result.Errors.Add("Cannot not get TypeMetaData Naming Pattern as Name is blank!");
                return result;
            }

            var split = Regex.Split(namingPattern, _regExMatch).Where(s => s != string.Empty).ToArray();

            foreach (var entity in entities)
            {
                var sb = new StringBuilder();
                await Task.Run(() => BuildIdentity(sb, split, entity));
                entity["externalId"] = sb.ToString();
            }

            return result;
        }

        private void BuildIdentity(StringBuilder sb, string[] split, object entity)
        {
            foreach (var section in split)
            {
                if (section.StartsWith('{'))
                {
                    var args = Regex.Split(section, @":|{|}").Where(s => s != string.Empty).ToArray();
                    switch (args[0])
                    {
                        case "date":
                            sb.Append(args[1].GetDate());
                            break;

                        case "increment":
                            sb.Append(args[1].GetIncrement());
                            break;

                        case "entity":
                            sb.Append(args[1].GetValueFromEntityObject(entity));
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    sb.Append(section);
                }
            }
        }
    }

}




