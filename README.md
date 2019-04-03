# automapper-object-initializer
This library provides ability to use object initializer expressions to configure member mappings in AutoMapper

Inspired by https://habr.com/en/post/338968/. See https://github.com/Birbone/FlashMapper.

For example, we would like to setup `AutoMapper` mapping from `UserModel` to `UserEntity`:
```c#
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
```

Without `automapper-object-initializer`:
```c#
MapperConfiguration configuration = new MapperConfiguration(cfg =>
{
  cfg.CreateMap<UserModel, UserEntity>()
    .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Name))
    .ForMember(dst => dst.IntDestinationProperty, opt => opt.MapFrom(src => src.IntSourceProperty))
    .ForMember(dst => dst.DateDestinationProperty, opt => opt.MapFrom(src => src.StringDateSourceProperty))
    .ForMember(dst => dst.PropertyToSetConstant, opt => opt.MapFrom(src => "5"))
    .ForMember(dst => dst.PropertyToIgnore, opt => opt.Ignore());
});
```

With `automapper-object-initializer`:
```c#
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
```
