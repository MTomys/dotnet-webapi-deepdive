using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace CourseLibrary.API.ActionConstraints;

[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
{
    private readonly string _requestHeaderToMatch;
    private readonly MediaTypeCollection _mediaTypes = new();

    public RequestHeaderMatchesMediaTypeAttribute(
        string requestHeaderToMatch,
        string mediaType,
        params string[] otherMediaTypes)
    {
        _requestHeaderToMatch = requestHeaderToMatch
                                ?? throw new ArgumentNullException(nameof(requestHeaderToMatch));

        if (MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
        {
            _mediaTypes.Add(parsedMediaType);
        }
        else
        {
            throw new ArgumentException("invalid media types found", nameof(mediaType));
        }

        foreach (var otherMediaType in otherMediaTypes)
        {
            if (MediaTypeHeaderValue.TryParse(otherMediaType, out var parsedOtherMediaType))
            {
                _mediaTypes.Add(parsedOtherMediaType);
            }
            else
            {
                throw new ArgumentException("invalid media types found", nameof(otherMediaType));
            }
        }
    }

    public bool Accept(ActionConstraintContext context)
    {
        var requestHeaders = context.RouteContext.HttpContext.Request.Headers;
        if (!requestHeaders.ContainsKey(_requestHeaderToMatch))
        {
            return false;
        }

        var parsedRequestMediaType = new MediaType(requestHeaders[_requestHeaderToMatch]);

        foreach (var mediaType in _mediaTypes)
        {
            var parsedMediaType = new MediaType(mediaType);
            if (parsedRequestMediaType.Equals(parsedMediaType))
            {
                return true;
            }
        }

        return false;
    }

    public int Order { get; }
}