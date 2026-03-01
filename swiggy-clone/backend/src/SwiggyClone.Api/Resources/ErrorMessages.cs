// File lives in Resources/ folder but uses the root namespace so that
// IStringLocalizer<ErrorMessages> resolves to Resources/ErrorMessages.resx
// when ResourcesPath = "Resources".
namespace SwiggyClone.Api;

/// <summary>
/// Marker class for <see cref="Microsoft.Extensions.Localization.IStringLocalizer{T}"/>
/// resource lookup. The localizer uses this type to find the corresponding
/// <c>ErrorMessages.resx</c> / <c>ErrorMessages.{culture}.resx</c> files.
/// </summary>
public sealed class ErrorMessages;
