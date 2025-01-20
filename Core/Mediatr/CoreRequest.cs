using AutoMapper.Configuration.Annotations;
using MediatR;

namespace Donatas.Core.Mediatr
{
    public class CoreRequest<T> : IRequest<T>
    {
        [Ignore]
        public string Message { get; set; } = string.Empty;
        [Ignore]
        public bool CreateLog { get; set; } = true;
    }
}
