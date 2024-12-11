using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Address;
using api.Models;

namespace api.Mappers
{
    public static class AddressMapper
    {
        public static SearchAddressModel ToAddressDto(this AddressElement element)
        {
            return new SearchAddressModel
            {
                ObjectId = element.ObjectId,
                ObjectGuid = element.ObjectGuid,
                Text = element.TypeName + " " + element.Name,
                ObjectLevel = (GarAddressLevel)element.Level,
                ObjectLevelText = ((GarAddressLevel)element.Level).GetSecondName()
            };
        }

        public static string GetSecondName(this GarAddressLevel level)
        {
            var fieldInfo = typeof(GarAddressLevel).GetField(level.ToString());
            var attribute = (AddressLevelAttribute)fieldInfo.GetCustomAttributes(typeof(AddressLevelAttribute), false).FirstOrDefault();
            return attribute != null ? attribute.SecondName : string.Empty;
        }
    }
}