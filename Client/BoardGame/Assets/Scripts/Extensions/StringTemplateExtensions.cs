using Assets.Scripts.Service;
using DataContainer.Generated;
using Dignus.Log;
using Dignus.Unity.DependencyInjection;

namespace Assets.Scripts.Extensions
{
    public static class StringHelper
    {
        public static string GetString(int id)
        {
            var template = TemplateContainer<StringTemplate>.Find(id);

            if (template.Invalid())
            {
                LogHelper.Error($"invalid template id : {id}");
                return null;
            }
            var userService = DignusUnityServiceContainer.Resolve<UserService>();

            var userModel = userService.GetUserModel();

            if (userModel.LanguageType == Internals.LanguageType.Kor)
            {
                return template.Korean;
            }
            else if (userModel.LanguageType == Internals.LanguageType.Eng)
            {
                return template.English;
            }
            else
            {
                LogHelper.Error($"invalid language : {userModel.LanguageType}");
                return null;
            }
        }
    }
}
