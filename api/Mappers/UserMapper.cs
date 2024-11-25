using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Dtos.User;
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
                Email = userRegisterDto.Email,
                BirthDate = userRegisterDto.BirthDate,
                Gender = userRegisterDto.Gender,
                PhoneNumber = userRegisterDto.PhoneNumber,
                UserName = userRegisterDto.Email
            };
        }

        public static UserDto ToUserDto(this User user) 
        {
            return new UserDto{
                Id = new Guid(user.Id),
                FullName = user.FullName,
                CreateTime = user.CreateTime,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Gender = user.Gender == 0 ? Gender.Male : Gender.Female,
                PhoneNumber = user.PhoneNumber
            };
            
        }
    }
}