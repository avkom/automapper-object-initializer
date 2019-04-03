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
            public string StringDateSourceProperty { get; set; }
        }

        public class UserEntity
        {
            public string Title { get; set; }
            public int IntDestinationProperty { get; set; }
            public DateTime DateDestinationProperty { get; set; }
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
                    .ForMember(dst => dst.DateDestinationProperty, opt => opt.MapFrom(src => src.StringDateSourceProperty))
                    .ForMember(dst => dst.PropertyToSetConstant, opt => opt.MapFrom(src => "5"))
                    .ForMember(dst => dst.PropertyToIgnore, opt => opt.Ignore());
            });
            IMapper mapper = configuration.CreateMapper();
            UserModel userModel = new UserModel
            {
                Name = "Name",
                IntSourceProperty = 10,
                StringDateSourceProperty = "2019-01-02T11:22:33.456Z"
            };

            // Act
            UserEntity userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            // Assert
            userEntity.ShouldNotBeNull();
            userEntity.Title.ShouldEqual(userModel.Name);
            userEntity.IntDestinationProperty.ShouldEqual(userModel.IntSourceProperty);
            userEntity.DateDestinationProperty.ToString().ShouldEqual(new DateTime(2019, 1, 2, 11, 22, 33, DateTimeKind.Utc).ToLocalTime().ToString());
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
                    .MapUsing(src => new UserEntity());
            }));

            // Assert
            exception.Message.ShouldContain("Parameter is not an object initializer expression.");
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
                        DateDestinationProperty = MappingOptions.MapFrom<DateTime>(src.StringDateSourceProperty),
                        PropertyToSetConstant = "5",
                        PropertyToIgnore = default
                    });
            });
            IMapper mapper = configuration.CreateMapper();
            UserModel userModel = new UserModel
            {
                Name = "Name",
                IntSourceProperty = 10,
                StringDateSourceProperty = "2019-01-02T11:22:33.456Z"
            };

            // Act
            UserEntity userEntity = mapper.Map<UserModel, UserEntity>(userModel);

            // Assert
            userEntity.ShouldNotBeNull();
            userEntity.Title.ShouldEqual(userModel.Name);
            userEntity.IntDestinationProperty.ShouldEqual(userModel.IntSourceProperty);
            userEntity.DateDestinationProperty.ToString().ShouldEqual(new DateTime(2019, 1, 2, 11, 22, 33, DateTimeKind.Utc).ToLocalTime().ToString());
            userEntity.PropertyToSetConstant.ShouldEqual("5");
            configuration.AssertConfigurationIsValid();
        }
    }
}
