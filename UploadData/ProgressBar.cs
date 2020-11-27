using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UploadData
{
    public class ProgressBar
    {
        private int _lastOutputLength;
        private readonly int _total;
        private readonly int _maximumWidth;
        Stopwatch Stopwatch;
        public ProgressBar(string titel,int total, int maximumWidth = 100)
        {
            _maximumWidth = maximumWidth;
            _total = total;
            Show($"{titel} [ ");
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public void Update(double currentProgress)
        {
            // Remove the last state           
            string clear = string.Empty.PadRight(_lastOutputLength, '\b');
            string FormatString = ""; //{0:%d} days . {0:%h} hours : {0:%mm} minutes";
            TimeSpan EtaTime = Stopwatch.GetEta((int)currentProgress, _total);
            if (EtaTime.Days > 0)
                FormatString = "{0:%d} days . {0:%h} hours : {0:%m} minutes : {0:%s} seconds";
            else if (EtaTime.Hours > 0)
                FormatString = "{0:%h} hours : {0:%m} minutes : {0:%s} seconds";
            else if(EtaTime.Minutes > 0)
                FormatString = "{0:%m} minutes : {0:%s} seconds";
            else
                FormatString = "{0:%s} seconds";



            Show(clear);

            // Generate new state           
            int width = (int)(currentProgress / _total * _maximumWidth);
            int fill = _maximumWidth - width;
            string output = string.Format("{0}{1}{2} ] {3}% ({4}/{5}) time remaining: {6}", 
                string.Empty.PadLeft(width, '='), 
                width < _maximumWidth ? '>' : '=' , 
                string.Empty.PadLeft(fill, ' '), 
                ((currentProgress / _total * 100)).ToString("0.00"),
                (int)currentProgress,
                _total,
                string.Format(FormatString, EtaTime));
            Show(output);
            _lastOutputLength = output.Length;
        }

        private void Show(string value)
        {
            Console.Write(value);
        }
    }
}
