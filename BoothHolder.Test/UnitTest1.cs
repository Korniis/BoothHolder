using BoothHolder.Model.DTO;
using BoothHolder.Model.Entity;
using Mapster;

namespace BoothHolder.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // TypeAdapterConfig ����
            var config = new TypeAdapterConfig();
            config.NewConfig<User, UserDTO>()
                  .Map(dest => dest.RoleNames, src => src.RoleList.Select(r => r.RoleName).ToList());

            // ���� User ʵ�����
            var user = new User
            {
                UserName = "John",
                Email = "john@example.com",
                Phone = "123456789",
                RoleList = new List<Role>
                {
                    new Role { RoleID = 1, RoleName = "Admin" },
                    new Role { RoleID = 2, RoleName = "User" }
                }
            };

            // ʹ�� Adapt ����ӳ��
            var userDto = user.Adapt<UserDTO>(config);

            // ������
            Console.WriteLine($"UserName: {userDto.UserName}, Roles: {string.Join(", ", userDto.RoleNames)}");

        }
    }
}