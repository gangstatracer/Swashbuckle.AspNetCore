using System;
using System.Linq;
using System.Text.Json;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public class JsonApiModelResolver : IApiModelResolver
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly SchemaGeneratorOptions _options;

        public JsonApiModelResolver(JsonSerializerOptions jsonSerializerOptions, SchemaGeneratorOptions options)
        {
            _jsonSerializerOptions = jsonSerializerOptions;
            _options = options;
        }

        public ApiModel ResolveApiModelFor(Type type)
        {
            // If it's nullable, use the inner value type
            var underlyingType = type.IsNullable(out Type innerType)
                ? innerType
                : type;

            if (underlyingType.IsValueType || underlyingType == typeof(string) || underlyingType == typeof(byte[]))
                return ResolveApiPrimitive(underlyingType);

            if (underlyingType.IsDictionary(out Type keyType, out Type valueType))
                return ResolveApiDictionary(underlyingType, keyType, valueType);

            if (underlyingType.IsEnumerable(out Type itemType))
                return ResolveApiArray(underlyingType, itemType);

            return ResolveApiObject(underlyingType);
        }

        private ApiModel ResolveApiPrimitive(Type type)
        {
            if (!type.IsEnum)
            {
                return new ApiPrimitive(type);
            }

            //var stringEnumConverter = (jsonPrimitiveContract.Converter as StringEnumConverter)
            //    ?? _jsonSerializerSettings.Converters.OfType<StringEnumConverter>().FirstOrDefault();

            //// Temporary shim to support obsolete config options
            //if (stringEnumConverter == null && _options.DescribeAllEnumsAsStrings)
            //{
            //    stringEnumConverter = new StringEnumConverter(_options.DescribeStringEnumsInCamelCase);
            //};

            return new ApiPrimitive(
                type: type,
                isStringEnum: false,
                apiEnumValues: type.GetEnumValues().Cast<object>());

            //var enumValues = type.GetMembers(BindingFlags.Public | BindingFlags.Static)
            //    .Select(enumMember =>
            //    {
            //        var enumMemberAttribute = enumMember.GetCustomAttributes<EnumMemberAttribute>().FirstOrDefault();
            //        return GetConvertedEnumName(stringEnumConverter, (enumMemberAttribute?.Value ?? enumMember.Name), (enumMemberAttribute?.Value != null));
            //    })
            //    .Distinct();
        }

        private ApiModel ResolveApiDictionary(Type type, Type keyType, Type valueType)
        {
            return new ApiDictionary(
                type: type,
                keyType: keyType,
                valueType: valueType);
        }

        private ApiModel ResolveApiArray(Type type, Type itemType)
        {
            return new ApiArray(
                type: type,
                itemType: itemType);
        }

        private ApiModel ResolveApiObject(Type type)
        {
            if (type == typeof(object))
            {
                return new ApiObject(
                    type: type,
                    apiProperties: Enumerable.Empty<ApiProperty>());
            }

            return new ApiModel(type);

            //var apiProperties = jsonObjectContract.Properties
            //    .Where(jsonProperty => !jsonProperty.Ignored)
            //    .Select(jsonProperty =>
            //    {
            //        var memberInfo = jsonProperty.DeclaringType.GetMember(jsonProperty.UnderlyingName).FirstOrDefault();
            //        var jsonPropertyAttributeData = memberInfo?.GetCustomAttributesData()
            //            .FirstOrDefault(attrData => attrData.AttributeType == typeof(JsonPropertyAttribute));

            //        var required = (jsonPropertyAttributeData == null || !jsonPropertyAttributeData.NamedArguments.Any(arg => arg.MemberName == "Required"))
            //            ? jsonObjectContract.ItemRequired
            //            : jsonProperty.Required;

            //        return new ApiProperty(
            //            apiName: jsonProperty.PropertyName,
            //            type: jsonProperty.PropertyType,
            //            apiRequired: (required == Required.Always || required == Required.AllowNull),
            //            apiNullable: (required != Required.Always && required != Required.DisallowNull && jsonProperty.PropertyType.IsReferenceOrNullableType()),
            //            apiReadOnly: (jsonProperty.Readable && !jsonProperty.Writable),
            //            apiWriteOnly: (jsonProperty.Writable && !jsonProperty.Readable),
            //            memberInfo: memberInfo);
            //    });

            //return new ApiObject(
            //    type: type,
            //    apiProperties: apiProperties,
            //    additionalPropertiesType: jsonObjectContract.ExtensionDataValueType);
        }
    }
}
