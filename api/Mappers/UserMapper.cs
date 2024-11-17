using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Models;

namespace api.Mappers
{
    public static class UserMapper
    {
        public static User ToUserFromRegisterDto(this UserRegisterDto userRegisterDto) 
        {
            return new User{
                FullName = userRegisterDto.FullName,
                Password = userRegisterDto.Password,
                Email = userRegisterDto.Password,
                BirthDate = userRegisterDto.BirthDate,
                Gender = userRegisterDto.Gender,
                PhoneNumber = userRegisterDto.PhoneNumber
            };
        }
    }
}