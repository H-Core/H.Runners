using System.Globalization;
using H.Core;
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
            Add(AsyncAction.WithoutArguments("say-my-name", async cancellationToken =>
            {
                if (string.IsNullOrWhiteSpace(UserName))
                {
                    await this.SayAsync("Я еще не знаю вашего имени. Пожалуйста, представьтесь", cancellationToken)
                        .ConfigureAwait(false);

                    var name = await this.GetNextCommandAsync(cancellationToken)
                        .ConfigureAwait(false);
                    if (name == null || string.IsNullOrWhiteSpace(name))
                    {
                        return;
                    }

                    // First char to upper case
                    name = name[0].ToString().ToUpper(CultureInfo.InvariantCulture) + name.Substring(1);

                    UserName = name;
                }

                await this.SayAsync($"Привет, {UserName}", cancellationToken).ConfigureAwait(false);
            }));
            AddSetting("username", o => UserName = o, NoEmpty, string.Empty);

            AddVariable("$username$", () => UserName ?? string.Empty);
        }

        #endregion
    }
}
