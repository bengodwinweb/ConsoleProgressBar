using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleProgressBar
{
    /// <summary>
    /// Displays a progress bar in the console
    /// Includes the progress percent and a spinner
    /// </summary>
    public class ConsoleProgressBar : IProgress<double>, IDisposable
    {
        private readonly int BLOCK_COUNT;
        private readonly char[] SPINNER_CHARS = { '|', '/', '-', '\\' };

        private readonly Timer _timer;

        private double _currentProgress = 0;
        private string _currentText = string.Empty;
        private int _animationIndex = 0;


        public ConsoleProgressBar(int displayChunks = 20)
        {
            BLOCK_COUNT = displayChunks;
            _timer = new Timer(1000 / 8.0);
            _timer.Elapsed += TimerElapsed;

            if (!Console.IsOutputRedirected)
            {
                Console.CursorVisible = false;
                ResetTimer();
            }
        }


        public void Report(double value)
        {
            value = Math.Max(0, Math.Min(1, value));
            _currentProgress = value;
        }


        private void TimerElapsed(object sender, EventArgs e)
        {
            lock (_timer)
            {
                if (_disposed)
                {
                    return;
                }


                double progress = Math.Round(_currentProgress, 3); // reference this once in case it is changed on the other thread

                int blocks = (int) (progress * BLOCK_COUNT);
                double percent = progress * 100;

                string text =
                    string.Format(
                        "[{0}{1}] {2:N1}% {3}",
                        new string('#', blocks),
                        new string('-', BLOCK_COUNT - blocks),
                        percent,
                        SPINNER_CHARS[_animationIndex++ % SPINNER_CHARS.Length]
                    );

                UpdateText(text);
                ResetTimer();
            }
        }

        private void UpdateText(string text)
        {
            if (text == _currentText)
            {
                return;
            }

            int commonPrefixLength = 0;
            int commonLength = Math.Min(_currentText.Length, text.Length);

            while (commonPrefixLength < commonLength && _currentText[commonPrefixLength] == text[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            StringBuilder output = new StringBuilder();
            output.Append(new string('\b', _currentText.Length - commonPrefixLength));
            output.Append(text.Substring(commonPrefixLength));

            // in case the new text is shorter than the previous
            int overlapCount = _currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                output.Append(new string(' ', overlapCount));
                output.Append(new string('\b', overlapCount));
            }

            Console.Write(output);
            _currentText = text;
        }

        void ResetTimer()
        {
            _timer.Start();
        }


        #region IDisposable Support
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            lock (_timer)
            {
                if (!_disposed)
                {
                    _disposed = true;


                    UpdateText("Done");
                    _currentText = null;
                    _timer.Dispose();
                    Console.CursorVisible = true;
                }
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
