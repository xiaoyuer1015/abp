using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace TestApp;

public class MyPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group1 = context.AddGroup("GroupDynamic1");
        group1.AddPermission("PermissionDynamic1", new FixedLocalizableString("Dynamic permission one SAK"));
        var permission2 = group1.AddPermission("PermissionDynamic2", new FixedLocalizableString("Dynamic permission two XY"));
        permission2.AddChild("PermissionDynamic2.1", new FixedLocalizableString("Dynamic permission two.1"));
        group1.AddPermission("PermissionDynamic3", new FixedLocalizableString("Dynamic permission three TA"));
    }
}