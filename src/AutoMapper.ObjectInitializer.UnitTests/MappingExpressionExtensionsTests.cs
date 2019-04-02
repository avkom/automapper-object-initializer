using System;
using Should;
using Xunit;

namespace AutoMapper.ObjectInitializer.UnitTests
{
    public class MappingExpressionExtensionsTests
    {
        public class UserModel
        {
            public string Name { get; set; }
            public int IntSourceProperty { get; set; }
        }

        public class UserEntity
        {
            public string Title { get; set; }
            public int IntDestinationProperty { get; set; }
            public string PropertyToSetConstant { get; set; }
            public string PropertyToIgnore { get; set; }
        }

        [Fact]
        public void TestForMember()
        {
            // Arrange
            MapperConfiguration configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserModel, UserEntity>()
                    .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dst => dst.IntDestinationProperty, opt => opt.MapFrom(src => src.IntSourceProperty))
                    .ForMember(dst => dst.PropertyToSetConstant, opt => opt.MapFrom(src => "5"))
                    .ForMember(dst => dst.PropertyToIgnore, opt => opt.Ignore());
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
            userEntity.IntDestinationProperty.ShouldEqual(userModel.IntSourceProperty);
            userEntity.PropertyToSetConstant.ShouldEqual("5");
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
                        Title = src.Name,
                        IntDestinationProperty = src.IntSourceProperty,
                        PropertyToSetConstant = "5",
                        PropertyToIgnore = default
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
            userEntity.IntDestinationProperty.ShouldEqual(userModel.IntSourceProperty);
            userEntity.PropertyToSetConstant.ShouldEqual("5");
            configuration.AssertConfigurationIsValid();
        }
    }
}
