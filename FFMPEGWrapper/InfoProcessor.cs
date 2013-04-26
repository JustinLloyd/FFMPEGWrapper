/*
 * This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text.RegularExpressions;
using System.Drawing;

namespace FFMPEGWrapper
{
    public class InfoProcessor
    {
        public static TimeSpan GetDuration(string outputCapture)
        {
            Match m = Regex.Match(outputCapture, @"[D|d]uration:.((\d|:|\.)*)");
            if (m.Success == false)
            {
                return TimeSpan.Zero;
            }

            string duration = m.Groups[1].Value;
            string[] timepieces = duration.Split(new char[] { ':', '.' });
            if (timepieces.Length == 4)
            {
                return new TimeSpan(0, Convert.ToInt16(timepieces[0]), Convert.ToInt16(timepieces[1]), Convert.ToInt16(timepieces[2]), Convert.ToInt16(timepieces[3]));
            }

            return TimeSpan.Zero;
        }

        public static double GetAudioBitRate(string outputCapture)
        {
            Match m = Regex.Match(outputCapture, @"[B|b]itrate:.((\d|:)*)");
            if (m.Success == false)
            {
                return 0.0;
            }

            double kb = 0.0;
            Double.TryParse(m.Groups[1].Value, out kb);

            return kb;
        }

        public static string GetAudioFormat(string outputCapture)
        {
            Match m = Regex.Match(outputCapture, @"[A|a]udio:(.*)");
            if (m.Success == false)
            {
                return String.Empty;
            }

            return m.Captures[0].Value;
        }

        public static string GetVideoFormat(string outputCapture)
        {
            Match m = Regex.Match(outputCapture, @"[V|v]ideo:(.*)");
            if (m.Success == false)
            {
                return string.Empty;
            }

            return m.Captures[0].Value;
        }

        public static Size GetVideoDimensions(string outputCapture)
        {
            Match m = Regex.Match(outputCapture, @"(\d{2,4})x(\d{2,4})");
            if (m.Success == false)
            {
                return Size.Empty;
            }

            int w;
            int h;

            int.TryParse(m.Groups[1].Value, out w);
            int.TryParse(m.Groups[2].Value, out h);

            return new Size(w, h);
        }

    }

}
