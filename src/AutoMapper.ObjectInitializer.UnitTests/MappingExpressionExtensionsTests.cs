using System;
using System.Linq.Expressions;
using Should;
using Xunit;

namespace AutoMapper.ObjectInitializer.UnitTests
{
    public class MappingExpressionExtensionsTests
    {
        public class UserModel
        {
            public string Name { get; set; }
        }

        public class UserEntity
        {
            public string Title { get; set; }
        }

        [Fact]
        public void TestForMember()
        {
            // Arrange
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, UserEntity>()
                    .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Name));
            });
            IMapper mapper = configuration.CreateMapper();
            UserModel userModel = new UserModel
            {
                Name = "Name"
            };

            // Act
            UserEntity userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            // Assert
            userEntity.ShouldNotBeNull();
            userEntity.Title.ShouldEqual(userModel.Name);
            configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public void TestMapUsingWithInvalidExpression()
        {
            // Act
            ArgumentException exception = Assert.Throws<ArgumentException>(() => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, UserEntity>()
                    .MapUsing(src => null);
            }));

            // Assert
            exception.Message.ShouldContain("Parameter is not an object initializer expression.");
        }

        [Fact]
        public void TestMapUsingWithNewExpression()
        {
            // Arrange
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, UserEntity>()
                    .MapUsing(src => new UserEntity());
            });
            IMapper mapper = configuration.CreateMapper();
            UserModel userModel = new UserModel
            {
                Name = "Name"
            };

            // Act
            UserEntity userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            // Assert
            userEntity.ShouldNotBeNull();
        }

        [Fact]
        public void TestMapUsingWithMemberInitExpression()
        {
            // Arrange
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, UserEntity>()
                    .MapUsing(src => new UserEntity
                    {
                        Title = src.Name
                    });
            });
            IMapper mapper = configuration.CreateMapper();
            UserModel userModel = new UserModel
            {
                Name = "Name"
            };

            // Act
            UserEntity userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            // Assert
            userEntity.ShouldNotBeNull();
            userEntity.Title.ShouldEqual(userModel.Name);
            configuration.AssertConfigurationIsValid();
        }
    }
}
