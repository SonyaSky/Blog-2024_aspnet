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
        public static SearchAddressModel ToAddressDtoFromAddress(this AddressElement element)
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

        public static SearchAddressModel ToAddressDtoFromHouse(this House element)
        {
            var result =  new SearchAddressModel
            {
                ObjectId = element.ObjectId,
                ObjectGuid = element.ObjectGuid,
                Text = element.HouseNum,
                ObjectLevel = GarAddressLevel.Building,
                ObjectLevelText = GarAddressLevel.Building.GetSecondName()
            };
            if (element.AddNum1 != null)
            {
                switch (element.AddType1)
                {
                    case 1:
                        result.Text += " к. " + element.AddNum1;
                        break;
                    case 2:
                        result.Text += " стр. " + element.AddNum1;
                        break;
                    case 3:
                        result.Text += " соор. " + element.AddNum1;
                        break;
                    case 4:
                        result.Text += " " + element.AddNum1;
                        break;
                    default:
                        break;
                }
            }
            if (element.AddNum2 != null)
            {
                switch (element.AddType2)
                {
                    case "2":
                        result.Text += " стр. " + element.AddNum2;
                        break;
                    case "3":
                        result.Text += " соор. " + element.AddNum2;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public static string GetSecondName(this GarAddressLevel level)
        {
            var fieldInfo = typeof(GarAddressLevel).GetField(level.ToString());
            var attribute = (AddressLevelAttribute)fieldInfo.GetCustomAttributes(typeof(AddressLevelAttribute), false).FirstOrDefault();
            return attribute != null ? attribute.SecondName : string.Empty;
        }
    }
}