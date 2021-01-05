using System;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Runners;
using IronOcr;

namespace H.Runners
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class OcrRunner : Runner
    {
        #region Properties

        private IronTesseract? Ocr { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public OcrRunner()
        {
            Add(new AsyncAction("ocr", async (command, cancellationToken) =>
            {
                var path = command.Input.Argument;

                var text = await OcrAsync(path, cancellationToken).ConfigureAwait(false);

                return new Value(text);
            }));
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public Task InitializeAsync(CancellationToken _ = default)
        {
            Ocr ??= new IronTesseract();

            return Task.FromResult(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> OcrAsync(string path, CancellationToken cancellationToken = default)
        {
            if (Ocr == null)
            {
                await InitializeAsync(cancellationToken).ConfigureAwait(false);
            }
            Ocr = Ocr ?? throw new InvalidOperationException("Ocr is null.");

            using var input = new OcrInput(path);

            input.Deskew();

            var result = await Ocr.ReadAsync(input).ConfigureAwait(false);

            return result.Text;
        }

        #endregion
    }
}
