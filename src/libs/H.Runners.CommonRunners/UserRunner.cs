using H.Core.Runners;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UserRunner : Runner
    {
        #region Properties

        private string? UserName { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public UserRunner()
        {
            //AddAction("say-my-name", async _ =>
            //{
            //    if (string.IsNullOrWhiteSpace(UserName))
            //    {
            //        await SayAsync("Я еще не знаю вашего имени. Пожалуйста, представьтесь");

            //        var name = await WaitNextCommand(8000);
            //        if (name == null || string.IsNullOrWhiteSpace(name))
            //        {
            //            return;
            //        }

            //        // First char to upper case
            //        name = name[0].ToString().ToUpper() + name.Substring(1);

            //        Settings.Set("username", name);
            //        SaveSettings();
            //    }

            //    Say($"Привет {UserName}");
            //});
            AddSetting("username", o => UserName = o, NoEmpty, string.Empty);

            AddVariable("$username$", () => UserName ?? string.Empty);
        }

        #endregion
    }
}
