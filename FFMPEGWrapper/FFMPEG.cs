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
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace FFMPEGWrapper
{
    public static class FFMPEG
    {
        public static string FFMPEGExecutableFilePath;

        private const int MaximumBuffers = 25;

        public static Queue<string> PreviousBuffers = new Queue<string>();

        static FFMPEG()
        {
            FFMPEGExecutableFilePath = ConfigurationManager.AppSettings["FFMPEGExecutableFilePath"];
        }

        public static string Execute(string inputFilePath)
        {
            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Input file path cannot be null");
            }

            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = inputFilePath
            };

            return Execute(parameters);
        }

        public static string Execute(string inputFilePath, string outputOptions, string outputFilePath)
        {
            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Input file path cannot be null");
            }

            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Output file path cannot be null");
            }

            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = inputFilePath,
                OutputOptions = outputOptions,
                OutputFilePath = outputFilePath,
            };

            return Execute(parameters);

        }

        public static string Execute(string inputFilePath, string outputOptions)
        {
            if (String.IsNullOrWhiteSpace(inputFilePath))
            {
                throw new ArgumentNullException("Input file path cannot be null");
            }

            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = inputFilePath,
                OutputOptions = outputOptions
            };

            return Execute(parameters);
        }

        public static string Execute(FFMPEGParameters parameters)
        {
            if (String.IsNullOrWhiteSpace(FFMPEGExecutableFilePath))
            {
                throw new ArgumentNullException("Path to FFMPEG executable cannot be null");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("FFMPEG parameters cannot be completely null");
            }

            using (Process ffmpegProcess = new Process())
            {
                ProcessStartInfo info = new ProcessStartInfo(FFMPEGExecutableFilePath)
                {
                    Arguments = parameters.ToString(),
                    WorkingDirectory = Path.GetDirectoryName(FFMPEGExecutableFilePath),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                ffmpegProcess.StartInfo = info;
                ffmpegProcess.Start();
                string processOutput = ffmpegProcess.StandardError.ReadToEnd();
                ffmpegProcess.WaitForExit();
                PreviousBuffers.Enqueue(processOutput);
                lock (PreviousBuffers)
                {
                    while (PreviousBuffers.Count > MaximumBuffers)
                    {
                        PreviousBuffers.Dequeue();
                    }

                }

                return processOutput;
            }

        }

    }

}
