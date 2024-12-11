using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Address
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AddressLevelAttribute : Attribute
    {
        public string SecondName { get; set; }

        public AddressLevelAttribute(string secondName)
        {
            SecondName = secondName;
        }
    }
}