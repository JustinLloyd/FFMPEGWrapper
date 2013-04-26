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
using System.Linq;
using System.Drawing;
using System.IO;
using FFMPEGWrapper;

namespace FFMPEGHarness
{
    class Program
    {
        private static readonly string m_outputFilePath;
        private static readonly string m_inputFilePath;

        static Program()
        {
            m_inputFilePath = Directory.GetFiles(".", "*.flv").First();
            m_outputFilePath = Path.Combine(Path.GetDirectoryName(m_inputFilePath), Path.ChangeExtension(String.Format("{0}_modified", Path.GetFileNameWithoutExtension(m_inputFilePath)), Path.GetExtension(m_inputFilePath)));
        }

        public static void VideoInfo()
        {
            string output = FFMPEG.Execute(m_inputFilePath);

            Console.WriteLine("Duration = " + InfoProcessor.GetDuration(output));
            Console.WriteLine("Audio Rate = " + InfoProcessor.GetAudioBitRate(output));
            Console.WriteLine("Audio Format = " + InfoProcessor.GetAudioFormat(output));
            Console.WriteLine("Video Format = " + InfoProcessor.GetVideoFormat(output));
            Console.WriteLine("Video Dimensions = " + InfoProcessor.GetVideoDimensions(output));
        }

        private static void ResizeVideo()
        {
            FFMPEGParameters parameters = new FFMPEGParameters()
            {
                InputFilePath = m_inputFilePath,
                OutputFilePath = m_outputFilePath,
                VideoCodec = "libx264",
                AudioCodec = "libvo_aacenc",
                Format = "flv",
                BufferSize = 50000,
                Size = new Size(320, 240),
                MaximumRate = 400,
                Overwrite = true,
                OutputOptions = "-ar 22050 -ab 128k -ac 1",
            };

            string output = FFMPEG.Execute(parameters);
        }

        private static void WatermarkVideo()
        {
            Tuple<WatermarkPosition, Point, string>[] testPositions = new Tuple<WatermarkPosition, Point, string>[]
            {
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.BottomRight, new Point(15,15), "bottom_right.flv"),
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.BottomLeft, new Point(15,15), "bottom_left.flv"),
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.TopRight, new Point(15,15), "top_right.flv"),
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.TopLeft, new Point(15,15), "top_left.flv"),
                
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.Center, new Point(0,0), "center.flv"),
                
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.CenterTop, new Point(0,15), "center_top.flv"),
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.CenterBottom, new Point(0,15), "center_bottom.flv"),
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.MiddleLeft, new Point(15,0), "middle_left.flv"),
                new Tuple<WatermarkPosition, Point, string>(WatermarkPosition.MiddleRight, new Point(15,0), "middle_right.flv"),
            };


            string watermarkFilePath = Directory.GetFiles(".", "*.png").First();
            VideoFile videoFile = new VideoFile(m_inputFilePath);
            // test all permitted position combinations for a watermark
            foreach (var testPosition in testPositions)
            {
                string newFilePath = videoFile.WatermarkVideo(watermarkFilePath, false, testPosition.Item1, testPosition.Item2);
                File.Delete(Path.Combine(Path.GetDirectoryName(m_inputFilePath), testPosition.Item3));
                File.Move(newFilePath, Path.Combine(Path.GetDirectoryName(m_inputFilePath), testPosition.Item3));
            }

        }

        static void Main(string[] args)
        {
            VideoFile videoFile = new VideoFile(m_inputFilePath);
            Console.WriteLine("File: " + m_inputFilePath);
            Console.WriteLine("Duration = " + videoFile.Duration);
            Console.WriteLine("Dimensions = " + videoFile.Dimensions);

            VideoInfo();
            ResizeVideo();
            WatermarkVideo();
        }


    }
}
