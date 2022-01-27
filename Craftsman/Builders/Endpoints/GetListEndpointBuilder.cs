﻿namespace Craftsman.Builders.Endpoints
{
    using System.Collections.Generic;
    using Enums;
    using Helpers;
    using Models;

    public class GetListEndpointBuilder
    {
        public static string GetEndpointTextForGetList(Entity entity, bool addSwaggerComments, Feature feature)
        {
            var lowercaseEntityVariable = entity.Name.LowercaseFirstLetter();
            var entityName = entity.Name;
            var entityNamePlural = entity.Plural;
            var readDto = Utilities.GetDtoName(entityName, Dto.Read);
            var readParamDto = Utilities.GetDtoName(entityName, Dto.ReadParamaters);
            var queryListMethodName = Utilities.QueryListName(entityName);
            var listResponse = $@"IEnumerable<{readDto}>";
            var getListEndpointName = entity.Name == entity.Plural ? $@"Get{entityNamePlural}List" : $@"Get{entityNamePlural}";
            var getListAuthorization = feature.IsProtected ? EndpointSwaggerCommentBuilders.BuildAuthorizations(feature.PermissionName) : "";

            return @$"{EndpointSwaggerCommentBuilders.GetSwaggerComments_GetList(entity, addSwaggerComments, listResponse, getListAuthorization.Length > 0)}{getListAuthorization}
    [Produces(""application/json"")]
    [HttpGet(Name = ""{getListEndpointName}"")]
    public async Task<IActionResult> Get{entityNamePlural}([FromQuery] {readParamDto} {lowercaseEntityVariable}ParametersDto)
    {{
        var query = new {Utilities.GetEntityListFeatureClassName(entity.Name)}.{queryListMethodName}({lowercaseEntityVariable}ParametersDto);
        var queryResponse = await _mediator.Send(query);

        var paginationMetadata = new
        {{
            totalCount = queryResponse.TotalCount,
            pageSize = queryResponse.PageSize,
            currentPageSize = queryResponse.CurrentPageSize,
            currentStartIndex = queryResponse.CurrentStartIndex,
            currentEndIndex = queryResponse.CurrentEndIndex,
            pageNumber = queryResponse.PageNumber,
            totalPages = queryResponse.TotalPages,
            hasPrevious = queryResponse.HasPrevious,
            hasNext = queryResponse.HasNext
        }};

        Response.Headers.Add(""X-Pagination"",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }}";
        }
        
    }
}