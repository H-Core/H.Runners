using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using H.Core.Utilities;

namespace H.Runners.IntegrationTests
{
    /// <summary>
    /// 
    /// </summary>
    public class TestWpfApp : IDisposable
    {
        #region Static methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<TestWpfApp> CreateAsync(
            CancellationToken cancellationToken = default)
        {
            if (Application.Current != null)
            {
                return new TestWpfApp(Application.Current);
            }

            using var exceptions = new ExceptionsBag();
            var thread = new Thread(() =>
            {
                try
                {
                    new Application()
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown,
                    }.Run();
                }
                catch (Exception exception)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    exceptions.OnOccurred(exception);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            while (Application.Current == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken)
                    .ConfigureAwait(false);
            }

            return new TestWpfApp(Application.Current);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Application Application { get; }

        /// <summary>
        /// 
        /// </summary>
        public Dispatcher Dispatcher => Application.Dispatcher;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TestWpfApp(Application application)
        {
            Application = application ?? throw new ArgumentNullException(nameof(application));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispatcher.InvokeShutdown();
        }

        #endregion
    }
}
