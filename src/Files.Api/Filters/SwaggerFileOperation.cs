using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Files.Api.Controllers;
using Files.Api.Models;
using NuGet.Packaging;

namespace Files.Api.Filters;
public class SwaggerFileOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.ActionDescriptor.RouteValues["action"] == nameof(FilesController.PostFiles))
        {
            operation.RequestBody = new OpenApiRequestBody();
            operation.RequestBody.Content.Add("multipart/form-data", new OpenApiMediaType()
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                        {
                            ["File"] = new OpenApiSchema
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            },
                        }
                }
            });
        }
        if (context.ApiDescription.ActionDescriptor.RouteValues["action"] == nameof(FilesController.PutFile))
        {
            operation.RequestBody = new OpenApiRequestBody();
            operation.RequestBody.Content.Add("multipart/form-data", new OpenApiMediaType()
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                        {
                            ["File"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary",
                            }
                        }
                }
            });
            
            var additionalProperties = GenerateKeyValuesFromPoco(typeof(FileItemSwashbuckleInputModel));
            operation.RequestBody.Content["multipart/form-data"].Schema.Properties.AddRange(additionalProperties);
        }
    }
    public static Dictionary<string, OpenApiSchema> GenerateKeyValuesFromPoco(Type pocoType)
    {
        var keyValues = new Dictionary<string, OpenApiSchema>();

        foreach (var propertyInfo in pocoType.GetProperties())
        {
            var openApiSchema = new OpenApiSchema
            {
                Type = "string", 
            };

            keyValues[propertyInfo.Name] = openApiSchema;
        }

        return keyValues;
    }
}

