using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Donatas.Core.Exceptions;
using Donatas.Core.Extensions;

namespace Donatas.Core.WebApi.Responses
{
    public class CoreResponseFactory : ICoreResponseFactory
    {
        public ICoreResponse GetCoreResponse(Exception ex) =>
            ex switch
            {
                BusinessException businessException =>
                    new CoreResponse(HttpStatusCode.UnprocessableEntity, ex.Message, null),
                BadHttpRequestException badHttpRequestException =>
                    new CoreResponse(HttpStatusCode.BadRequest, ex.Message, null),
                ValidationException validationException =>
                    new CoreResponse(HttpStatusCode.NotAcceptable, ex.Message, validationException.GetErrorMessages()),
                _ =>
                    new CoreResponse(HttpStatusCode.InternalServerError, ex.ToDetailedMessage(), null),
            };
    }
}
