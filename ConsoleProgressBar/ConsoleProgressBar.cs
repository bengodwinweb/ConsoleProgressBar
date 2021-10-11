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
    /// Includes the progress percent and a spinner.
    /// 
    /// Updates the console on a background thread. 
    /// Writes output 8 times per second, regardless of how many times the user calls Report()
    /// </summary>
    public class ConsoleProgressBar : IProgress<double>, IDisposable
    {
        private readonly int BLOCK_COUNT; // total blocks drawn in progress bar
        private readonly char[] SPINNER_CHARS = { '|', '/', '-', '\\' }; // characters for the spinner animation

        private readonly Timer _timer;

        /// <summary>
        /// Current progress of the task from 0.0 to 1.0
        /// </summary>
        private double _currentProgress = 0;

        /// <summary>
        /// Last text printed to the console
        /// </summary>
        private string _currentText = string.Empty;

        /// <summary>
        /// Index of current SPINNER_CHARS character used in the spinner animation
        /// </summary>
        private int _animationIndex = 0;


        /// <summary>
        /// Instantiates the progress bar, and begins printing progress to the Console.
        /// Progress will continue to be printed until the object is Disposed
        /// </summary>
        /// <param name="displayChunks">Number of blocks used to display progress</param>
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

        /// <summary>
        /// Updates the progress that will be reported the next time the display is updated
        /// </summary>
        /// <param name="value"></param>
        public void Report(double value)
        {
            value = Math.Max(0, Math.Min(1, value));
            _currentProgress = value;
        }

        /// <summary>
        /// Timer event handler. Determine the new display text and update the console
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerElapsed(object sender, EventArgs e)
        {
            lock (_timer)
            {
                if (_disposed)
                {
                    return;
                }

                // reference _currentProgress once in case it is changed on the other thread
                double progress = Math.Round(_currentProgress, 3); // round to the nearest tenth of a percent (_currentProgress is 0 - 1)

                int blocks = (int) (progress * BLOCK_COUNT); // determine the number of blocks that should be filled in
                double percent = progress * 100; // determine the percentage that should be shown to the user (e.g.: 62.4 when _currentProgress is .624). 

                // [###-------] 31.4% (spinner char)
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
            // text will always be different in at least one character because of the spinner

            // Keep the current text up until there is a difference
            int commonPrefixLength = 0;
            int commonLength = Math.Min(_currentText.Length, text.Length);

            while (commonPrefixLength < commonLength && _currentText[commonPrefixLength] == text[commonPrefixLength])
            {
                commonPrefixLength++;
            }

            StringBuilder output = new StringBuilder();
            output.Append(new string('\b', _currentText.Length - commonPrefixLength)); // delete from the current end back until the last character that remained the same
            output.Append(text.Substring(commonPrefixLength)); // add the new text after the change

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
